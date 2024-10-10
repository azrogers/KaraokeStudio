namespace KaraokeStudio
{
	public partial class VolumeSlider : UserControl
	{
		public float Volume { get; set; }

		public event Action<float>? OnVolumeChanged;

		private Brush _contentBrush;

		public VolumeSlider()
		{
			InitializeComponent();

			_contentBrush = new SolidBrush(VisualStyle.PositiveColor);
		}

		private void VolumeSlider_MouseClick(object sender, MouseEventArgs e)
		{
			var n = Math.Clamp(e.Location.X / (float)Size.Width, 0, 1);
			Volume = n;
		}

		private void VolumeSlider_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.Clear(BackColor);
			var contentRect = new Rectangle(0, 0, (int)(Size.Width * Volume), Size.Height);
			e.Graphics.FillRectangle(_contentBrush, contentRect);
			e.Graphics.DrawRectangle(Pens.Black, ClientRectangle);
			e.Graphics.DrawString(Math.Floor(Volume * 100).ToString() + "%", SystemFonts.DefaultFont, Brushes.Black, ClientRectangle, StringFormat.GenericTypographic);
		}
	}
}
