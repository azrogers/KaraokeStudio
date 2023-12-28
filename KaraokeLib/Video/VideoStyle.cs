using KaraokeLib.Config;
using KaraokeLib.Util;
using SkiaSharp;

namespace KaraokeLib.Video
{
	public class VideoStyle
	{
		public SKFont Font => _font;

		public SKPaint NormalPaint => _normalPaint;

		public SKPaint HighlightedPaint => _highlightedPaint;

		public SKPaint StrokePaint => _strokePaint;
		public SKPaint BackgroundPaint => _backgroundPaint;

		public float LineHeight { get; private set; }

		private SKFont _font;
		private KaraokeConfig _config;

		private SKPaint _normalPaint;
		private SKPaint _highlightedPaint;
		private SKPaint _strokePaint;
		private SKPaint _backgroundPaint;

		// buffer used to reduce allocations in GetTextWidth
		private ushort[] _charBuffer = new ushort[256];


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

			LineHeight = StyleUtil.GetFontHeight(_font) * config.LineHeightMultiplier;
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

		/// <summary>
		/// Obtains the width of a given span of text in pixels.
		/// </summary>
		/// <param name="paint">If specified, this will be the paint used to obtain the text width.</param>
		public float GetTextWidth(string text, SKPaint? paint = null)
		{
			if (text.Length > _charBuffer.Length)
			{
				// extend char buffer
				_charBuffer = new ushort[Math.Max(_charBuffer.Length * 2, text.Length)];
			}

			for (var i = 0; i < text.Length; i++)
			{
				_charBuffer[i] = _font.GetGlyph(text[i]);
			}

			return _font.MeasureText(new ReadOnlySpan<ushort>(_charBuffer, 0, text.Length), paint ?? NormalPaint);
		}
	}
}
