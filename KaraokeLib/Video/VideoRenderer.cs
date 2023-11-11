using KaraokeLib.Util;
using KaraokeLib.Video.Elements;
using KaraokeLib.Video.Plan;
using KaraokeLib.Video.Transitions;
using SkiaSharp;

namespace KaraokeLib.Video
{
	public class VideoRenderer
	{
		private VideoContext _context;
		private VideoSection[] _sections;
		private IVideoElement[] _elements;

		public VideoRenderer(VideoContext context, VideoLayoutState layoutState, VideoSection[] sections)
		{
			_context = context;
			_sections = sections;
			_elements = VideoElementGenerator.Generate(context, layoutState, sections);
		}

		public void RenderFrame(VideoPlan videoPlan, VideoTimecode videoTimecode, SKCanvas canvas)
		{
			// we can ignore clearing because we draw over it anyways
			canvas.DrawRect(
				new SKRect(0, 0, _context.Size.Width, _context.Size.Height),
				_context.Style.BackgroundPaint);

			var posSeconds = videoTimecode.ToSeconds();

			// draw to an intermediate surface for transitions
			var surface = SKSurface.Create(new SKImageInfo((int)_context.Size.Width, (int)_context.Size.Height));

			var elements = videoPlan.GetElementsForFrame(videoTimecode);
			foreach (var ev in elements)
			{
				var startTime = ev.Element.StartTimecode.GetTimeSeconds();
				var endTime = ev.Element.EndTimecode.GetTimeSeconds();

				// handle transitions if necessary
				if (
					posSeconds >= startTime &&
					(startTime + ev.Element.StartTransition.Duration) > posSeconds)
				{
					// 0 when starting, 1 when finishing
					var t = (float)Math.Clamp((posSeconds - startTime) / ev.Element.StartTransition.Duration, 0, 1);
					var context = new TransitionContext()
					{
						Destination = canvas,
						Surface = surface,
						IsStartTransition = true,
						TransitionPosition = t,
						VideoContext = _context,
						VideoPosition = posSeconds
					};
					HandleTransition(ev.Element.StartTransition, ev.Element, context);
				}
				else if (
					posSeconds < endTime &&
					(endTime - ev.Element.EndTransition.Duration) < posSeconds)
				{
					var beforeT = (ev.Element.EndTransition.Duration - (endTime - posSeconds)) / ev.Element.EndTransition.Duration;
					// 1 when starting, 0 when finishing
					var t = (float)Math.Clamp(1.0 - beforeT, 0, 1);
					var context = new TransitionContext()
					{
						Destination = canvas,
						Surface = surface,
						IsStartTransition = false,
						TransitionPosition = t,
						VideoContext = _context,
						VideoPosition = posSeconds
					};
					HandleTransition(ev.Element.StartTransition, ev.Element, context);
				}
				else
				{
					// skip the surface and draw directly to the canvas
					ev.Element.Render(_context, canvas, posSeconds);
				}
			}

			surface.Dispose();
		}

		private void HandleTransition(TransitionConfig transition, IVideoElement elem, TransitionContext context)
		{
			var realT = EasingFunctions.Evaluate(transition.EasingCurve, context.TransitionPosition);
			TransitionManager.Get(transition.Type).Blit(elem, context);
		}
	}
}
