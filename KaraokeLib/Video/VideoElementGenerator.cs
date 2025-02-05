﻿using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Video.Elements;

namespace KaraokeLib.Video
{
	/// <summary>
	/// Takes in <see cref="VideoParagraph"/>s contained in <see cref="VideoSection"/>s and creates <see cref="IVideoElement"/>s to display.
	/// </summary>
	public class VideoElementGenerator
	{
		/// <summary>
		/// Generates an array of video elements out of an IEnumerable of non-lyrics events.
		/// </summary>
		public static IVideoElement[] GenerateFromEvents(VideoContext context, IEnumerable<KaraokeEvent> events)
		{
			var elements = new List<IVideoElement>();
			foreach (var ev in events)
			{
				switch (ev)
				{
					case ImageKaraokeEvent e:
						elements.Add(new VideoImageElement(context, e));
						break;
				}
			}

			return elements.ToArray();
		}

		public static IVideoElement[] GenerateFromSections(VideoContext context, VideoLayoutState layoutState, VideoSection[] sections)
		{
			var elements = new List<IVideoElement>();
			var lineElements = new Dictionary<int, List<IVideoElement>>();

			var actualNumLines = sections.Any() ? sections.Max(s => s.Paragraphs.Any() ? s.Paragraphs.Max(p => p.Lines.Count()) : 0) : 0;

			var elementId = 0;
			var paragraphId = 0;
			foreach (var section in sections)
			{
				foreach (var para in section.Paragraphs)
				{
					elementId = GenerateParagraph(context, layoutState, para, elementId, paragraphId++, actualNumLines, ref elements, ref lineElements);
				}
			}

			foreach (var (i, line) in lineElements)
			{
				if (!line.Any())
				{
					continue;
				}

				// need to manually handle changing the first element's start time
				{
					var firstElem = line[0];
					var newStartSeconds = Math.Max(0, firstElem.StartTimecode.GetTimeSeconds() - context.Config.LyricLeadTime);
					firstElem.SetTiming(new TimeSpanTimecode(newStartSeconds), firstElem.EndTimecode);
				}

				if (line.Count > 1)
				{
					// try to give each element the configured lead and trail time without overlapping events
					for (var j = 1; j < line.Count; j++)
					{
						var thisElement = line[j];
						var prevElement = line[j - 1];

						var distBetween = thisElement.StartTimecode.GetTimeSeconds() - prevElement.EndTimecode.GetTimeSeconds();
						var newStartSeconds = thisElement.StartTimecode.GetTimeSeconds() - Math.Min(distBetween / 2, context.Config.LyricLeadTime);
						thisElement.SetTiming(new TimeSpanTimecode(newStartSeconds), thisElement.EndTimecode);

						var newEndSeconds = prevElement.EndTimecode.GetTimeSeconds() + Math.Min(distBetween / 2, context.Config.LyricTrailTime);
						prevElement.SetTiming(prevElement.StartTimecode, new TimeSpanTimecode(newEndSeconds));
					}
				}

				// need to manually handle changing the last element's end time
				{
					var lastElem = line[line.Count - 1];
					var newEndSeconds = Math.Min(context.LastFrameTimecode.ToSeconds(), lastElem.EndTimecode.GetTimeSeconds() + context.Config.LyricTrailTime);
					lastElem.SetTiming(lastElem.StartTimecode, new TimeSpanTimecode(newEndSeconds));
				}
			}

			return elements.ToArray();
		}

		private static int GenerateParagraph(
			VideoContext context,
			VideoLayoutState layoutState,
			VideoParagraph paragraph,
			int nextElementId,
			int paragraphId,
			int actualNumLines,
			ref List<IVideoElement> outElements,
			ref Dictionary<int, List<IVideoElement>> outLineElements)
		{
			var startYPos = CalculateYPos(context, actualNumLines);
			var lineHeight = context.Style.LineHeight;

			// first line will get a ++ but we want that index to be 0
			var lineIndex = -1;
			var elementId = nextElementId;

			// create elements out of each line
			foreach (var line in paragraph.Lines)
			{
				lineIndex++;
				if (!line.Any())
				{
					continue;
				}

				var elem = new VideoTextElement(context, layoutState, line, startYPos + lineHeight * lineIndex, elementId++, paragraphId);
				outElements.Add(elem);
				if (!outLineElements.TryGetValue(lineIndex, out var lineElements))
				{
					lineElements = new List<IVideoElement>();
				}

				lineElements.Add(elem);
				outLineElements[lineIndex] = lineElements;
			}

			return elementId;
		}

		private static float CalculateYPos(VideoContext context, int actualNumLines)
		{
			var safeArea = context.Style.GetSafeArea(context.Size);
			var lineHeight = context.Style.LineHeight;
			switch (context.Config.VerticalAlign)
			{
				case VerticalAlignment.Top:
					return safeArea.Top;
				case VerticalAlignment.Bottom:
					return safeArea.Bottom - lineHeight * actualNumLines;
				case VerticalAlignment.Center:
					return (safeArea.Height - lineHeight * actualNumLines) / 2 + safeArea.Top;
				default:
					throw new NotImplementedException($"Unknown enum value {context.Config.VerticalAlign}");
			}
		}
	}
}
