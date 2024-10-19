using SkiaSharp;
using System.Drawing;

namespace KaraokeLib.Util
{
	public struct KSize
	{
		public int Width;
		public int Height;

		public KSize(int width, int height)
		{
			Width = width;
			Height = height;
		}

		public KSize(SKSize other)
		{
			Width = (int)other.Width;
			Height = (int)other.Height;
		}


		public static implicit operator SKSize(KSize s) => new SKSize(s.Width, s.Height);
		public static implicit operator Size(KSize s) => new Size(s.Width, s.Height);
		public static implicit operator KSize(Size s) => new KSize(s.Width, s.Height);
		public static implicit operator SKPoint(KSize s) => new SKPoint(s.Width, s.Height);
	}
}
