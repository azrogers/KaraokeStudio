using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;
using KaraokeStudio.Util;
using NLog;
using SkiaSharp;
using System.Data;

namespace KaraokeStudio.Timeline
{
    /// <summary>
    /// Represents the timeline that displays events and allows the user to modify them.
    /// </summary>
    /// <remarks>
    /// The control contains a canvas. The canvas is larger than the client rect of the control - it's as long as is needed to display every event.
    /// We use the concept of "canvas space" and "control space" to distinguish between these. 
    /// A coordinate in control space is in screen coordinates, relative to the location of the control on the screen.
    /// A coordinate in canvas space is in the coordinates of the full canvas, which may be scaled or scrolled depending on the user's inputs.
    /// </remarks>
    public partial class TimelineControl : UserControl, IDisposable
	{
		private const float PADDING_TOP = 5.0f;
		// width of the playhead for clicking and dragging to scrub
		private const float SCRUB_AREA = 15.0f;

		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private SKColor _backgroundColor;
		private SKPaint _lightPaint;
		private SKPaint _highlightPaint;

		private TimelineCanvas _timelineCanvas;
		private KaraokeProject? _currentProject;
		private MouseEventArgs? _lastMouseDownEventArgs;
		private MouseEventArgs? _lastMouseMoveEventArgs;
		private KaraokeTrack[] _tracks;
		private float _horizZoomFactor = 1.0f;
		private float _verticalZoomFactor = 1.0f;
		private Point _selectionStartPoint = Point.Empty;
		private TimelineUIState _uiState = TimelineUIState.Idle;
		private System.Windows.Forms.Timer _mouseTimer;
		private bool _awaitingMouseTimer = false;
		private float _prevPlaybackHeadXPos = 0;

		private UpdateDispatcher.Handle _projectHandle;
		private UpdateDispatcher.Handle _eventsUpdateHandle;
		private UpdateDispatcher.Handle _tracksUpdateHandle;

		/// <summary>
		/// Called when the positioning of tracks has changed.
		/// </summary>
		public event Action? OnTrackPositioningChanged;

		public TimelineControl()
		{
			InitializeComponent();

			Disposed += OnDispose;

			_mouseTimer = new System.Windows.Forms.Timer();
			_mouseTimer.Interval = 100;
			_mouseTimer.Tick += _mouseTimer_Tick;

			_tracks = new KaraokeTrack[0];

			_timelineCanvas = new TimelineCanvas();

			_backgroundColor = VisualStyle.NeutralDarkColor.ToSKColor();

			_lightPaint = new SKPaint() { Color = VisualStyle.NeutralLightColor.ToSKColor() };
			_highlightPaint = new SKPaint()
			{
				Color = VisualStyle.HighlightColor.ToSKColor(),
				IsAntialias = true
			};

			SelectionManager.OnSelectedEventsChanged += OnSelectedEventsChanged;

			horizScroll.Enabled = false;
			verticalScroll.Enabled = false;

			_eventsUpdateHandle = UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				RecalculateScrollBars();
				skiaControl.Invalidate();
			});

