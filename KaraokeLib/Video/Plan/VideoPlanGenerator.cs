using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Plan
{
	public class VideoPlanGenerator
	{
		public static VideoPlan CreateVideoPlan(VideoContext context, VideoSection[] sections)
		{
			var elements = VideoElementGenerator.Generate(context, sections);
			/*var vacancyPlanCreator = new VacancyPlanCreator(context, elements);
			var naivePlan = vacancyPlanCreator.CreatePlan(context.LastFrameTimecode);
			naivePlan.FinalizePlan();*/
			// dead simple "IsVisible" check for plan for now
			var naivePlan = new VideoPlan();
			for(var i = 0; i < context.LastFrameTimecode.FrameNumber; i++)
			{
				var timecode = new VideoTimecode(i, context.Config.FrameRate);
				var seconds = timecode.ToSeconds();
				foreach(var elem in elements)
				{
					if(elem.StartTimecode.GetTimeSeconds() <= seconds && elem.EndTimecode.GetTimeSeconds() > seconds)
					{
						naivePlan.RecordElementFrame(elem, timecode);
					}
				}
			}
			naivePlan.FinalizePlan();
			return naivePlan;
		}
	}
}
