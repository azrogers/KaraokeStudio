using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KaraokeLib.Util
{
	public struct KColor
	{
		public byte Red;
		public byte Green;
		public byte Blue;

		public KColor(byte r, byte g, byte b)
		{
			Red = r;
			Green = g;
			Blue = b;
		}

		public KColor(SKColor color)
		{
			Red = color.Red;
			Green = color.Green;
			Blue = color.Blue;
		}

		public SKColor ToSKColor() => new SKColor(Red, Green, Blue);

		public static implicit operator SKColor(KColor c) => new SKColor(c.Red, c.Green, c.Blue);
	}
}
