using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeLib.Util;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Managers;
using KaraokeStudio.Project;
using KaraokeStudio.Timeline.EventRenderers;
using KaraokeStudio.Util;
using SkiaSharp;

namespace KaraokeStudio.Timeline
{
	/// <summary>
	/// The actual canvas that the timeline is drawn to.
	/// The TimelineControl presents a scrolled view into this canvas.
	/// </summary>
	internal class TimelineCanvas : IDisposable
	{
		// the width of the handles on each side of the event for grabbing, in control space
		private const float EVENT_HANDLE_WIDTH = 5.0f;
		private const float SNAP_WIDTH = 10.0f;
		internal const float PIXELS_PER_SECOND = 50.0f;
		internal const float TRACK_HEIGHT = 50.0f;

		private SKPictureRecorder _pictureRecorder;
		private SKPicture? _picture = null;
		private KaraokeProject? _project = null;
		private SKSize _size = new SKSize(1, 1);
		private bool _hasSetCursor = false;

		private SKColor _backgroundColor;
		private SKPaint _shadowPaint;
		private SKPaint _lightPaint;
		private SKPaint _textPaint;
		private SKPaint _borderPaint;
		private SKPaint _highlightPaint;
		private SKPaint _strokePaint;
		private SKPaint _selectedStrokePaint;
		private SKPaint _selectionBoxStrokePaint;

		private Dictionary<KaraokeEventType, ICustomEventRenderer> _eventRenderers = new Dictionary<KaraokeEventType, ICustomEventRenderer>()
		{
			{ KaraokeEventType.AudioClip, new AudioClipEventRenderer() },
			{ KaraokeEventType.Image, new ImageEventRenderer() }
		};

		private SKFont _font;
		private float _ellipsisWidth;
		private float _lineHeight;

		private DragState? _dragState;
		private bool _isDragging = false;
		private SKPoint? _dragStartPoint;

		private Dictionary<string, SKRect> _textBounds = [];

		// the clickable items for each track
		private ClickableItem[][] _clickableItems = [];
		private Dictionary<int, ClickableItem> _eventClickableItems = [];

		private Dictionary<KaraokeEventType, SKPaint> _eventTypePaints = [];
		private Dictionary<int, KaraokeEvent> _events = [];

		private UpdateDispatcher.Handle _projectHandle;
		private UpdateDispatcher.Handle _eventsUpdateHandle;
		private UpdateDispatcher.Handle _tracksUpdateHandle;

