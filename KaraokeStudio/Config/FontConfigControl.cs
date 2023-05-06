using KaraokeLib.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SkiaSharp;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace KaraokeStudio.Config
{
	public partial class FontConfigControl : BaseConfigControl
	{
		private KFont _font;

		public FontConfigControl()
		{
			InitializeComponent();
		}

		private void fontButton_Click(object sender, EventArgs e)
		{
			var dialog = new FontDialog();
			dialog.Font = FontFromKFont(_font, _font.Size);
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				_font = new KFont()
				{
					Family = dialog.Font.FontFamily.Name,
					Size = dialog.Font.Size,
					Slant = dialog.Font.Italic ? SKFontStyleSlant.Italic : SKFontStyleSlant.Upright,
					Weight = dialog.Font.Bold ? SKFontStyleWeight.Bold : SKFontStyleWeight.Normal,
					Width = SKFontStyleWidth.Normal
				};
				UpdateLabel();
				SendValueChanged();
			}
		}

		private void UpdateLabel()
		{
			fontLabel.Text = $"{_font.Family}, Size {_font.Size}{(_font.Weight != SKFontStyleWeight.Normal ? ", Bold" : "")}{(_font.Slant != SKFontStyleSlant.Upright ? ", Italic" : "")}";
			fontLabel.Font = FontFromKFont(_font, fontLabel.Font.Size);
		}

		private Font FontFromKFont(KFont font, float size)
		{
			var style = FontStyle.Regular;
			if (font.Slant == SKFontStyleSlant.Italic)
			{
				style |= FontStyle.Italic;
			}
			if (font.Weight == SKFontStyleWeight.Bold)
			{
				style |= FontStyle.Bold;
			}

			return new Font(font.Family, size, style);
		}

		internal override void UpdateValue(object config)
		{
			var val = Field?.GetValue<KFont>(config);
			if(val != null)
			{
				_font = val ?? new KFont();
				UpdateLabel();
			}
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, _font);
		}
	}
}
