using KaraokeLib.Video.Elements;
using Melanchall.DryWetMidi.Tools;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace KaraokeLib.Video.Plan
{
	/// <summary>
	/// Creates a video plan using a naive approach based on vacant spots in the frame.
	/// </summary>
	internal class VacancyPlanCreator
	{
		private VideoContext _context;
		private IVideoElement[] _elements;

		public VacancyPlanCreator(VideoContext context, IVideoElement[] elements)
		{
			_context = context;
			_elements = elements;
		}

		public VideoPlan CreatePlan(VideoTimecode lastFrameTimecode)
		{
			var plan = new VideoPlan();
			for(var i = 0; i <= lastFrameTimecode.FrameNumber; i++)
			{
				RecordFrameToPlan(plan, new VideoTimecode(i, lastFrameTimecode.FrameRate));
			}

			return plan;
		}

		private void RecordFrameToPlan(VideoPlan plan, VideoTimecode videoTimecode)
		{
			var posSeconds = videoTimecode.ToSeconds();
			var bounds = (posSeconds - _context.Config.LyricTrailTime, posSeconds + _context.Config.LyricLeadTime);

			// TODO: more efficient scan of video elements, instead of iterating every one every time
			List<IVideoElement>[] videoElements;
			videoElements = new List<IVideoElement>[Enum.GetValues<VideoElementPriority>().Length];
			for (var i = 0; i < videoElements.Length; i++)
			{
				videoElements[i] = new List<IVideoElement>();
			}

			foreach (var elem in _elements)
			{
				var priority = elem.GetPriority(posSeconds, bounds);
				videoElements[(int)priority].Add(elem);
			}

			// possible new approach
			// run through the video entirely once without rendering, but with the code as it is
			// then identify issues like text disappearing before it should, paragraphs that overlap
			// and use that information for a second render pass
			// paragraphs that overlap can be done beforehand - offset consecutive paragraphs that don't
			// use all their lines such that the earlier paragraph shows up lower and the later paragraph shows up higher

			var tracker = new FrameVacancyTracker(_context, posSeconds, bounds);
			var frameEvents = new List<IVideoElement>();

			FillEvents(tracker, ref videoElements[(int)VideoElementPriority.Current], ref frameEvents, true);
			FillEvents(tracker, ref videoElements[(int)VideoElementPriority.AfterCurrent], ref frameEvents, true);
			FillEvents(tracker, ref videoElements[(int)VideoElementPriority.BeforeCurrent], ref frameEvents, false);
			FillEvents(tracker, ref videoElements[(int)VideoElementPriority.AfterOutOfRange], ref frameEvents, true);
			FillEvents(tracker, ref videoElements[(int)VideoElementPriority.BeforeOutOfRange], ref frameEvents, false);

			foreach(var ev in frameEvents)
			{
				plan.RecordElementFrame(ev, videoTimecode);
			}
		}

		private void FillEvents(FrameVacancyTracker tracker, ref List<IVideoElement> inEvents, ref List<IVideoElement> outEvents, bool forwards)
		{
			for (
				var i = (forwards ? 0 : inEvents.Count - 1);
				(forwards ? i < inEvents.Count : i >= 0);
				i += (forwards ? 1 : -1))
			{
				if (!tracker.AddOccupant(inEvents[i]))
				{
					return;
				}

				outEvents.Add(inEvents[i]);
			}
		}
	}
}
