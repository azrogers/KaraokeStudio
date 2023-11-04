using KaraokeLib.Events;

namespace KaraokeLib.Video
{
    public class VideoParagraph
	{
		private VideoContext _context;
		private KaraokeEvent[][] _lines;
		private float[] _lineWidths;
		private int _usedLines = 0;
		private IEventTimecode? _startTimecode;
		private IEventTimecode? _endTimecode;

		public double StartTimeSeconds => _startTimecode?.GetTimeSeconds() ?? 0.0;

		public double EndTimeSeconds => _endTimecode?.GetTimeSeconds() ?? 0.0;

		public IEnumerable<KaraokeEvent[]> Lines => _lines;

		public VideoParagraph(VideoContext context, IEnumerable<KaraokeEvent[]> lines, IEnumerable<float> lineWidths)
		{
			_context = context;
			_lines = lines.ToArray();
			_lineWidths = lineWidths.ToArray();
			_usedLines = lines.Count();
		}

		public IEnumerable<KaraokeEvent> GetLineEvents(int lineIndex)
		{
			if (lineIndex < 0 || lineIndex >= _usedLines)
			{
				return new KaraokeEvent[0];
			}

			return _lines[lineIndex];
		}

		public static VideoParagraph[] CreateParagraphs(VideoContext context, KaraokeEvent[] lyrics, VideoLayoutState layoutState)
		{
			var newParagraphs = new List<VideoParagraph>();
			var i = 0;

			while (i < lyrics.Length)
			{
				var para = FillParagraph(context, lyrics, i, layoutState, layoutState.NumLines, out var numConsumed);

				if (numConsumed <= 0)
				{
					throw new InvalidDataException("Paragraph took zero events?");
				}

				i += numConsumed;
				newParagraphs.Add(para);
			}

			return newParagraphs.ToArray();
		}

		/// <summary>
		/// Creates a new paragraph from the input lyrics, taking as many events as it takes to fill up a page.
		/// </summary>
		/// <param name="offset">The index into the events array to start from.</param>
		/// <param name="numLines">How many lines to allow in a paragraph. If less than 1, lines are unlimited.</param>
		/// <param name="numEventsConsumed">Returns how many events were included in this paragraph.</param>
		private static VideoParagraph FillParagraph(
			VideoContext context,
			KaraokeEvent[] lyrics,
			int offset,
			VideoLayoutState layoutState,
			int numLines,
			out int numEventsConsumed)
		{
			numEventsConsumed = 0;
			var currentLine = 0;
			var lines = new List<KaraokeEvent[]>();
			var lineWidths = new List<float>();
			var currentLineWidth = 0.0f;
			var currentLineEvents = new List<KaraokeEvent>();

			var safeArea = context.Style.GetSafeArea(context.Size);

			for (var i = offset; i < lyrics.Length; i++)
			{
				var ev = lyrics[i];

				if (ev.Type == KaraokeEventType.ParagraphBreak)
				{
					numEventsConsumed++;
					break;
				}

				if (ev.Type == KaraokeEventType.LineBreak && currentLine == 0 && !currentLineEvents.Any())
				{
					// ignore line breaks at the start of paragraphs
					numEventsConsumed++;
					continue;
				}

				// include a space if this isn't another syllable and we're not at the start of a new line
				var hasSpace = ev.LinkedId == -1 && currentLineEvents.Any();
				var textToMeasure = (hasSpace ? " " : "") + ev.GetText(layoutState);
				var width = context.Style.GetTextWidth(textToMeasure);
				if (ev.Type == KaraokeEventType.Lyric && (currentLineWidth + width) <= safeArea.Width)
				{
					// we can fit it on the current line
					currentLineEvents.Add(ev);
					currentLineWidth += width;
				}
				else if (currentLineEvents.Any()) // don't create a new line if we don't have any events on this line yet
				{
					// create a new line
					var line = currentLineEvents.ToArray();
					lines.Add(line);
					if (ev.LinkedId != -1)
					{
						layoutState.SetHyphenated(line.Last());
					}
					lineWidths.Add(currentLineWidth);
					currentLine++;

					currentLineEvents.Clear();

					if (currentLine >= numLines && numLines > 0)
					{
						// we're out of lines, this paragraph is done
						break;
					}

					if (ev.Type != KaraokeEventType.LineBreak)
					{
						// get this event recorded in the new line if we have an event worth recording
						currentLineWidth = width;
						currentLineEvents.Add(ev);
					}
					else
					{
						currentLineWidth = 0;
					}
				}

				numEventsConsumed++;
			}

			// we have some leftover line events
			if (currentLineEvents.Any())
			{
				lines.Add(currentLineEvents.ToArray());
				lineWidths.Add(currentLineWidth);
			}

			return new VideoParagraph(context, lines, lineWidths);
		}
	}
}
