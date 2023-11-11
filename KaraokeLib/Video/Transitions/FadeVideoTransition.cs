using KaraokeLib.Video.Elements;
using SkiaSharp;

namespace KaraokeLib.Video.Transitions
{
	internal class FadeVideoTransition : IVideoTransition
	{
		public VideoTransitionType Type => VideoTransitionType.Fade;

		public void Blit(IVideoElement elem, TransitionContext context)
		{
			var paint = new SKPaint()
			{
				ColorF = new SKColorF(1.0f, 1.0f, 1.0f, context.TransitionPosition)
			};

			context.Surface.Canvas.Clear();
			elem.Render(context.VideoContext, context.Surface.Canvas, context.VideoPosition);
			context.Destination.DrawSurface(context.Surface, SKPoint.Empty, paint);
		}
	}
}
