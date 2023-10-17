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

namespace KaraokeStudio.LyricsEditor
{
	public partial class LyricsEditorControl : UserControl
	{
		private readonly RectangleF Padding = new RectangleF(15.0f, 15.0f, 15.0f, 15.0f);

		private LyricsView _view;
		private KaraokeProject? _project;
		private Size? _previousClientSize;

		public LyricsEditorControl()
		{
			InitializeComponent();
			_view = new LyricsView();
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_project = project;
			UpdateView();
			skiaControl.Invalidate();
		}

		private void UpdateView()
		{
			_view.UpdateView(_project, new RectangleF(0, 0, skiaControl.ClientSize.Width, skiaControl.ClientSize.Height));
		}

		private SKMatrix CreateTranslationMatrix() => SKMatrix.CreateTranslation(0, -scrollBar.Value);
		private SKPoint TranslatePointToCanvas(Point point) => CreateTranslationMatrix().Invert().MapPoint(new SKPoint(point.X, point.Y));

		private void skiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			var viewRect = new SKRect(0, 0, skiaControl.ClientSize.Width, skiaControl.ClientSize.Height);

			if (_previousClientSize == null || _previousClientSize != skiaControl.ClientSize)
			{
				_previousClientSize = skiaControl.ClientSize;
				UpdateView();
				// TODO: restore accurate scroll pos
			}

			// update scroll bar based on new height
			scrollBar.Maximum = (int)(_view.Height - viewRect.Height);
			scrollBar.SmallChange = (int)(viewRect.Height / 20);
			scrollBar.LargeChange = (int)(viewRect.Height / 10);

			_view.Render(e.Surface.Canvas, CreateTranslationMatrix(), viewRect);
		}

		private void scrollBar_Scroll(object sender, ScrollEventArgs e)
		{
			skiaControl.Invalidate();
		}

		private void skiaControl_MouseDown(object sender, MouseEventArgs e)
		{
			_view.UpdateCursorPosition(TranslatePointToCanvas(e.Location));
			skiaControl.Invalidate();
		}

		private void LyricsEditorControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{

			}
		}
	}
}