			_tracksUpdateHandle = UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				_tracks = _currentProject?.Tracks.OrderBy(t => t.Id).ToArray() ?? new KaraokeTrack[0];
				RecalculateScrollBars();
				skiaControl.Invalidate();
			});

			_projectHandle = UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				if (_currentProject != null)
				{
					// cleanup old event listeners
					_currentProject.PlaybackState.OnPositionChanged -= OnPositionChanged;
				}

				if (update.Project != null)
				{
					update.Project.PlaybackState.OnPositionChanged += OnPositionChanged;
				}

				_tracks = update.Project?.Tracks.OrderBy(t => t.Id).ToArray() ?? new KaraokeTrack[0];
				_currentProject = update.Project;

				RecalculateScrollBars();
				skiaControl.Invalidate();
			});
		}

		private void OnDispose(object? sender, EventArgs e)
		{
			_eventsUpdateHandle.Release();
			_tracksUpdateHandle.Release();
			_projectHandle.Release();

			_mouseTimer.Dispose();
			_timelineCanvas.Dispose();

			if (_currentProject != null)
			{
				_currentProject.PlaybackState.OnPositionChanged -= OnPositionChanged;
			}

			SelectionManager.OnSelectedEventsChanged -= OnSelectedEventsChanged;
		}

		public RectangleF GetTrackRect(int trackId)
		{
			var trackIndex = -1;
			for (var i = 0; i < _tracks.Length; i++)
			{
				if (_tracks[i].Id == trackId)
				{
					trackIndex = i;
					break;
				}
			}

			if (trackIndex == -1)
			{
				Logger.Warn($"Track rect requested for ID {trackId} but no such track found?");
				return RectangleF.Empty;
			}

			var skRect = CreateMatrix().MapRect(new SKRect(
				0,
				trackIndex * TimelineCanvas.TRACK_HEIGHT,
				_timelineCanvas.Size.Width,
				(trackIndex + 1) * TimelineCanvas.TRACK_HEIGHT - 2));
			return new RectangleF(skRect.Left, skRect.Top, skRect.Width, skRect.Height);
		}

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			_tracks = project?.Tracks.OrderBy(t => t.Id).ToArray() ?? new KaraokeTrack[0];
			_timelineCanvas.OnProjectEventsChanged();
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void OnSelectedEventsChanged()
		{
			skiaControl.Invalidate();
		}

		private void OnPositionChanged(double pos)
		{
			var viewportRect = GetViewportClientRect();
			// playback head has moved off screen, let's scroll to catch up with it
			if(viewportRect.Contains(new Point((int)_prevPlaybackHeadXPos, 10)) && !viewportRect.Contains(new Point((int)GetPlaybackHeadXPos(), 10)))
			{
				var scrollOffset =
					SKMatrix
					.CreateScale(_horizZoomFactor, _verticalZoomFactor)
					.MapPoint(new SKPoint((float)_timelineCanvas.GetXPosOfTime(_currentProject?.PlaybackState.Position ?? 0), 0)).X;

				horizScroll.Value = Math.Clamp((int)scrollOffset, horizScroll.Minimum, horizScroll.Maximum);
			}

			skiaControl.Invalidate();
		}

		private bool SelectEventAtPosition(Point pos)
		{
			var result = _timelineCanvas.SelectEventAtPoint(TranslatePointToCanvas(pos));
			return result;
		}

		/// <summary>
		/// Returns the XY position in canvas space of the (0, 0) coordinate in control space.
		/// </summary>
		private SKPoint GetCanvasOffset()
		{
			return new SKPoint(
				-(horizScroll.Enabled ? horizScroll.Value : 0.0f),
				-(verticalScroll.Enabled ? verticalScroll.Value : 0.0f) + PADDING_TOP
			);
		}

		/// <summary>
		/// Creates a matrix to transform from canvas space to control space.
		/// </summary>
		private SKMatrix CreateMatrix()
		{
			var translation = GetCanvasOffset();
			return SKMatrix.CreateScaleTranslation(_horizZoomFactor, _verticalZoomFactor, translation.X, translation.Y);
		}

		/// <summary>
		/// Creates a matrix to transform from control space to canvas space.
		/// </summary>
		private SKMatrix CreateInverseMatrix() => CreateMatrix().Invert();

		private float GetPlaybackHeadXPos()
		{
			var playbackPos = _currentProject?.PlaybackState.Position ?? 0;
			return CreateMatrix().MapPoint(new SKPoint((float)(_timelineCanvas.GetXPosOfTime(playbackPos)), 0)).X;
		}

		private SKPoint TranslatePointToCanvas(Point pos)
		{
			return CreateInverseMatrix().MapPoint(new SKPoint(pos.X, pos.Y));
		}

		private void RecalculateScrollBars()
		{
			if (_currentProject == null)
			{
				verticalScroll.Enabled = false;
				horizScroll.Enabled = false;
				return;
			}

			var clientRect = GetViewportClientRect();
			var rect = CreateMatrix().MapRect(new SKRect(0, 0, _timelineCanvas.Size.Width, _timelineCanvas.Size.Height + 5));
			var size = new SKSize(rect.Width, rect.Height + PADDING_TOP);

			verticalScroll.Enabled = size.Height > clientRect.Height;
			horizScroll.Enabled = size.Width > clientRect.Width;

			verticalScroll.Minimum = 0;
			verticalScroll.Maximum = (int)Math.Ceiling(size.Height - clientRect.Height);
			horizScroll.Minimum = 0;
			horizScroll.Maximum = (int)Math.Ceiling(size.Width - clientRect.Width);
		}

		private void PaintSurface(SKCanvas canvas)
		{
			canvas.Clear(_backgroundColor);

			var savePoint = canvas.Save();
			var matrix = CreateMatrix();

			var clientRect = GetViewportClientRect();
			SKRect? selectionRect =
				_uiState == TimelineUIState.Multiselect ?
				CreateInverseMatrix().MapRect(GetCurrentSelectionRect(_lastMouseMoveEventArgs?.Location ?? _lastMouseDownEventArgs?.Location ?? Point.Empty)) :
				null;
			var visibleRect = CreateInverseMatrix().MapRect(new SKRect(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom));
			_timelineCanvas.CopyContents(canvas, matrix, visibleRect, selectionRect);

			canvas.RestoreToCount(savePoint);

			var playheadXPos = GetPlaybackHeadXPos();
			_prevPlaybackHeadXPos = playheadXPos;

			savePoint = canvas.Save();
			canvas.DrawLine(new SKPoint(playheadXPos, 0), new SKPoint(playheadXPos, clientRect.Height), _lightPaint);

			var tri = new SKPath();
			tri.AddPoly(new SKPoint[]
			{
				new SKPoint(playheadXPos + -5f, 0.0f),
				new SKPoint(playheadXPos + 5f, 0.0f),
				new SKPoint(playheadXPos + 0.0f, 10f)
			});

			canvas.DrawPath(tri, _highlightPaint);
			canvas.RestoreToCount(savePoint);
		}

		private void UpdateZoom(float horiz, float vertical)
		{
			if (_currentProject == null)
			{
				return;
			}

			var posToRestore = 0.0;
			var playbackXPos = GetPlaybackHeadXPos();
			// if the playback head is on screen, use that as the center of our zoom
			if (playbackXPos >= 0 && playbackXPos < ClientSize.Width)
			{
				posToRestore = _currentProject.PlaybackState.Position;
			}
			// otherwise, use the time at the center of the widget
			else
			{
				var centerPos = CreateInverseMatrix().MapPoint(new SKPoint(ClientSize.Width / 2.0f, 0));
				var centerTime = _timelineCanvas.GetTimeOfPoint(centerPos);
				posToRestore = centerTime;
			}

			_horizZoomFactor = horiz;
			_verticalZoomFactor = vertical;
			RecalculateScrollBars();
			skiaControl.Invalidate();

			var scrollOffset =
				SKMatrix
				.CreateScale(_horizZoomFactor, _verticalZoomFactor)
				.MapPoint(new SKPoint((float)_timelineCanvas.GetXPosOfTime(posToRestore), 0)).X;

			horizScroll.Value = Math.Clamp((int)(scrollOffset - ClientSize.Width / 2), horizScroll.Minimum, horizScroll.Maximum);
			OnTrackPositioningChanged?.Invoke();
		}

		private RectangleF GetViewportClientRect()
		{
			return new RectangleF(
				0,
				PADDING_TOP,
				ClientSize.Width - 20,
				ClientSize.Height - PADDING_TOP - 20
			);
		}

		private void SeekToLocation(Point point)
		{
			if (_currentProject != null)
			{
				var pos = _timelineCanvas.GetTimeOfPoint(TranslatePointToCanvas(point));
				_currentProject.PlaybackState.SeekAbsolute(pos);
			}
		}

		private SKRect GetCurrentSelectionRect(Point mousePos)
		{
			return new SKRect(
				Math.Min(_selectionStartPoint.X, mousePos.X),
				Math.Min(_selectionStartPoint.Y, mousePos.Y),
				Math.Max(_selectionStartPoint.X, mousePos.X),
				Math.Max(_selectionStartPoint.Y, mousePos.Y));
		}

		private void HandleClick(Point location)
		{
			if (!SelectEventAtPosition(location))
			{
				SelectionManager.Deselect();
				SeekToLocation(location);
			}
		}

		#region UI Events
		private void skiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			PaintSurface(e.Surface.Canvas);
		}

		private void skiaControl_Layout(object sender, LayoutEventArgs e)
		{
			RecalculateScrollBars();
		}

		private void horizPlusButton_Click(object sender, EventArgs e)
		{
			UpdateZoom(_horizZoomFactor * 2.0f, _verticalZoomFactor);
		}

		private void horizMinusButton_Click(object sender, EventArgs e)
		{
			UpdateZoom(Math.Max(_horizZoomFactor * 0.5f, 0.01f), _verticalZoomFactor);
		}

		private void verticalPlusButton_Click(object sender, EventArgs e)
		{
			UpdateZoom(_horizZoomFactor, _verticalZoomFactor * 2.0f);
		}

		private void verticalMinusButton_Click(object sender, EventArgs e)
		{
			UpdateZoom(_horizZoomFactor, Math.Max(_verticalZoomFactor * 0.5f, 0.01f));
		}

		private void verticalScroll_Scroll(object sender, ScrollEventArgs e)
		{
			skiaControl.Invalidate();
			OnTrackPositioningChanged?.Invoke();
		}

		private void horizScroll_Scroll(object sender, ScrollEventArgs e)
		{
			skiaControl.Invalidate();
			OnTrackPositioningChanged?.Invoke();
		}

		private void skiaControl_MouseDown(object sender, MouseEventArgs e)
		{
			_uiState = TimelineUIState.Pending;
			_lastMouseDownEventArgs = e;
			_mouseTimer.Stop();

			var playheadXPos = GetPlaybackHeadXPos();
			var playheadRect = new SKRect(playheadXPos - SCRUB_AREA / 2.0f, 0, playheadXPos + SCRUB_AREA / 2.0f, ClientSize.Height);
			if (playheadRect.Contains(new SKPoint(e.X, e.Y)))
			{
				_uiState = TimelineUIState.Scrubbing;
			}
			else
			{
				_selectionStartPoint = e.Location;
				_awaitingMouseTimer = true;
				_mouseTimer.Start();
			}

			skiaControl.Invalidate();
		}

		private void skiaControl_MouseUp(object sender, MouseEventArgs e)
		{
			if (_uiState == TimelineUIState.Pending)
			{
				var dist = new SKPoint(e.X - _selectionStartPoint.X, e.Y - _selectionStartPoint.Y).Length;
				if (dist < 1.5)
				{
					HandleClick(e.Location);
				}
			}
			else if(_uiState == TimelineUIState.Multiselect)
			{
				_timelineCanvas.SelectEventsInRect(CreateInverseMatrix().MapRect(GetCurrentSelectionRect(e.Location)));
			}
			else if(_uiState == TimelineUIState.Dragging)
			{
				_timelineCanvas.EndDrag();
			}

			_uiState = TimelineUIState.Idle;
			_awaitingMouseTimer = false;
			skiaControl.Invalidate();
		}

		private void skiaControl_MouseMove(object sender, MouseEventArgs e)
		{
			_lastMouseMoveEventArgs = e;
			if (_uiState == TimelineUIState.Pending)
			{
				var dist = new SKPoint(e.X - _selectionStartPoint.X, e.Y - _selectionStartPoint.Y).Length;
				if (dist > 1.5)
				{
					_uiState =
						_timelineCanvas.MaybeStartDragging(TranslatePointToCanvas(e.Location)) ?
						TimelineUIState.Dragging :
						TimelineUIState.Multiselect;
				}
			}

			_awaitingMouseTimer = false;

			_timelineCanvas.UpdateMousePos(
				CreateMatrix(),
				new SKPoint(e.X, e.Y),
				_uiState == TimelineUIState.Idle || _uiState == TimelineUIState.Pending);

			if (_uiState == TimelineUIState.Scrubbing)
			{
				SeekToLocation(e.Location);
			}
			else if (_uiState == TimelineUIState.Multiselect || _uiState == TimelineUIState.Dragging)
			{
				skiaControl.Invalidate();
			}
		}

		private void _mouseTimer_Tick(object? sender, EventArgs e)
		{
			_mouseTimer.Stop();

			// moused up too quickly, treat it as just a click on nothing
			if (_awaitingMouseTimer && _uiState == TimelineUIState.Idle)
			{
				HandleClick(_lastMouseDownEventArgs?.Location ?? Point.Empty);
			}
		}
		#endregion

		private enum TimelineUIState
		{
			// nothing special is happening
			Idle,
			// we don't know what we're doing yet
			Pending,
			// we're currently box selecting
			Multiselect,
			// we're currently scrubbing the timeline
			Scrubbing,
			// we're currently dragging an event
			Dragging
		}
	}
}
