using KaraokeLib.Lyrics;
using KaraokeLib.Util;
using KaraokeLib.Video;
using SkiaSharp;

namespace KaraokeStudio.Timeline
{
	/// <summary>
	/// The actual canvas that the timeline is drawn to.
	/// The TimelineControl presents a scrolled view into this canvas.
	/// </summary>
	internal class TimelineCanvas
	{
		public float TrackHeight => 50.0f;
		public float PixelsPerSecond => 50.0f;

		private SKPictureRecorder _pictureRecorder;
		private SKPicture? _picture = null;
		private KaraokeProject? _project = null;
		private SKSize _size = new SKSize(1, 1);

		private SKColor _backgroundColor;
		private SKPaint _lightPaint;
		private SKPaint _textPaint;
		private SKPaint _borderPaint;
		private SKPaint _highlightPaint;
		private SKPaint _strokePaint;
		private SKPaint _selectedStrokePaint;

		private SKFont _font;
		private float _ellipsisWidth;
		private float _lineHeight;
		private Dictionary<string, SKRect> _textBounds = new Dictionary<string, SKRect>();

		private ClickableItem? _selectedEvent;

		private ClickableItem[][] _clickableItems = new ClickableItem[0][];

		private Dictionary<LyricsEventType, SKPaint> _eventTypePaints = new Dictionary<LyricsEventType, SKPaint>();
		private Dictionary<int, LyricsEvent> _events = new Dictionary<int, LyricsEvent>();

		public SKSize Size => _size;

		public TimelineCanvas()
		{
			_pictureRecorder = new SKPictureRecorder();

			_backgroundColor = VisualStyle.NeutralDarkColor.ToSKColor();
			_borderPaint = new SKPaint() { Color = VisualStyle.BorderColor.ToSKColor() };
			_lightPaint = new SKPaint() { Color = VisualStyle.NeutralLightColor.ToSKColor() };
			_textPaint = new SKPaint()
			{
				Color = VisualStyle.NeutralLightColor.ToSKColor(),
				IsAntialias = true
			};
			_highlightPaint = new SKPaint() { Color = VisualStyle.HighlightColor.ToSKColor() };
			_strokePaint = new SKPaint()
			{
				Color = VisualStyle.BorderColor.ToSKColor(),
				IsStroke = true,
				IsAntialias = true
			};
			_selectedStrokePaint = new SKPaint()
			{
				Color = VisualStyle.NeutralLightColor.ToSKColor(),
				IsStroke = true,
				StrokeWidth = 2,
				IsAntialias = true
			};

			foreach (var pair in VisualStyle.EventColors)
			{
				_eventTypePaints.Add(pair.Key, new SKPaint() { Color = pair.Value.ToSKColor() });
			}

			_font = new SKFont(VisualStyle.DefaultTypeface, 6.0f);

			// overestimate ellipsis size to accomodate spacing - a bit of a hack, to be quite honest
			var dotGlyph = _font.GetGlyph('.');
			_font.MeasureText(new ushort[] { dotGlyph, dotGlyph, dotGlyph, dotGlyph }, out var ellipsisBounds);
			_ellipsisWidth = ellipsisBounds.Width;

			_lineHeight = StyleUtil.GetFontHeight(_font);
		}

		/// <summary>
		/// Copy the contents of the timeline canvas onto the given destination canvas.
		/// </summary>
		/// <param name="destination">The canvas to copy onto.</param>
		/// <param name="matrix">The matrix to use to zoom and pan the canvas.</param>
		/// <param name="visibleRect">The rect in canvas space that's visible in the control.</param>
		public void CopyContents(SKCanvas destination, SKMatrix matrix, SKRect visibleRect)
		{
			if (_picture != null)
			{
				destination.DrawPicture(_picture, ref matrix);
			}

			if (_selectedEvent != null)
			{
				// draw selection rect
				destination.Save();
				destination.SetMatrix(matrix);
				destination.DrawRect(_selectedEvent.Value.Rect, _selectedStrokePaint);
				destination.Restore();
			}

			for (var i = 0; i < _clickableItems.GetLength(0); i++)
			{
				var arr = _clickableItems[i];
				if (arr == null)
				{
					continue;
				}

				for (var j = 0; j < arr.Length; j++)
				{
					var item = arr[j];
					// this rect is visible!
					if (visibleRect.IntersectsWith(item.Rect) && _events.TryGetValue(item.EventId, out var ev))
					{
						TryDrawEventText(destination, ev, matrix.MapRect(item.Rect));
					}
				}
			}
		}

		public void SelectEventAtPoint(SKPoint point)
		{
			var ev = FindEventAtPoint(point);
			_selectedEvent = ev;
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_project = project;
			_events.Clear();
			_textBounds.Clear();
			_selectedEvent = null;

			var newSize = CalculateSize();
			if (newSize != _size)
			{
				// recreate the canvas if the size has changed
				RecreateCanvas(newSize);
			}
		}

