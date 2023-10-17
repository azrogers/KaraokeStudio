using KaraokeLib.Util;
using Melanchall.DryWetMidi.Tools;
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
		private SKFont _font;
		private KaraokeConfig _config;

		private SKPaint _normalPaint;
		private SKPaint _highlightedPaint;
		private SKPaint _strokePaint;
		private SKPaint _backgroundPaint;

		public SKFont Font => _font;

		public SKPaint NormalPaint => _normalPaint;

		public SKPaint HighlightedPaint => _highlightedPaint;

		public SKPaint StrokePaint => _strokePaint;
		public SKPaint BackgroundPaint => _backgroundPaint;

		public float LineHeight { get; private set; }


		public VideoStyle(KaraokeConfig config)
		{
			var typeface = SKTypeface.FromFamilyName(config.Font.Family, config.Font.Weight, config.Font.Width, config.Font.Slant);
			_font = new SKFont(typeface, config.Font.Size);

			_normalPaint = new SKPaint(_font)
			{
				IsAntialias = true,
				TextSize = config.Font.Size,
				Color = config.NormalColor,
				Style = SKPaintStyle.Fill
			};

			_highlightedPaint = new SKPaint(_font)
			{
				IsAntialias = true,
				TextSize = config.Font.Size,
				Color = config.HighlightColor,
				Style = SKPaintStyle.Fill
			};

			_strokePaint = new SKPaint(_font)
			{
				IsAntialias = true,
				TextSize = config.Font.Size,
				Color = config.StrokeColor,
				Style = SKPaintStyle.Stroke,
				StrokeJoin = SKStrokeJoin.Miter,
				StrokeWidth = config.StrokeWidth
			};

			_backgroundPaint = new SKPaint()
			{
				Color = config.BackgroundColor,
				Style = SKPaintStyle.Fill
			};

			LineHeight = StyleUtil.GetFontHeight(_font);
			_config = config;
		}

		public SKRect GetSafeArea(SKSize size)
		{
			return new SKRect(
				_config.Padding.Left, 
				_config.Padding.Top, 
				size.Width - _config.Padding.Right, 
				size.Height - _config.Padding.Bottom
			);
		}

		public float GetTextWidth(string text, SKPaint? paint = null)
		{
			return _font.MeasureText(text.ToCharArray().Select(c => _font.GetGlyph(c)).ToArray(), paint ?? NormalPaint);
		}
	}
}
