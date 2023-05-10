using KaraokeLib.Lyrics;
using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	/// <summary>
	/// Takes in <see cref="VideoParagraph"/>s contained in <see cref="VideoSection"/>s and creates <see cref="IVideoElement"/>s to display.
	/// </summary>
	internal class VideoElementGenerator
	{
		public static IVideoElement[] Generate(VideoContext context, VideoSection[] sections)
		{
			var elements = new List<IVideoElement>();
			var lineElements = new List<IVideoElement>[context.NumLines];
			for(var i = 0; i < context.NumLines; i++)
			{
				lineElements[i] = new List<IVideoElement>();
			}

			foreach (var section in sections)
			{
				foreach(var para in section.Paragraphs)
				{
					GenerateParagraph(context, para, ref elements, ref lineElements);
				}
			}

			for(var i = 0; i < context.NumLines; i++)
			{
				var line = lineElements[i];
				if(!line.Any())
				{
					continue;
				}

				// need to manually handle changing the first element's start time
				{
					var firstElem = line[0];
					var newStartSeconds = Math.Max(0, firstElem.StartTimecode.GetTimeSeconds() - context.Config.LyricLeadTime);
					firstElem.StartTimecode = new TimeSpanTimecode(TimeSpan.FromSeconds(newStartSeconds));
				}

				if(line.Count > 1)
				{
					// try to give each element the configured lead and trail time without overlapping events
					for (var j = 1; j < line.Count; j++)
					{
						var thisElement = line[j];
						var prevElement = line[j - 1];

						var distBetween = thisElement.StartTimecode.GetTimeSeconds() - prevElement.EndTimecode.GetTimeSeconds();
						var newStartSeconds = thisElement.StartTimecode.GetTimeSeconds() - Math.Min(distBetween / 2, context.Config.LyricLeadTime);
						thisElement.StartTimecode = new TimeSpanTimecode(TimeSpan.FromSeconds(newStartSeconds));

						var newEndSeconds = prevElement.EndTimecode.GetTimeSeconds() + Math.Min(distBetween / 2, context.Config.LyricTrailTime);
						prevElement.EndTimecode = new TimeSpanTimecode(TimeSpan.FromSeconds(newEndSeconds));
					}
				}

				// need to manually handle changing the last element's end time
				{
					var lastElem = line[line.Count - 1];
					var newEndSeconds = Math.Min(context.LastFrameTimecode.ToSeconds(), lastElem.EndTimecode.GetTimeSeconds() + context.Config.LyricTrailTime);
					lastElem.EndTimecode = new TimeSpanTimecode(TimeSpan.FromSeconds(newEndSeconds));
				}
			}

			return elements.ToArray();
		}

		private static void GenerateParagraph(
			VideoContext context, 
			VideoParagraph paragraph, 
			ref List<IVideoElement> outElements,
			ref List<IVideoElement>[] outLineElements)
		{
			var safeArea = context.Style.GetSafeArea(context.Size);
			var lineHeight = context.Style.LineHeight;
			var startYPos = (safeArea.Height - lineHeight * context.NumLines) / 2 + safeArea.Top;

			// create elements out of each line
			for (var i = 0; i < context.NumLines; i++)
			{
				var events = paragraph.GetLineEvents(i).ToArray();
				if(!events.Any())
				{
					continue;
				}

				var elem = new VideoTextElement(context, events, startYPos + lineHeight * i);
				outElements.Add(elem);
				outLineElements[i].Add(elem);
			}
		}
	}
}
