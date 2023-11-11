using KaraokeLib.Video.Elements;
using SkiaSharp;

namespace KaraokeLib.Video.Transitions
{
	internal class SwipeVideoTransition : IVideoTransition
	{
		public VideoTransitionType Type => VideoTransitionType.Swipe;

		public void Blit(IVideoElement elem, TransitionContext context)
		{
			var rect = new SKRect(
				elem.Position.X + (context.IsStartTransition ? 0 : (1.0f - context.TransitionPosition) * elem.Size.Width),
				elem.Position.Y,
				elem.Position.X + (context.IsStartTransition ? context.TransitionPosition * elem.Size.Width : elem.Size.Width),
				elem.Position.Y + elem.Size.Height * 2);

			var savePos = context.Destination.Save();
			context.Destination.ClipRect(rect);
			elem.Render(context.VideoContext, context.Destination, context.VideoPosition);
			context.Destination.RestoreToCount(savePos);
		}
	}
}
