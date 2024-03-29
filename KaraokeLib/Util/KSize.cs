﻿using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
	}
}
