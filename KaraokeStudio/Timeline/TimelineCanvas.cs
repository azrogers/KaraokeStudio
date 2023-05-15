using KaraokeLib.Lyrics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;

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
		private float _dotGlyphWidth;
		private Dictionary<ushort, SKRect> _glyphBounds = new Dictionary<ushort, SKRect>();

		private int _selectedEventId = -1;

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

			_font = new SKFont(SKTypeface.FromFamilyName("Arial"), 6.0f);
			_font.MeasureText(new ushort[] { _font.GetGlyph('.') }, out var bounds);
			_dotGlyphWidth = bounds.Width;
		}

		public void CopyContents(SKCanvas destination, SKMatrix matrix)
		{
			if (_picture != null)
			{
				destination.DrawPicture(_picture, ref matrix);
			}
		}

		public void SelectEventAtPoint(SKPoint point)
		{
			var ev = FindEventAtPoint(point);
			_selectedEventId = ev?.Id ?? -1;
			// TODO: can we draw the selected event highlight over the top so we can avoid redrawing the whole canvas?
			RedrawCanvas();
		}
		
		/// <summary>
		/// Returns an event on the timeline at the given (X, Y) position, if any.
		/// </summary>
		public LyricsEvent? FindEventAtPoint(SKPoint point)
		{
			if(_project == null)
			{
				return null;
			}

			var track = (int)Math.Floor(point.Y / TrackHeight);
			if(track < 0 || track >= _clickableItems.GetLength(0))
			{
				return null;
			}

			foreach(var item in _clickableItems[track])
			{
				if(point.X >= item.StartX && point.X < item.EndX && _events.TryGetValue(item.EventId, out var ev))
				{
					return ev;
				}
			}

			return null;
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_project = project;
			_events.Clear();
			_selectedEventId = -1;

			var newSize = CalculateSize();
			if (newSize != _size)
			{
				// recreate the canvas if the size has changed
				RecreateCanvas(newSize);
			}
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
			for(var i = 0; i < projectTracks.Length; i++)
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
					canvas.DrawRect(eventRect, ev.Id == _selectedEventId ? _selectedStrokePaint : _strokePaint);

					// TODO: text size should remain the same regardless of zoom level and text should get more visible as we zoom
					TryDrawEventText(canvas, ev, eventRect);

					items[j] = new ClickableItem()
					{
						StartX = eventRect.Left,
						EndX = eventRect.Right,
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
			// draw text if we can fit it
			if (string.IsNullOrWhiteSpace(ev.Text))
			{
				return;
			}

			var padding = 3.0f;
			var textPos = new SKPoint(rect.Left + padding, rect.Top + padding);
			var maxWidth = rect.Width - padding * 2;

			var glyphs = ev.Text.ToCharArray().Select(c => _font.GetGlyph(c)).ToArray();
			// temporary arr for optimization
			var arr = new ushort[1];
			var widthSoFar = 0.0f;
			var lastDrawableIndex = -1;
			var lastAbbrevIndex = -1;
			var lineHeight = 0.0f;
			for (var i = 0; i < glyphs.Length; i++)
			{
				if (!_glyphBounds.TryGetValue(glyphs[i], out var bounds))
				{
					arr[0] = glyphs[i];

					_font.MeasureText(arr, out bounds);
					_glyphBounds[glyphs[i]] = bounds;
				}

				var width = bounds.Width;
				lineHeight = Math.Max(bounds.Height, lineHeight);

				if ((widthSoFar + width) > maxWidth)
				{
					break;
				}

				// if we can fit this text plus ellipses, let's remember that
				if ((widthSoFar + width + _dotGlyphWidth * 3) <= maxWidth)
				{
					lastAbbrevIndex = i;
				}

				lastDrawableIndex = i;
				widthSoFar += width;
			}

			textPos.Y += lineHeight;

			if (lastDrawableIndex == glyphs.Length - 1)
			{
				// we can draw the full string
				canvas.DrawText(ev.Text, textPos, _textPaint);
			}
			else if (lastAbbrevIndex > -1)
			{
				// we can draw at least part of the string
				canvas.DrawText(ev.Text.Substring(0, lastAbbrevIndex + 1) + "...", textPos, _textPaint);
			}
		}

		private struct ClickableItem
		{
			public int EventId;
			public float StartX;
			public float EndX;
		}
	}
}
