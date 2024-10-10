using KaraokeLib.Util;

namespace KaraokeLib.Video.Transitions
{
	public struct TransitionConfig
	{
		public VideoTransitionType Type { get; set; }

		public double Duration { get; set; }

		public EasingType EasingCurve { get; set; }
	}
}
