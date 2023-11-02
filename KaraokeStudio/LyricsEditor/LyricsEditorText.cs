using KaraokeLib.Events;
using KaraokeStudio.Util;
using Newtonsoft.Json.Linq;
using ScintillaNET;
using System.Text;
using System.Windows.Documents;

namespace KaraokeStudio.LyricsEditor
{
    internal static class LyricsEditorText
	{
		/// <summary>
		/// Creates LyricsEditorTextElement instances from the given stream of events.
		/// </summary>
		public static LyricsEditorTextElement[] CreateElements(IEnumerable<KaraokeEvent> events)
		{
			var elements = new List<LyricsEditorTextElement>();
			var currentLyricEvents = new List<KaraokeEvent>();
			var lastLyricEventId = int.MinValue;
			var nextId = 0;

			foreach (var e in events.OrderBy(ev => ev.StartTimeSeconds))
			{
				if (e.Type == KaraokeEventType.Lyric && e.LinkedId == lastLyricEventId)
				{
					// this is another syllable of the existing word
					currentLyricEvents.Add(e);
					lastLyricEventId = e.Id;
					continue;
				}

				if (currentLyricEvents.Any())
				{
					// we're starting a new element, push the old one
					elements.Add(new LyricsEditorTextElement(nextId++, KaraokeEventType.Lyric, currentLyricEvents));
					currentLyricEvents.Clear();
					lastLyricEventId = int.MinValue;
				}

				if (e.Type == KaraokeEventType.Lyric)
				{
					// this is the first lyric of a new word
					currentLyricEvents.Add(e);
					lastLyricEventId = e.Id;
				}
				else
				{
					elements.Add(new LyricsEditorTextElement(nextId++, e.Type, new KaraokeEvent[] { e }));
				}
			}

			// push the last word
			if (currentLyricEvents.Any())
			{
				elements.Add(new LyricsEditorTextElement(nextId++, KaraokeEventType.Lyric, currentLyricEvents));
			}

			return elements.ToArray();
		}

		/// <summary>
		/// Creates a final string to display out of an array of LyricsEditorTextElement instances.
		/// </summary>
		public static LyricsEditorTextResult CreateString(LyricsEditorTextElement[] elements)
		{
			var offsets = new Dictionary<int, int>();
			var builder = new StringBuilder();
			var startOfLine = true;
			foreach (var e in elements)
			{
				offsets[e.Id] = builder.Length;

				var str = e.ToString();
				// only add a space if there's previous stuff on this line
				if (!startOfLine && !string.IsNullOrWhiteSpace(str))
				{
					builder.Append(' ');
				}
				builder.Append(str);

				startOfLine = str.EndsWith("\n");
			}

			return new LyricsEditorTextResult(builder.ToString(), offsets);
		}

