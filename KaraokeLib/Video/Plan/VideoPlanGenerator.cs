using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Plan
{
	internal class VideoPlanGenerator
	{
		public static VideoPlan CreateVideoPlan(VideoContext context, VideoSection[] sections, VideoTimecode lastFrameTimecode)
		{
			var elements = VideoElementGenerator.Generate(context, sections);
			var vacancyPlanCreator = new VacancyPlanCreator(context, elements);
			var naivePlan = vacancyPlanCreator.CreatePlan(lastFrameTimecode);
			naivePlan.FinalizePlan();
			return naivePlan;
		}
	}
}
