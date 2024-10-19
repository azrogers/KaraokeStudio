using KaraokeLib.Tracks;
using KaraokeLib.Util;
using KaraokeLib.Video.Elements;
using KaraokeLib.Video.Plan;
using KaraokeLib.Video.Transitions;
using SkiaSharp;

namespace KaraokeLib.Video
{
	public class VideoRenderer : IDisposable
	{
		private VideoPlan[] _plans;
		private VideoContext _context;

		public VideoRenderer(VideoContext context, IEnumerable<KaraokeTrack> tracks)
		{
			_context = context;

			var plans = new List<VideoPlan>();
			foreach(var track in tracks.OrderByDescending(t => t.Order).Where(t => KaraokeTrackTypeMapping.TrackHasVideoContent(t.Type)))
			{
				var layoutState = new VideoLayoutState(context);
				if(track.Type == KaraokeTrackType.Lyrics)
				{
					var sections = VideoSection.SectionsFromTrack(context, track, layoutState);
					plans.Add(VideoPlanGenerator.CreatePlanFromSections(context, layoutState, sections));
				}
				else
				{
					plans.Add(VideoPlanGenerator.CreatePlanFromElements(context, VideoElementGenerator.GenerateFromEvents(context, track.Events)));
				}
			}

			_plans = [.. plans];
		}

		public void RenderFrame(VideoTimecode timecode, SKCanvas canvas)
		{
			// we can ignore clearing because we draw over it anyways
			canvas.DrawRect(
				new SKRect(0, 0, _context.Size.Width, _context.Size.Height),
				_context.Style.BackgroundPaint);

			foreach(var plan in _plans)
			{
				RenderPlan(plan, timecode, canvas);
			}
		}

		private void RenderPlan(VideoPlan plan, VideoTimecode videoTimecode, SKCanvas canvas)
		{
			var posSeconds = videoTimecode.ToSeconds();

			// draw to an intermediate surface for transitions
			var surface = SKSurface.Create(new SKImageInfo((int)_context.Size.Width, (int)_context.Size.Height));

			var elements = plan.GetElementsForFrame(videoTimecode);
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
					HandleTransition(ev.Element.EndTransition, ev.Element, context);
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

		public void Dispose()
		{
			foreach(var plan in _plans)
			{
				plan.Dispose();
			}

			_plans = [];
		}
	}
}
