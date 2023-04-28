using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	public class VideoStyle
	{
		private static readonly char[] ALPHABET = new char[] {
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 
			'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P', 
			'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c', 
			'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
			'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

		private SKFont _font;
		private float _padding = 0.1f;
		private bool _cachedLineHeight = false;
		private float _lineHeight = 0;

		private SKPaint _normalPaint;
		private SKPaint _highlightedPaint;
		private SKPaint _strokePaint;

		public SKFont Font => _font;

		public SKPaint NormalPaint => _normalPaint;

		public SKPaint HighlightedPaint => _highlightedPaint;

		public SKPaint StrokePaint => _strokePaint;

		public float LineHeight
		{
			get
			{
				if (!_cachedLineHeight)
				{
					CacheLineHeight();
				}

				return _lineHeight;
			}
		}


		public VideoStyle(KaraokeConfig config)
		{
			var typeface = SKTypeface.FromFamilyName(config.FontFamily, config.FontWeight, config.FontWidth, config.FontSlant);
			_font = new SKFont(typeface, config.FontSize);

			_normalPaint = new SKPaint(_font)
			{
				IsAntialias = true,
				TextSize = config.FontSize,
				Color = config.NormalColor,
				Style = SKPaintStyle.Fill
			};

			_highlightedPaint = new SKPaint(_font)
			{
				IsAntialias = true,
				TextSize = config.FontSize,
				Color = config.HighlightColor,
				Style = SKPaintStyle.Fill
			};

			_strokePaint = new SKPaint(_font)
			{
				IsAntialias = true,
				TextSize = config.FontSize,
				Color = config.StrokeColor,
				Style = SKPaintStyle.Stroke,
				StrokeJoin = SKStrokeJoin.Miter,
				StrokeWidth = config.StrokeWidth
			};

			_padding = config.FramePaddingPercent;
		}

		public SKRect GetSafeArea(SKSize size)
		{
			var paddingX = size.Width * _padding * 0.5f;
			var paddingY = size.Height * _padding * 0.5f;

			return new SKRect(
				paddingX, 
				paddingY, 
				paddingX + size.Width - paddingX * 2, 
				paddingY + size.Height - paddingY * 2
			);
		}

		public float GetTextWidth(string text, SKPaint? paint = null)
		{
			return _font.MeasureText(text.ToCharArray().Select(c => _font.GetGlyph(c)).ToArray(), paint ?? NormalPaint);
		}

		private void CacheLineHeight()
		{
			var glyphs = ALPHABET.Select(c => _font.GetGlyph(c)).ToArray();
			SKRect bounds;
			_font.MeasureText(glyphs, out bounds);
			_lineHeight = bounds.Height;
			_cachedLineHeight = true;
		}
	}
}
