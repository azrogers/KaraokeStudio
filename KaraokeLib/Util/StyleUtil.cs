using SkiaSharp;

namespace KaraokeLib.Util
{
	public static class StyleUtil
	{
		private static readonly char[] ALPHABET = new char[] {
			'0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C',
			'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'O', 'P',
			'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z', 'a', 'b', 'c',
			'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'o', 'p',
			'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z' };

		/// <summary>
		/// Returns the line height of the given font.
		/// </summary>
		public static float GetFontHeight(SKFont font)
		{
			var glyphs = ALPHABET.Select(c => font.GetGlyph(c)).ToArray();
			font.MeasureText(glyphs, out var bounds);
			return bounds.Height;
		}
	}
}