		public SKSize Size => _size;

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
				StrokeJoin = SKStrokeJoin.Miter,
				IsAntialias = true
			};
			_selectionBoxStrokePaint = new SKPaint()
			{
				Color = VisualStyle.NeutralLightColor.ToSKColor(),
				IsStroke = true,
				StrokeWidth = 1,
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

			_projectHandle = UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				_project = update.Project;

				foreach (var (type, renderer) in _eventRenderers)
				{
					renderer.RecreateContext();
				}

				OnProjectEventsChanged();
			});

			_eventsUpdateHandle = UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				OnProjectEventsChanged();
			});

			_tracksUpdateHandle = UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				OnProjectEventsChanged();
			});
		}

		public void Dispose()
		{
			_eventsUpdateHandle.Release();
			_tracksUpdateHandle.Release();
			_projectHandle.Release();

			foreach (var (type, renderer) in _eventRenderers)
			{
				renderer.RecreateContext();
			}

			_picture?.Dispose();
			_pictureRecorder.Dispose();
		}

		/// <summary>
		/// CopyTyped the contents of the timeline canvas onto the given destination canvas.
		/// </summary>
		/// <param name="destination">The canvas to copy onto.</param>
		/// <param name="matrix">The matrix to use to zoom and pan the canvas.</param>
		/// <param name="visibleRect">The rect in canvas space that's visible in the control.</param>
		public void CopyContents(SKCanvas destination, SKMatrix matrix, SKRect visibleRect, SKRect? selectionRect)
		{
			if (_picture != null)
			{
				destination.DrawPicture(_picture, ref matrix);
			}

			// draw the currently dragged event separately
			var rectOverrides = new Dictionary<int, SKRect>();
			if (_isDragging && _dragState != null)
			{
				destination.Save();
				destination.SetMatrix(matrix);
				foreach (var ev in _dragState.Value.Events)
				{
					rectOverrides[ev.Id] = DrawEvent(destination, _dragState.Value.TrackIndex, ev, _dragState.Value.EventTimings[ev.Id]);
				}
				destination.Restore();
			}

			var selectedEvents = SelectionManager.SelectedEvents;
			if (selectionRect != null)
			{
				// include items currently being box selected if any
				var pendingSelectedItems =
					_eventClickableItems.Values
					.Where(item => item.Rect.IntersectsWith(selectionRect.Value))
					.Select(item => _events[item.EventId]);
				selectedEvents = selectedEvents.Concat(pendingSelectedItems);
			}

			// draw borders for selected events
			foreach (var ev in selectedEvents)
			{
				var item = _eventClickableItems[ev.Id];
				var rawRect = rectOverrides.ContainsKey(ev.Id) ? rectOverrides[ev.Id] : item.Rect;
				var rect = matrix.MapRect(rawRect);

				// offset by stroke width so the stroke is inset
				var offset = _selectedStrokePaint.StrokeWidth;

				// if the rect gets too big it'll get glitchy drawing -
				// so only draw the visible part of the rect (plus a bit of padding so the border won't show)
				rect.Left = Math.Max(rect.Left + offset, destination.DeviceClipBounds.Left - 20);
				rect.Right = Math.Min(rect.Right - offset, destination.DeviceClipBounds.Right + 20);
				rect.Top += offset;
				rect.Bottom -= offset;

				destination.Save();
				destination.DrawRect(rect, _selectedStrokePaint);
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
					var rect = rectOverrides.ContainsKey(item.EventId) ? rectOverrides[item.EventId] : item.Rect;
					// this rect is visible!
					if (visibleRect.IntersectsWith(rect) && _events.TryGetValue(item.EventId, out var ev))
					{
						TryDrawEventText(destination, ev, matrix.MapRect(rect));
					}
				}
			}

			if (selectionRect != null)
			{
				destination.Save();
				destination.DrawRect(matrix.MapRect(selectionRect.Value), _selectionBoxStrokePaint);
				destination.Restore();
			}
		}

		/// <summary>
		/// Selects the KaraokeEvent at the given point in canvas space, if present.
		/// </summary>
		/// <returns>True if an event was selected, false if not.</returns>
		public bool SelectEventAtPoint(SKPoint point)
		{
			var ev = FindClickableItemAtPoint(point);
			var isShiftDown = System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Shift);
			if (ev != null)
			{
				SelectionManager.Select(_events[ev.Value.EventId], !isShiftDown);
			}
			return ev != null;
		}

		public bool SelectEventsInRect(SKRect rect)
		{
			if (!System.Windows.Forms.Control.ModifierKeys.HasFlag(Keys.Shift))
			{
				SelectionManager.Deselect();
			}

			var anySelected = false;

			foreach (var (id, item) in _eventClickableItems)
			{
				if (item.Rect.IntersectsWith(rect))
				{
					anySelected = true;
					SelectionManager.Select(_events[id], false);
				}
			}

			return anySelected;
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

		/// <summary>
		/// Starts dragging an event, if possible.
		/// </summary>
		public bool MaybeStartDragging(SKPoint point)
		{
			if (_dragState == null)
			{
				return false;
			}

			_dragStartPoint = point;
			_isDragging = true;
			// event will be drawn overlaid on the already drawn canvas while dragging
			RecreateCanvas(_size);
			return true;
		}

		public void EndDrag()
		{
			if (_isDragging && _dragState != null)
			{
				var eventIds = new HashSet<int>(_dragState.Value.Events.Select(ev => ev.Id));

				var eventsPerTrack = new Dictionary<int, int[]>();
				var tracks = _project?.Tracks.Where(t => t.Events.Any(ev => eventIds.Contains(ev.Id))) ?? Enumerable.Empty<KaraokeTrack>();
				CommandDispatcher.Dispatch(new SetEventTimingsCommand(tracks.ToArray(), _dragState.Value.EventTimings, eventIds.Count > 1 ? "Drag events" : "Drag event"));
			}
			_isDragging = false;
			_dragState = null;
			// redraw canvas with dragged element
			RecreateCanvas(_size);
		}

		public KaraokeEvent? FindEventAtPoint(SKPoint point)
		{
			var clickableItem = FindClickableItemAtPoint(point);
			if (clickableItem != null)
			{
				return _events[clickableItem.Value.EventId];
			}

			return null;
		}

		/// <summary>
		/// Updates the timeline canvas based on the position of the mouse.
		/// </summary>fon
		/// <param name="matrix">The matrix to convert from canvas space to control space.</param>
		/// <param name="mousePos">The position of the mouse in control space.</param>
		/// <param name="isIdle">If true, the timeline's UI state is idle.</param>
		public void UpdateMousePos(SKMatrix matrix, SKPoint mousePos, bool isIdle)
		{
			var mousePosCanvas = matrix.Invert().MapPoint(mousePos);

			if (_isDragging)
			{
				UpdateDrag(matrix, mousePosCanvas.X / PIXELS_PER_SECOND);
				return;
			}

			_dragState = null;

			if (_hasSetCursor)
			{
				Cursor.Current = Cursors.Default;
				_hasSetCursor = false;
			}

			if (!isIdle)
			{
				return;
			}

			var item = FindClickableItemAtPoint(mousePosCanvas);
			if (item == null)
			{
				return;
			}

			var ev = _events[item.Value.EventId];
			var evRect = matrix.MapRect(item.Value.Rect);

			var isTouchingStart = Math.Abs(evRect.Left - mousePos.X) < EVENT_HANDLE_WIDTH;
			var isTouchingEnd = Math.Abs(evRect.Right - mousePos.X) < EVENT_HANDLE_WIDTH;

			DragType dragType;

			if (isTouchingStart)
			{
				Cursor.Current = Cursors.VSplit;
				dragType = DragType.MoveStart;
			}
			else if (isTouchingEnd)
			{
				Cursor.Current = Cursors.VSplit;
				dragType = DragType.MoveEnd;
			}
			else
			{
				Cursor.Current = Cursors.SizeAll;
				dragType = DragType.MoveEvent;
			}

			var events = new List<KaraokeEvent>() { ev };
			if (dragType == DragType.MoveEvent && SelectionManager.SelectedEvents.Count() > 1)
			{
				events.AddRange(SelectionManager.SelectedEvents.Where(e => e.Id != ev.Id));
			}

			_hasSetCursor = true;
			_dragState = new DragState(dragType, item.Value.TrackIndex, events.ToArray());
		}

		internal void OnProjectEventsChanged()
		{
			_events.Clear();
			_textBounds.Clear();
			_eventClickableItems.Clear();
			_dragState = null;
			_isDragging = false;
			RecreateCanvas(CalculateSize());
		}

		private void UpdateDrag(SKMatrix matrix, double time)
		{
			if (_dragState == null || _project == null)
			{
				return;
			}

			var ids = _dragState.Value.Events.Select(ev => ev.Id).ToHashSet();
			var activeTrack = _project.Tracks.Where(t => t.Events.Any(ev => ids.Contains(ev.Id))).FirstOrDefault();
			if (activeTrack == null)
			{
				return;
			}

			var origStart = _dragState.Value.Events.Min(ev => ev.StartTimeSeconds);
			var origEnd = _dragState.Value.Events.Max(ev => ev.EndTimeSeconds);
			var firstEventId = _dragState.Value.Events.First().Id;

			var precedingEvents = activeTrack.Events.Where(ev => !ids.Contains(ev.Id) && ev.EndTimeSeconds <= origStart);
			var precedingEventEnd = precedingEvents.Any() ? precedingEvents.Max(ev => ev.EndTimeSeconds) : 0;
			var succeedingEvents = activeTrack.Events.Where(ev => !ids.Contains(ev.Id) && ev.StartTimeSeconds >= origEnd);
			var succeedingEventStart = succeedingEvents.Any() ? succeedingEvents.Min(ev => ev.StartTimeSeconds) : double.MaxValue;
			var minEventSize = 0.05;

			switch (_dragState.Value.Type)
			{
				case DragType.MoveStart:
					{
						var dragPos = GetDragPos(matrix, time, precedingEventEnd, origEnd - minEventSize, new double[] {
							precedingEventEnd,
							origEnd - minEventSize,
							origStart,
							_project.PlaybackState.Position });
						_dragState.Value.EventTimings[firstEventId] = (dragPos, _dragState.Value.Events[0].EndTimeSeconds);
					}
					break;
				case DragType.MoveEnd:
					{
						var dragPos = GetDragPos(matrix, time, origStart + minEventSize, succeedingEventStart, new double[] {
							origStart + minEventSize,
							succeedingEventStart,
							origEnd,
							_project.PlaybackState.Position });
						_dragState.Value.EventTimings[firstEventId] = (_dragState.Value.Events[0].StartTimeSeconds, dragPos);
					}
					break;
				case DragType.MoveEvent:
					{
						// time is relative to where we started to drag from
						var len = _dragState.Value.Events.Max(ev => ev.EndTimeSeconds) - _dragState.Value.Events.Min(ev => ev.StartTimeSeconds);
						var timeOffset = time - (_dragStartPoint?.X ?? 0) / PIXELS_PER_SECOND;
						var curTime = origStart + timeOffset;
						var dragPos = GetDragPos(matrix, curTime, precedingEventEnd, succeedingEventStart - len, new double[] {
							precedingEventEnd,
							succeedingEventStart - len,
							_project.PlaybackState.Position,
							_project.PlaybackState.Position - len
						});

						var start = _dragState.Value.Events.Min(ev => ev.StartTimeSeconds);

						foreach (var ev in _dragState.Value.Events)
						{
							var offset = ev.StartTimeSeconds - start;
							_dragState.Value.EventTimings[ev.Id] = (dragPos + offset, dragPos + offset + ev.LengthSeconds);
						}
					}
					break;
			}
		}

		private double GetDragPos(SKMatrix matrix, double curTime, double minTime, double maxTime, double[] snapPoints)
		{
			if (curTime < minTime)
			{
				return minTime;
			}
			else if (curTime > maxTime)
			{
				return maxTime;
			}

			var curX = matrix.MapPoint(new SKPoint((float)curTime * PIXELS_PER_SECOND, 0)).X;
			var snapX = snapPoints.Select(s => (s, matrix.MapPoint(new SKPoint((float)s * PIXELS_PER_SECOND, 0)).X)).ToArray();

			foreach (var (snapTime, snapPos) in snapX)
			{
				if (Math.Abs(curX - snapPos) < SNAP_WIDTH / 2)
				{
					return Math.Clamp(snapTime, minTime, maxTime);
				}
			}

			return curTime;
		}

		/// <summary>
		/// Returns an event on the timeline at the given (X, Y) position, if any.
		/// </summary>
		private ClickableItem? FindClickableItemAtPoint(SKPoint point)
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

		private SKRect DrawEvent(SKCanvas canvas, int trackIndex, KaraokeEvent ev, (double Start, double End) timing)
		{
			var trackYPos = trackIndex * TRACK_HEIGHT;

			var eventRect = new SKRect(
				PIXELS_PER_SECOND * (float)timing.Start,
				trackYPos,
				PIXELS_PER_SECOND * (float)timing.End,
				trackYPos + TRACK_HEIGHT - 2.0f
			);

			if (!_eventTypePaints.TryGetValue(ev.Type, out var paint))
			{
				paint = _highlightPaint;
			}

			canvas.DrawRect(eventRect, paint);

			if (_eventRenderers.ContainsKey(ev.Type))
			{
				_eventRenderers[ev.Type].Render(canvas, eventRect, ev);
			}

			canvas.DrawRect(new SKRect(eventRect.Left, eventRect.Top + eventRect.Height - 1, eventRect.Right, eventRect.Bottom), _shadowPaint);
			canvas.DrawRect(eventRect, _strokePaint);
			return eventRect;
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

			var projectTracks = _project.Tracks.OrderBy(t => t.Order).ToArray();

			if (_clickableItems.GetLength(0) != projectTracks.Length)
			{
				_clickableItems = new ClickableItem[projectTracks.Length][];
			}

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
					if (_isDragging && _dragState != null && _dragState.Value.Events.Any(e => e.Id == ev.Id))
					{
						// skip event being dragged - it's rendered over the canvas
						continue;
					}
					_events[ev.Id] = ev;

					var eventRect = DrawEvent(canvas, i, ev, (ev.StartTimeSeconds, ev.EndTimeSeconds));

					items[j] = new ClickableItem()
					{
						Rect = eventRect,
						EventId = ev.Id,
						TrackIndex = i
					};

					_eventClickableItems[ev.Id] = items[j];
				}

				// draw border
				var borderY = (i + 1) * TRACK_HEIGHT;
				canvas.DrawLine(new SKPoint(0, borderY), new SKPoint(_size.Width, borderY), _borderPaint);
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

			var padding = 5.0f;
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

		private struct DragState
		{
			/// <summary>
			/// The events we're currently dragging.
			/// </summary>
			public KaraokeEvent[] Events;

			public Dictionary<int, (double Start, double End)> EventTimings;

			public DragType Type;
			public int TrackIndex;

			public DragState(DragType type, int trackIndex, KaraokeEvent[] events)
			{
				Type = type;
				TrackIndex = trackIndex;
				Events = events;
				EventTimings = new Dictionary<int, (double Start, double End)>();
				foreach (var ev in events)
				{
					EventTimings[ev.Id] = (ev.StartTimeSeconds, ev.EndTimeSeconds);
				}
			}
		}

		private enum DragType
		{
			MoveStart,
			MoveEnd,
			MoveEvent
		}

		private struct ClickableItem
		{
			public int EventId;
			public SKRect Rect;
			public int TrackIndex;
		}
	}
}
