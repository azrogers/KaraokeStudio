using KaraokeLib;
using KaraokeLib.Util;
using KaraokeLib.Video.Elements;
using KaraokeLib.Video.Plan;
using KaraokeLib.Video.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
			canvas.Clear();
			canvas.DrawRect(
				new SKRect(0, 0, _context.Size.Width, _context.Size.Height),
				_context.Style.BackgroundPaint);

			var posSeconds = videoTimecode.ToSeconds();

			// draw to an intermediate surface for transitions
			var surface = SKSurface.Create(new SKImageInfo((int)_context.Size.Width, (int)_context.Size.Height));

			var elements = videoPlan.GetElementsForFrame(videoTimecode);
			foreach (var ev in elements)
			{
				surface.Canvas.Clear();

				ev.Element.Render(_context, surface.Canvas, posSeconds);

				var startTime = ev.Element.StartTimecode.GetTimeSeconds();
				var endTime = ev.Element.EndTimecode.GetTimeSeconds();

				// handle transitions if necessary
				if(
					posSeconds >= startTime && 
					(startTime + ev.Element.StartTransition.Duration) > posSeconds)
				{
					// 0 when starting, 1 when finishing
					var t = (float)Math.Clamp((posSeconds - startTime) / ev.Element.StartTransition.Duration, 0, 1);
					HandleTransition(ev.Element.StartTransition, ev.Element, surface, canvas, t, true);
				}
				else if(
					posSeconds < endTime &&
					(endTime - ev.Element.EndTransition.Duration) < posSeconds)
				{
					var beforeT = (ev.Element.EndTransition.Duration - (endTime - posSeconds)) / ev.Element.EndTransition.Duration;
					// 1 when starting, 0 when finishing
					var t = (float)Math.Clamp(1.0 - beforeT, 0, 1);
					HandleTransition(ev.Element.EndTransition, ev.Element, surface, canvas, t, false);
				}
				else
				{
					canvas.DrawSurface(surface, SKPoint.Empty);
				}
			}
		}

		private void HandleTransition(TransitionConfig transition, IVideoElement elem, SKSurface surface, SKCanvas dest, float t, bool isStartTransition)
		{
			var realT = EasingFunctions.Evaluate(transition.EasingCurve, t);
			TransitionManager.Get(transition.Type).Blit(elem, surface, dest, realT, isStartTransition);
		}
	}
}
