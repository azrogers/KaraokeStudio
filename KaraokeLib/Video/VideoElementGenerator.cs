using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	internal class VideoElementGenerator
	{
		public static IVideoElement[] Generate(VideoContext context, VideoSection[] sections)
		{
			var elements = new List<IVideoElement>();
			foreach(var section in sections)
			{
				foreach(var para in section.Paragraphs)
				{
					GenerateParagraph(context, para, ref elements);
				}
			}

			return elements.ToArray();
		}

		private static void GenerateParagraph(VideoContext context, VideoParagraph paragraph, ref List<IVideoElement> outElements)
		{
			var safeArea = context.Style.GetSafeArea(context.Size);
			var lineHeight = context.Style.LineHeight;
			var startYPos = (safeArea.Height - lineHeight * context.NumLines) / 2 + safeArea.Top;

			for (var i = 0; i < context.NumLines; i++)
			{
				var events = paragraph.GetLineEvents(i).ToArray();
				if(!events.Any())
				{
					continue;
				}

				var totalWidth = 0f;
				var lineElements = new List<VideoTextElement>();

				for (var j = 0; j < events.Length; j++)
				{
					var text = events[j].Text;
					if(j != 0 && events[j].LinkedId == -1)
					{
						text = " " + text;
					}

					var element = new VideoTextElement(context, text, events[j].StartTime, events[j].EndTime);
					lineElements.Add(element);
					totalWidth += element.Width;
				}

				var textXPos = safeArea.Left + safeArea.Width / 2 - totalWidth / 2;
				var textYPos = startYPos + lineHeight * i;
				foreach(var elem in lineElements)
				{
					elem.Position = (textXPos, textYPos);
					textXPos += elem.Width;
				}

				outElements.AddRange(lineElements);
			}
		}
	}
}
