using KaraokeLib.Video.Elements;

namespace KaraokeLib.Video.Transitions
{
	internal class NoneVideoTransition : IVideoTransition
	{
		public VideoTransitionType Type => VideoTransitionType.None;

		public void Blit(IVideoElement elem, TransitionContext context)
		{
			elem.Render(context.VideoContext, context.Destination, context.VideoPosition);
		}
	}
}
