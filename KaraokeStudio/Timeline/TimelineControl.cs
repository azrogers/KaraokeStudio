using KaraokeLib.Lyrics;
using KaraokeLib.Util;
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
	public partial class TimelineControl : UserControl
	{
		private const float PADDING_TOP = 5.0f;
		private const float TRACK_HEADER_WIDTH = 200.0f;

		private SKColor _backgroundColor;
		private SKPaint _backgroundPaint;
		private SKPaint _shadowPaint;
		private SKPaint _selectTrackPaint;
		private SKPaint _lightPaint;
		private SKPaint _highlightPaint;
		private SKPaint _trackHeaderTypePaint;
		private SKPaint _trackHeaderTitlePaint;
		private Dictionary<LyricsTrackType, SKPaint> _trackHeaderPaints;

		private TimelineCanvas _timelineCanvas;
		private int _selectedTrackId = -1;
		private KaraokeProject? _currentProject;
		private float _horizZoomFactor = 1.0f;
		private float _verticalZoomFactor = 1.0f;
		private double _currentVideoPosition;
		private bool _mouseDown = false;

		/// <summary>
		/// Called when the video position was changed by this control and needs to be propogated.
		/// </summary>
		public event Action<double>? OnPositionChangedEvent;
		
		// TODO: merge selection systems together? unified for multiple types?

		/// <summary>
		/// Called when the currently selected event has changed.
		/// </summary>
		public event Action<LyricsEvent?>? OnEventSelectionChanged;

		/// <summary>
		/// Called when the currently selected track has changed.
		/// </summary>
		public event Action<LyricsTrack?>? OnTrackSelectionChanged;

		public TimelineControl()
		{
			InitializeComponent();

			_timelineCanvas = new TimelineCanvas();
			_timelineCanvas.OnEventSelectionChanged += _timelineCanvas_OnEventSelectionChanged;

			_backgroundColor = VisualStyle.NeutralDarkColor.ToSKColor();
			_backgroundPaint = new SKPaint() { Color = _backgroundColor };

			_shadowPaint = new SKPaint() { Color = new SKColor(0, 0, 0, 127) };
			_selectTrackPaint = new SKPaint() { Color = new SKColor(255, 255, 255, 25) };

			_lightPaint = new SKPaint() { Color = VisualStyle.NeutralLightColor.ToSKColor() };
			_highlightPaint = new SKPaint() { 
				Color = VisualStyle.HighlightColor.ToSKColor(),
				IsAntialias = true
			};

			_trackHeaderPaints = VisualStyle.TrackColors.Select(kv => (kv.Key, new SKPaint() { Color = kv.Value.ToSKColor() })).ToDictionary(k => k.Key, v => v.Item2);

			_trackHeaderTitlePaint = new SKPaint()
			{
				Color = new SKColor(255, 255, 255, 255),
				Typeface = SKTypeface.FromFamilyName("Open Sans", SKFontStyleWeight.Bold, SKFontStyleWidth.Normal, SKFontStyleSlant.Upright),
				TextSize = 18,
				IsAntialias = true
			};

			_trackHeaderTypePaint = new SKPaint()
			{
				Color = new SKColor(255, 255, 255, 127),
				Typeface = SKTypeface.FromFamilyName("Open Sans", SKFontStyleWeight.Normal, SKFontStyleWidth.Normal, SKFontStyleSlant.Italic),
				TextSize = 12,
				IsAntialias = true
			};

			horizScroll.Enabled = false;
			verticalScroll.Enabled = false;
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_currentProject = project;
			_selectedTrackId = -1;
			_timelineCanvas.OnProjectChanged(project);

			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			_timelineCanvas.OnProjectChanged(project);
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		internal void OnPositionChanged(double pos)
		{
			_currentVideoPosition = pos;
			skiaControl.Invalidate();
		}

		private bool SelectEventAtPosition(Point pos)
		{
			var result = _timelineCanvas.SelectEventAtPoint(TranslatePointToCanvas(pos));
			return result;
		}

		private bool SelectTrackAtPosition(Point pos)
		{
			_selectedTrackId = -1;
			if (_currentProject == null)
			{
				return false;
			}

			var yPos = pos.Y - PADDING_TOP;

			if(pos.X < TRACK_HEADER_WIDTH && yPos < _currentProject.Tracks.Count() * TimelineCanvas.TRACK_HEIGHT)
			{
				_selectedTrackId = _currentProject.Tracks.Skip((int)(yPos / TimelineCanvas.TRACK_HEIGHT)).FirstOrDefault()?.Id ?? -1;
				return _selectedTrackId != -1;
			}

			return false;
		}

		private bool HandleSelection(Point pos)
		{
			_selectedTrackId = -1;
			_timelineCanvas.Deselect();

			if (!SelectEventAtPosition(pos) && !SelectTrackAtPosition(pos))
			{
				return false;
			}

			skiaControl.Invalidate();
			return true;
		}

		/// <summary>
		/// Returns the XY position in canvas space of the (0, 0) coordinate in control space.
		/// </summary>
		private SKPoint GetCanvasOffset()
		{
			return new SKPoint(
				-(horizScroll.Enabled ? horizScroll.Value : 0.0f) + TRACK_HEADER_WIDTH,
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
			var rect = CreateMatrix().MapRect(new SKRect(0, 0, _timelineCanvas.Size.Width, _timelineCanvas.Size.Height));
			var size = new SKSize(rect.Width, rect.Height);

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
			var visibleRect = CreateInverseMatrix().MapRect(new SKRect(clientRect.Left, clientRect.Top, clientRect.Right, clientRect.Bottom));
			_timelineCanvas.CopyContents(canvas, matrix, visibleRect);

			canvas.RestoreToCount(savePoint);

			PaintTrackHeaders(canvas);

			var playheadXPos = CreateMatrix().MapPoint(new SKPoint((float)(_timelineCanvas.GetXPosOfTime(_currentVideoPosition)), 0)).X;

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

		private void PaintTrackHeaders(SKCanvas canvas)
		{
			int savePoint = canvas.Save();
			canvas.DrawRect(0, 0, TRACK_HEADER_WIDTH, ClientSize.Height, _backgroundPaint);

			canvas.RestoreToCount(savePoint);

			if (_currentProject == null)
			{
				return;
			}

			savePoint = canvas.Save();

			var nextY = PADDING_TOP;
			var rightPadding = 3.0f;
			foreach (var track in _currentProject.Tracks)
			{
				canvas.DrawRect(0, nextY, TRACK_HEADER_WIDTH - rightPadding, TimelineCanvas.TRACK_HEIGHT, _trackHeaderPaints[track.Type]);
				canvas.DrawRect(0, nextY + TimelineCanvas.TRACK_HEIGHT - 2, TRACK_HEADER_WIDTH - rightPadding, 2, _shadowPaint);

				// draw light overlay for selected track
				if(_selectedTrackId == track.Id)
				{
					canvas.DrawRect(0, nextY, TRACK_HEADER_WIDTH - rightPadding, TimelineCanvas.TRACK_HEIGHT, _selectTrackPaint);
				}

				var trackTypeStr = track.Type.ToString();
				var trackTypeWidth = _trackHeaderTypePaint.MeasureText(trackTypeStr);

				canvas.DrawText(trackTypeStr, new SKPoint(TRACK_HEADER_WIDTH - trackTypeWidth - 5 - rightPadding, TimelineCanvas.TRACK_HEIGHT - 5), _trackHeaderTypePaint);
				var trackTitleHeight = StyleUtil.GetFontHeight(_trackHeaderTitlePaint.ToFont());
				canvas.DrawText("Track " + track.Id, new SKPoint(5, 5 + trackTitleHeight), _trackHeaderTitlePaint);
				nextY += TimelineCanvas.TRACK_HEIGHT;
			}

			canvas.RestoreToCount(savePoint);
		}

		private void UpdateZoom(float horiz, float vertical)
		{
			var centerPos = CreateInverseMatrix().MapPoint(new SKPoint(ClientSize.Width / 2.0f, 0));
			var centerTime = _timelineCanvas.GetTimeOfPoint(centerPos);

			_horizZoomFactor = horiz;
			_verticalZoomFactor = vertical;
			RecalculateScrollBars();
			_currentVideoPosition = centerTime;
			skiaControl.Invalidate();

			var scrollOffset = CreateMatrix().MapPoint(new SKPoint((float)_timelineCanvas.GetXPosOfTime(centerTime), 0)).X;
			// we need to position half a screen before the new center pos, so it remains the center
			var offsetWidth = CreateInverseMatrix().MapPoint(new SKPoint(ClientSize.Width / 2.0f, 0)).X;
			horizScroll.Value = Math.Clamp((int)(scrollOffset - ClientSize.Width / 2.0f), horizScroll.Minimum, horizScroll.Maximum);
		}

		private RectangleF GetViewportClientRect()
		{
			return new RectangleF(
				TRACK_HEADER_WIDTH, 
				PADDING_TOP,
				ClientSize.Width - TRACK_HEADER_WIDTH,
				ClientSize.Height - PADDING_TOP
			);
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
		}

		private void horizScroll_Scroll(object sender, ScrollEventArgs e)
		{
			skiaControl.Invalidate();
		}

		private void skiaControl_MouseDown(object sender, MouseEventArgs e)
		{
			var oldSelectedTrackId = _selectedTrackId;
			_mouseDown = !HandleSelection(e.Location);

			if (_selectedTrackId != oldSelectedTrackId)
			{
				OnTrackSelectionChanged?.Invoke(_currentProject?.Tracks.Where(t => t.Id == _selectedTrackId).FirstOrDefault());
			}
		}

		private void skiaControl_MouseUp(object sender, MouseEventArgs e)
		{
			_mouseDown = false;
		}

		private void skiaControl_MouseMove(object sender, MouseEventArgs e)
		{
			if (_mouseDown)
			{
				var pos = _timelineCanvas.GetTimeOfPoint(TranslatePointToCanvas(e.Location));
				OnPositionChangedEvent?.Invoke(pos);
			}
		}

		private void _timelineCanvas_OnEventSelectionChanged(LyricsEvent? obj)
		{
			OnEventSelectionChanged?.Invoke(obj);
		}
		#endregion
	}
}
