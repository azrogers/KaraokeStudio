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

		[ConfigRange(1)]
		public int FrameRate = 30;
		public KSize VideoSize = new(1920, 1080);

		public KColor NormalColor = new KColor(255, 255, 255);
		public KColor HighlightColor = new KColor(70, 175, 90);
		public KColor StrokeColor = new KColor(0, 0, 0);

		[ConfigRange(0.0)]
		public float StrokeWidth = 3;

		public KFont Font = new KFont()
		{
			Size = 200,
			Family = "Arial",
			Weight = SKFontStyleWeight.Bold,
			Slant = SKFontStyleSlant.Upright,
			Width = SKFontStyleWidth.Normal
		};

		[ConfigRange(0.0, 1.0)]
		public float FramePaddingPercent = 0.1f;

		[ConfigRange(0.0)]
		public double LyricLeadTime = 2.0;
		[ConfigRange(0.0)]
		public double LyricTrailTime = 1.0;

		// TODO: should be moved to a separate "import settings" class with a separate window for MIDI import settings
		[ConfigRange(0.0)]
		public double MinTimeBetweenSections = 10.0;
	}

	public class ConfigRangeAttribute : Attribute
	{
		private bool _isDecimal = false;
		private bool _hasMin = true;
		private bool _hasMax = true;
		private double _min;
		private double _max;

		public bool HasMax => _hasMax;

		public bool IsDecimal => _isDecimal;

		public double Minimum => _min;

		public double Maximum => _max;

		public ConfigRangeAttribute(double min, double max)
		{
			_min = min;
			_max = max;
			_isDecimal = true;
		}

		public ConfigRangeAttribute(int min, int max)
		{
			_isDecimal = false;
			_min = min;
			_max = max;
		}

		public ConfigRangeAttribute(double min)
		{
			_min = min;
			_max = -1;
			_hasMax = false;
			_isDecimal = true;
		}

		public ConfigRangeAttribute(int min)
		{
			_isDecimal = false;
			_min = min;
			_max = -1;
			_hasMax = false;
		}
	}
}
