using KaraokeLib.Lyrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	internal class VideoParagraph
	{
		private VideoContext _context;
		private LyricsEvent[][] _lines;
		private float[] _lineWidths;
		private int _usedLines = 0;
		private IEventTimecode? _startTimecode;
		private IEventTimecode? _endTimecode;

		public double StartTimeSeconds => _startTimecode?.GetTimeSeconds() ?? 0.0;

		public double EndTimeSeconds => _endTimecode?.GetTimeSeconds() ?? 0.0;

		public VideoParagraph(VideoContext context, int numLines)
		{
			_context = context;
			_lines = new LyricsEvent[numLines][];
			_lineWidths = new float[numLines];
		}

		public IEnumerable<LyricsEvent> GetLineEvents(int lineIndex)
		{
			if (lineIndex < 0 || lineIndex >= _usedLines)
			{
				return new LyricsEvent[0];
			}

			return _lines[lineIndex];
		}

		public int FillParagraph(LyricsEvent[] lyrics, int offset)
		{
			var numEventsConsumed = 0;
			var currentLine = 0;
			var currentLineWidth = 0.0f;
			var currentLineEvents = new List<LyricsEvent>();

			var safeArea = _context.Style.GetSafeArea(_context.Size);

			for (var i = offset; i < lyrics.Length; i++)
			{
				var ev = lyrics[i];

				if (ev.Type == LyricsEventType.ParagraphBreak)
				{
					numEventsConsumed++;
					break;
				}

				if (ev.Type == LyricsEventType.LineBreak && currentLine == 0 && !currentLineEvents.Any())
				{
					// ignore line breaks at the start of paragraphs
					numEventsConsumed++;
					continue;
				}

				// need to reset because we modify state instead of producing separate render state currently
				ev.IsHyphenated = false;

				// include a space if this isn't another syllable and we're not at the start of a new line
				var hasSpace = ev.LinkedId == -1 && currentLineEvents.Any();
				var textToMeasure = (hasSpace ? " " : "") + ev.Text;
				var width = _context.Style.GetTextWidth(textToMeasure);
				if (ev.Type == LyricsEventType.Lyric && (currentLineWidth + width) <= safeArea.Width)
				{
					// we can fit it on the current line
					currentLineEvents.Add(ev);
					currentLineWidth += width;
				}
				else if (currentLineEvents.Any()) // don't create a new line if we don't have any events on this line yet
				{
					// create a new line
					_lines[currentLine] = currentLineEvents.ToArray();
					if (ev.LinkedId != -1)
					{
						// TODO: avoid modifying the LyricsEvent state
						_lines[currentLine].Last().IsHyphenated = true;
					}
					_lineWidths[currentLine] = currentLineWidth;
					currentLine++;

					currentLineEvents.Clear();

					if (currentLine >= _lineWidths.Length)
					{
						// we're out of lines, this paragraph is done
						break;
					}

					if(ev.Type != LyricsEventType.LineBreak)
					{
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
				_lines[currentLine] = currentLineEvents.ToArray();
				_lineWidths[currentLine] = currentLineWidth;
				_usedLines = currentLine + 1;
			}
			else
			{
				_usedLines = currentLine;
			}

			if (_usedLines > 0 && _lines[_usedLines - 1].Length == 0)
			{
				_usedLines--;
			}

			if (_usedLines > 0)
			{
				_startTimecode = _lines[0][0].StartTime;
				_endTimecode = _lines[_usedLines - 1].Last().EndTime;
			}

			return numEventsConsumed;
		}
	}
}
