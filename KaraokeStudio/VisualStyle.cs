using FontAwesome.Sharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal static class VisualStyle
	{
		public static readonly Color NeutralLightColor = Color.FromArgb(251, 255, 254);
		public static readonly Color NeutralDarkColor = Color.FromArgb(39, 41, 50);
		public static readonly Color HighlightColor = Color.FromArgb(59, 142, 165);
		public static readonly Color PositiveColor = Color.FromArgb(112, 174, 110);
		public static readonly Color NegativeColor = Color.FromArgb(240, 93, 94);

		private static Dictionary<IconChar, Bitmap> _buttonBitmaps = new Dictionary<IconChar, Bitmap>();

		public static void PaintIconButton(Graphics g, Button button, IconChar icon)
		{
			if(!_buttonBitmaps.TryGetValue(icon, out var bitmap))
			{
				bitmap = _buttonBitmaps[icon] = icon.ToBitmap(HighlightColor, 32);
			}

			g.FillRectangle(new SolidBrush(NeutralDarkColor), button.ClientRectangle);
			var height = button.ClientRectangle.Height * 0.8f;
			var rect = new RectangleF(
				(float)button.ClientRectangle.Width / 2 - height / 2, 
				(float)button.ClientRectangle.Height / 2 - height / 2, 
				height, 
				height);
			g.DrawImage(bitmap, rect);
		}
	}
}
