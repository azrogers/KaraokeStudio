using KaraokeLib.Util;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib
{
	public class KaraokeConfig
	{
		public static KaraokeConfig Default = new KaraokeConfig();

		public int FrameRate = 30;
		public int VideoWidth = 1920;
		public int VideoHeight = 1080;

		public KColor NormalColor = new KColor(255, 255, 255);
		public KColor HighlightColor = new KColor(70, 175, 90);
		public KColor StrokeColor = new KColor(0, 0, 0);

		public float StrokeWidth = 3;

		public float FontSize = 200;
		public string FontFamily = "Arial";
		public SKFontStyleWidth FontWidth = SKFontStyleWidth.Normal;
		public SKFontStyleWeight FontWeight = SKFontStyleWeight.Bold;
		public SKFontStyleSlant FontSlant = SKFontStyleSlant.Upright;

		public float FramePaddingPercent = 0.1f;

		public double LyricLeadTime = 2.0;
		public double LyricTrailTime = 1.0;
		public double MinTimeBetweenSections = 10.0;
	}
}
