namespace KaraokeLib.Video.Plan
{
	public class VideoPlanGenerator
	{
		public static VideoPlan CreateVideoPlan(VideoContext context, VideoLayoutState layoutState, VideoSection[] sections)
		{
			var elements = VideoElementGenerator.Generate(context, layoutState, sections);

			// dead simple "IsVisible" check for plan for now
			var naivePlan = new VideoPlan();
			for (var i = 0; i < context.LastFrameTimecode.FrameNumber; i++)
			{
				var timecode = new VideoTimecode(i, context.Config.FrameRate);
				var seconds = timecode.ToSeconds();
				foreach (var elem in elements)
				{
					if (elem.StartTimecode.GetTimeSeconds() <= seconds && elem.EndTimecode.GetTimeSeconds() > seconds)
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
