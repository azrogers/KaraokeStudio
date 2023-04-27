using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib
{
	internal class KaraokeConfig
	{
		public static KaraokeConfig Default = new KaraokeConfig();

		public int FrameRate = 30;
		public int VideoWidth = 1920;
		public int VideoHeight = 1080;

		public double LyricLeadTime = 2.0;
		public double LyricTrailTime = 1.0;
		public double MinTimeBetweenSections = 10.0;
	}
}
