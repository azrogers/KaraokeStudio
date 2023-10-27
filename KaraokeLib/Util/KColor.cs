using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Drawing;

namespace KaraokeLib.Util
{
	public struct KColor
	{
		public byte Red;
		public byte Green;
		public byte Blue;
		public byte Alpha;

		public KColor(byte r, byte g, byte b)
		{
			Red = r;
			Green = g;
			Blue = b;
			Alpha = 255;
		}

		public KColor(byte r, byte g, byte b, byte a)
		{
			Red = r;
			Green = g;
			Blue = b;
			Alpha = a;
		}

		public KColor(SKColor color)
		{
			Red = color.Red;
			Green = color.Green;
			Blue = color.Blue;
			Alpha = color.Alpha;
		}

		public SKColor ToSKColor() => new SKColor(Red, Green, Blue, Alpha);

		public static implicit operator SKColor(KColor c) => new SKColor(c.Red, c.Green, c.Blue, c.Alpha);
		public static implicit operator Color(KColor c) => Color.FromArgb(c.Alpha, c.Red, c.Green, c.Blue);
		public static implicit operator KColor(Color c) => new KColor(c.R, c.G, c.B, c.A);
	}
}
