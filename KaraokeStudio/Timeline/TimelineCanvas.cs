using KaraokeLib.Events;
using KaraokeLib.Util;
using KaraokeLib.Video;
using KaraokeStudio.Util;
using SkiaSharp;

namespace KaraokeStudio.Timeline
{
    /// <summary>
    /// The actual canvas that the timeline is drawn to.
    /// The TimelineControl presents a scrolled view into this canvas.
    /// </summary>
    internal class TimelineCanvas
	{
		private const float PIXELS_PER_SECOND = 50.0f;
		internal const float TRACK_HEIGHT = 50.0f;

		private SKPictureRecorder _pictureRecorder;
		private SKPicture? _picture = null;
		private KaraokeProject? _project = null;
		private SKSize _size = new SKSize(1, 1);

		private SKColor _backgroundColor;
		private SKPaint _shadowPaint;
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

		private Dictionary<KaraokeEventType, SKPaint> _eventTypePaints = new Dictionary<KaraokeEventType, SKPaint>();
		private Dictionary<int, KaraokeEvent> _events = new Dictionary<int, KaraokeEvent>();

		public SKSize Size => _size;

		/// <summary>
		/// The KaraokeEvent currently selected, if any.
		/// </summary>
		public KaraokeEvent? SelectedEvent => _selectedEvent == null ? null : _events[_selectedEvent.Value.EventId];

		/// <summary>
		/// Called when the currently selected event has changed.
		/// </summary>
		public event Action<KaraokeEvent?>? OnEventSelectionChanged;

		public TimelineCanvas()
		{
			_pictureRecorder = new SKPictureRecorder();

			_backgroundColor = VisualStyle.NeutralDarkColor.ToSKColor();

			_shadowPaint = new SKPaint() { Color = new SKColor(0, 0, 0, 127) };
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
		/// CopyTyped the contents of the timeline canvas onto the given destination canvas.
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

		/// <summary>
		/// Selects the KaraokeEvent at the given point in canvas space, if present.
		/// </summary>
		/// <returns>True if an event was selected, false if not.</returns>
		public bool SelectEventAtPoint(SKPoint point)
		{
			var ev = FindEventAtPoint(point);
			_selectedEvent = ev;
			OnEventSelectionChanged?.Invoke(_selectedEvent == null ? null : _events[_selectedEvent.Value.EventId]);
			return ev != null;
		}

		public void Deselect()
		{
			_selectedEvent = null;
			OnEventSelectionChanged?.Invoke(null);
		}

		/// <summary>
		/// Returns the position in seconds of the given point in canvas space.
		/// </summary>
		public double GetTimeOfPoint(SKPoint point)
		{
			return Math.Clamp(point.X / PIXELS_PER_SECOND, 0, _project?.Length.TotalSeconds ?? 0);
		}

		/// <summary>
		/// Returns the X position in canvas space of the given time in seconds.
		/// </summary>
		public double GetXPosOfTime(double time)
		{
			return time * PIXELS_PER_SECOND;
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_project = project;
			_events.Clear();
			_textBounds.Clear();
			_selectedEvent = null;

			RecreateCanvas(CalculateSize());
		}

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			var oldSelectedEvent = _selectedEvent;
			OnProjectChanged(project);
			if(oldSelectedEvent != null && _events.ContainsKey(oldSelectedEvent.Value.EventId))
			{
				_selectedEvent = FindItemForEventId(oldSelectedEvent.Value.EventId);
				OnEventSelectionChanged?.Invoke(_selectedEvent == null ? null : _events[_selectedEvent.Value.EventId]);
			}
		}

		private ClickableItem? FindItemForEventId(int eventId)
		{
			for(var i = 0; i < _clickableItems.Length; i++)
			{
				for(var j = 0; j < _clickableItems[i].Length; j++)
				{
					if (_clickableItems[i][j].EventId == eventId)
					{
						return _clickableItems[i][j];
					}
				}
			}

			return null;
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

			var track = (int)Math.Floor(point.Y / TRACK_HEIGHT);
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

			var width = (float)(PIXELS_PER_SECOND * _project.Length.TotalSeconds);
			var height = _project.Tracks.Count() * TRACK_HEIGHT;

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

			var projectTracks = _project.Tracks.OrderBy(t => t.Id).ToArray();

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
						PIXELS_PER_SECOND * (float)ev.StartTimeSeconds,
						trackYPos,
						PIXELS_PER_SECOND * (float)ev.EndTimeSeconds,
						trackYPos + TRACK_HEIGHT - 2.0f
					);

					if (!_eventTypePaints.TryGetValue(ev.Type, out var paint))
					{
						paint = _highlightPaint;
					}

					canvas.DrawRect(eventRect, paint);
					canvas.DrawRect(new SKRect(eventRect.Left, eventRect.Top + eventRect.Height - 1, eventRect.Right, eventRect.Bottom), _shadowPaint);
					canvas.DrawRect(eventRect, _strokePaint);

					items[j] = new ClickableItem()
					{
						Rect = eventRect,
						EventId = ev.Id
					};
				}

				// draw border
				var borderY = trackYPos + TRACK_HEIGHT;
				canvas.DrawLine(new SKPoint(0, borderY), new SKPoint(_size.Width, borderY), _borderPaint);
				trackYPos += TRACK_HEIGHT;
			}

			_picture = _pictureRecorder.EndRecording();
		}

		private void TryDrawEventText(SKCanvas canvas, KaraokeEvent ev, SKRect rect)
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
