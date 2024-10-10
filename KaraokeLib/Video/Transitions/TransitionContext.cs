using SkiaSharp;

namespace KaraokeLib.Video.Transitions
{
	internal struct TransitionContext
	{
		public VideoContext VideoContext;
		public SKSurface Surface;
		public SKCanvas Destination;
		public float TransitionPosition;
		public bool IsStartTransition;
		public double VideoPosition;
	}
}
