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

		internal void OnPositionChanged(double pos)
		{
			_currentVideoPosition = pos;
			skiaControl.Invalidate();
		}

		private void SelectEventAtPosition(Point pos)
		{
			_timelineCanvas.SelectEventAtPoint(TranslatePointToCanvas(pos));
			skiaControl.Invalidate();
		}

		private SKPoint TranslatePointToCanvas(Point pos)
		{
			var matrix = SKMatrix.CreateScaleTranslation(
				1.0f / _horizZoomFactor,
				1.0f / _verticalZoomFactor,
				(horizScroll.Enabled ? horizScroll.Value : 0.0f) * _horizZoomFactor,
				(verticalScroll.Enabled ? verticalScroll.Value : 0.0f) * _verticalZoomFactor - PaddingTop);
			return matrix.MapPoint(new SKPoint(pos.X, pos.Y));
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

		private void skiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			var canvas = e.Surface.Canvas;
			canvas.Clear(_backgroundColor);

			var savePoint = canvas.Save();

			var drawX = -(horizScroll.Enabled ? horizScroll.Value : 0.0f) * _horizZoomFactor;
			var drawY = -(verticalScroll.Enabled ? verticalScroll.Value : 0.0f) * _verticalZoomFactor + PaddingTop;

			var matrix = SKMatrix.CreateScaleTranslation(_horizZoomFactor, _verticalZoomFactor, drawX, drawY);

			_timelineCanvas.CopyContents(canvas, matrix);

			canvas.RestoreToCount(savePoint);

			var playheadXPos = (float)(_currentVideoPosition * _timelineCanvas.PixelsPerSecond) * _horizZoomFactor;

			savePoint = canvas.Save();
			canvas.Translate(drawX, 0);
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

		private void skiaControl_Layout(object sender, LayoutEventArgs e)
		{
			RecalculateScrollBars();
		}

		private void horizPlusButton_Click(object sender, EventArgs e)
		{
			_horizZoomFactor = Math.Max(_horizZoomFactor * 0.5f, 0.01f);
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void horizMinusButton_Click(object sender, EventArgs e)
		{
			_horizZoomFactor *= 2.0f;
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void verticalPlusButton_Click(object sender, EventArgs e)
		{
			_verticalZoomFactor = Math.Max(_verticalZoomFactor * 0.5f, 0.01f);
			RecalculateScrollBars();
			skiaControl.Invalidate();
		}

		private void verticalMinusButton_Click(object sender, EventArgs e)
		{
			_verticalZoomFactor *= 2.0f;
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
			SelectEventAtPosition(e.Location);
		}
	}
}
