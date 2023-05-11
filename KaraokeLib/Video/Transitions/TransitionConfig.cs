using KaraokeLib.Util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Transitions
{
	public class TransitionConfig
	{
		public VideoTransitionType Type { get; set; }

		public double Duration { get; set; }

		public EasingType EasingCurve { get; set; }
	}
}
