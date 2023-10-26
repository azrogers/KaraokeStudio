using Newtonsoft.Json.Linq;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
		private float PaddingTop => 5.0f;

		private SKColor _backgroundColor;
		private SKPaint _lightPaint;
		private SKPaint _highlightPaint;

		private TimelineCanvas _timelineCanvas;
		private KaraokeProject? _currentProject;
		private float _horizZoomFactor = 1.0f;
		private float _verticalZoomFactor = 1.0f;
		private double _currentVideoPosition;
		private bool _mouseDown = false;

		public event Action<double>? OnPositionChangedEvent;

		public TimelineControl()
		{
			InitializeComponent();

			_timelineCanvas = new TimelineCanvas();

			_backgroundColor = VisualStyle.NeutralDarkColor.ToSKColor();
			_lightPaint = new SKPaint() { Color = VisualStyle.NeutralLightColor.ToSKColor() };
			_highlightPaint = new SKPaint() { 
				Color = VisualStyle.HighlightColor.ToSKColor(),
				IsAntialias = true
			};

			horizScroll.Enabled = false;
			verticalScroll.Enabled = false;
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_currentProject = project;
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
			skiaControl.Invalidate();
			return result;
		}

		/// <summary>
		/// Returns the XY position in canvas space of the (0, 0) coordinate in control space.
		/// </summary>
		private SKPoint GetCanvasOffset()
		{
			return new SKPoint(
				-(horizScroll.Enabled ? horizScroll.Value : 0.0f) * _horizZoomFactor,
				-(verticalScroll.Enabled ? verticalScroll.Value : 0.0f) * _verticalZoomFactor + PaddingTop
			);
		}

		/// <summary>
		/// Creates a matrix to transform from control space to canvas space.
		/// </summary>
		private SKMatrix CreateMatrix()
		{
			var translation = GetCanvasOffset();
			return SKMatrix.CreateScaleTranslation(_horizZoomFactor, _verticalZoomFactor, translation.X, translation.Y);
		}

		/// <summary>
		/// Creates a matrix to transform from canvas space to control space.
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

			var size = new SKSize(
				_timelineCanvas.Size.Width * _horizZoomFactor,
				_timelineCanvas.Size.Height * _verticalZoomFactor
			);

			verticalScroll.Enabled = size.Height > ClientSize.Height;
			horizScroll.Enabled = size.Width > ClientSize.Width;

			verticalScroll.Minimum = 0;
			verticalScroll.Maximum = (int)Math.Ceiling(size.Height - ClientSize.Height);
			horizScroll.Minimum = 0;
			horizScroll.Maximum = (int)Math.Ceiling(size.Width - ClientSize.Width);
		}

		private void PaintSurface(SKCanvas canvas)
		{
			canvas.Clear(_backgroundColor);

			var savePoint = canvas.Save();
			var offset = GetCanvasOffset();
			var matrix = CreateMatrix();

			var visibleRect = CreateInverseMatrix().MapRect(new SKRect(0, PaddingTop, ClientSize.Width, ClientSize.Height));
			_timelineCanvas.CopyContents(canvas, matrix, visibleRect);

			canvas.RestoreToCount(savePoint);

			var playheadXPos = (float)(_currentVideoPosition * _timelineCanvas.PixelsPerSecond) * _horizZoomFactor;

			savePoint = canvas.Save();
			canvas.Translate(offset.X, 0);
			canvas.DrawLine(new SKPoint(playheadXPos, 0), new SKPoint(playheadXPos, ClientSize.Height), _lightPaint);

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
			_horizZoomFactor *= 2.0f;
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void horizMinusButton_Click(object sender, EventArgs e)
		{
			_horizZoomFactor = Math.Max(_horizZoomFactor * 0.5f, 0.01f);
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void verticalPlusButton_Click(object sender, EventArgs e)
		{
			_verticalZoomFactor *= 2.0f;
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void verticalMinusButton_Click(object sender, EventArgs e)
		{
			_verticalZoomFactor = Math.Max(_verticalZoomFactor * 0.5f, 0.01f);
			RecalculateScrollBars();
			skiaControl.Invalidate();
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
			if(!SelectEventAtPosition(e.Location))
			{
			}

			_mouseDown = true;
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
		#endregion
	}
}
