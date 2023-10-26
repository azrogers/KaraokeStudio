using Cyotek.Windows.Forms;
using KaraokeLib.Util;

namespace KaraokeStudio.Config
{
	public partial class ColorConfigControl : BaseConfigControl
	{
		private KColor _chosenColor;
		private ColorPickerDialog _dialog;

		public ColorConfigControl()
		{
			InitializeComponent();
			_dialog = new ColorPickerDialog();
		}

		private void colorDisplayPanel_Paint(object sender, PaintEventArgs e)
		{
			var brush = new SolidBrush(Color.FromArgb(_chosenColor.Red, _chosenColor.Green, _chosenColor.Blue));
			e.Graphics.FillRectangle(brush, e.ClipRectangle);
			var pen = new Pen(Color.Black, 1);
			e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, colorDisplayPanel.ClientSize.Width - 1, colorDisplayPanel.ClientSize.Height - 1));
		}

		private void colorPickButton_Click(object sender, EventArgs e)
		{
			var result = _dialog.ShowDialog();
			if (result == DialogResult.OK)
			{
				// TODO: support alpha
				_chosenColor = new KColor(_dialog.Color.R, _dialog.Color.G, _dialog.Color.B);
				colorDisplayPanel.Invalidate();
				SendValueChanged();
			}
		}

		internal override void UpdateValue(object value)
		{
			var val = Field?.GetValue<KColor>(value);
			if (val != null)
			{
				_chosenColor = val ?? new KColor(0, 0, 0);
				colorDisplayPanel.Invalidate();
			}
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, _chosenColor);
		}
	}
}