		/// <summary>
		/// Returns an event on the timeline at the given (X, Y) position, if any.
		/// </summary>
		private ClickableItem? FindEventAtPoint(SKPoint point)
		{
			if (_project == null)
			{
				return null;
			}

			var track = (int)Math.Floor(point.Y / TrackHeight);
			if (track < 0 || track >= _clickableItems.GetLength(0))
			{
				return null;
			}

			foreach (var item in _clickableItems[track])
			{
				if (item.Rect.Contains(point) && _events.TryGetValue(item.EventId, out var ev))
				{
					return item;
				}
			}

			return null;
		}

		private SKSize CalculateSize()
		{
			if (_project == null)
			{
				return new SKSize(1, 1);
			}

			var width = (float)(PixelsPerSecond * _project.Length.TotalSeconds);
			var height = _project.Tracks.Count() * TrackHeight;

			return new SKSize(width, height);
		}

		private void RecreateCanvas(SKSize newSize)
		{
			_pictureRecorder.Dispose();

			_size = newSize;
			_pictureRecorder = new SKPictureRecorder();

			RedrawCanvas();
		}

		private void RedrawCanvas()
		{
			var canvas = _pictureRecorder.BeginRecording(new SKRect(0, 0, _size.Width, _size.Height));

			canvas.Clear(_backgroundColor);

			if (_project == null)
			{
				_picture = _pictureRecorder.EndRecording();
				return;
			}

			var projectTracks = _project.Tracks.ToArray();

			if (_clickableItems.GetLength(0) != projectTracks.Length)
			{
				_clickableItems = new ClickableItem[projectTracks.Length][];
			}

			var trackYPos = 0.0f;
			for (var i = 0; i < projectTracks.Length; i++)
			{
				var events = projectTracks[i].Events;
				var items = _clickableItems[i];

				if (items == null || items.Length != events.Count)
				{
					// allocate a new array of clickable items for each event
					items = _clickableItems[i] = new ClickableItem[events.Count];
				}

				// draw track contents
				for (var j = 0; j < events.Count; j++)
				{
					var ev = events[j];
					_events[ev.Id] = ev;

					var eventRect = new SKRect(
						PixelsPerSecond * (float)ev.StartTimeSeconds,
						trackYPos,
						PixelsPerSecond * (float)ev.EndTimeSeconds,
						trackYPos + TrackHeight - 1.0f
					);

					if (!_eventTypePaints.TryGetValue(ev.Type, out var paint))
					{
						paint = _highlightPaint;
					}

					canvas.DrawRect(eventRect, paint);
					canvas.DrawRect(eventRect, _strokePaint);

					items[j] = new ClickableItem()
					{
						Rect = eventRect,
						EventId = ev.Id
					};
				}

				// draw border
				var borderY = trackYPos + TrackHeight;
				canvas.DrawLine(new SKPoint(0, borderY), new SKPoint(_size.Width, borderY), _borderPaint);
				trackYPos += TrackHeight;
			}

			_picture = _pictureRecorder.EndRecording();
		}

		private void TryDrawEventText(SKCanvas canvas, LyricsEvent ev, SKRect rect)
		{
			var eventText = ev.GetText(null);

			// draw text if we can fit it
			if (string.IsNullOrWhiteSpace(eventText))
			{
				return;
			}

			var padding = 3.0f;
			var paddedRect = new SKRect(
				rect.Left + padding,
				rect.Top + padding,
				rect.Right - padding,
				rect.Bottom - padding);

			var glyphs = eventText.ToCharArray().Select(c => _font.GetGlyph(c)).ToArray();
			var glyphsSpan = new ReadOnlySpan<ushort>(glyphs);

			var widthSoFar = 0.0f;
			var lastDrawableIndex = -1;
			var lastAbbrevIndex = -1;
			var glyphsSoFar = new List<ushort>();
			for (var i = 0; i < glyphs.Length; i++)
			{
				var text = eventText.Substring(0, i + 1);
				if (!_textBounds.TryGetValue(text, out var bounds))
				{
					_font.MeasureText(glyphsSpan.Slice(0, i + 1), out bounds, _textPaint);
					_textBounds[text] = bounds;
				}

				var width = bounds.Width;

				if (!paddedRect.Contains(paddedRect.Left + widthSoFar + width, paddedRect.Top))
				{
					break;
				}

				// if we can fit this text plus ellipses, let's remember that
				if (paddedRect.Contains(paddedRect.Left + widthSoFar + width + _ellipsisWidth, paddedRect.Top))
				{
					lastAbbrevIndex = i;
				}

				lastDrawableIndex = i;
				widthSoFar += width;
			}

			var textPos = new SKPoint(paddedRect.Left, paddedRect.Top + _lineHeight);

			if (lastDrawableIndex == glyphs.Length - 1)
			{
				// we can draw the full string
				canvas.DrawText(eventText, textPos, _textPaint);
			}
			else if (lastAbbrevIndex > -1)
			{
				// we can draw at least part of the string
				canvas.DrawText(eventText.Substring(0, lastAbbrevIndex + 1) + "...", textPos, _textPaint);
			}
		}

		private struct ClickableItem
		{
			public int EventId;
			public SKRect Rect;
		}
	}
}