		/// <summary>
		/// Uses old LyricsEditorTextElement instances to fill in missing time information for a new body of text.
		/// </summary>
		public static IEnumerable<LyricsEditorTextElement> UpdateFromString(string text, LyricsEditorTextElement[] oldElements)
		{
			var nextEventId = 0;
			var nextElementId = 0;
			var tokens = LyricsTokenizer.Tokenize(text);
			var newElements = LyricsLexer.Process(tokens).ToArray();

			var oldArr = oldElements.Select(o => o.ToHash()).ToArray() ?? new int[0];
			var newArr = newElements.Select(o => o.ToHash()).ToArray() ?? new int[0];

			var results = Diff.DiffInt(oldArr, newArr).OrderBy(d => d.StartA).ToArray();
			var resultIndex = 0;
			var oldIndex = 0;
			var newIndex = 0;
			while (oldIndex < oldArr.Length)
			{
				Diff.Item? result = results.Length > resultIndex ? results[resultIndex] : null;
				// we have a diff! handle it
				if (result != null && result?.StartA == oldIndex && result?.StartB == newIndex)
				{
					var item = result.Value;

					var timeStart = oldElements[item.StartA].StartTime;
					var timeEnd = oldElements[Math.Min(item.StartA + item.deletedA, oldElements.Length - 1)].EndTime;

					var totalLen = newElements.Skip(item.StartB).Take(item.insertedB).Select(r => r.Tokens.Sum(t => t.Length)).Sum() + item.insertedB;
					var startPos = 0;
					// handle all the skipped elements
					foreach (var elem in newElements.Skip(item.StartB).Take(item.insertedB))
					{
						yield return ToTextElement(elem, timeStart, timeEnd, startPos, totalLen, ref nextEventId, ref nextElementId, out var len);
						startPos += len;
					}

					oldIndex += item.deletedA;
					newIndex += item.insertedB;
					resultIndex++;

					continue;
				}

				var oldElem = oldElements[oldIndex];
				var events = new List<KaraokeEvent>(oldElem.Events.Length);
				var lastId = -1;
				foreach (var ev in oldElem.Events)
				{
					events.Add(new KaraokeEvent(ev.Type, nextEventId, ev.StartTime, ev.EndTime, lastId) { RawValue = ev.RawValue });
					lastId = nextEventId++;
				}

				yield return new LyricsEditorTextElement(nextElementId, oldElem.Type, events);
				oldIndex++;
				newIndex++;
			}
		}

		private static LyricsEditorTextElement ToTextElement(
			LyricsLexerElement elem,
			double timeStart,
			double timeEnd,
			int startPos,
			int totalLen,
			ref int nextEventId,
			ref int nextElementId,
			out int len)
		{
			var pos = startPos;

			if (elem.Type == KaraokeEventType.Lyric)
			{
				var events = new List<KaraokeEvent>();
				var lastId = -1;
				foreach (var token in elem.Tokens)
				{
					var startTime = Utility.Lerp(timeStart, timeEnd, pos / (double)totalLen);
					pos += token.Length;
					var endTime = Utility.Lerp(timeStart, timeEnd, pos / (double)totalLen);

					events.Add(new KaraokeEvent(KaraokeEventType.Lyric, nextEventId, new TimeSpanTimecode(startTime), new TimeSpanTimecode(endTime), lastId) { RawValue = token });

					lastId = nextEventId++;
				}

				// add char for space
				pos++;

				len = pos - startPos;
				return new LyricsEditorTextElement(nextElementId++, KaraokeEventType.Lyric, events);
			}
			else
			{
				var startTime = Utility.Lerp(timeStart, timeEnd, pos / (double)totalLen);
				pos++;
				var endTime = Utility.Lerp(timeStart, timeEnd, pos / (double)totalLen);

				len = pos - startPos;
				return new LyricsEditorTextElement(
					nextElementId++, 
					elem.Type, 
					new KaraokeEvent[] {
						new KaraokeEvent(elem.Type, nextEventId++, new TimeSpanTimecode(startTime), new TimeSpanTimecode(endTime)) 
					});
			}
		}
	}

	public class LyricsEditorTextResult
	{
		public string Text;
		public Dictionary<int, int> EventOffsets;

		public LyricsEditorTextResult(string text, Dictionary<int, int> eventOffsets)
		{
			Text = text;
			EventOffsets = eventOffsets;
		}

		internal int PositionToCharIndex(IEnumerable<LyricsEditorTextElement> textElements, double position)
		{
			foreach (var elem in textElements.OrderBy(e => e.StartTime))
			{
				if (position < elem.StartTime)
				{
					return EventOffsets[elem.Id];
				}
				else if (position >= elem.StartTime && position < elem.EndTime)
				{
					var start = EventOffsets[elem.Id];
					var end = start + elem.ToString().Length;
					var normalizedPos = elem.GetNormalizedPosition(position);
					return (int)((end - start) * normalizedPos);
				}
			}

			return Text.Length - 1;
		}
	}
}
