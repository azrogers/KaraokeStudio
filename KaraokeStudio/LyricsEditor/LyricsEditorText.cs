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
		const float NEW_ELEMENT_SIZE = 0.25f;

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
		public static IEnumerable<KaraokeEvent> GetEventsFromString(string text, LyricsEditorTextElement[] oldElementsArr, double length)
		{
			var nextEventId = 0;
			var nextElementId = 0;
			var tokens = LyricsTokenizer.Tokenize(text);
			var oldElements = oldElementsArr.OrderBy(e => e.StartTime).ToArray();
			var newElements = LyricsLexer.Process(tokens).ToArray();

			// we don't have a custom diff algorithm for LyricsEditorTextElement vs LyricsLexerElement -
			// we just diff two arrays of ints computed from a purpose-specific hash
			var oldArr = oldElements.Select(o => o.ToDiffHash()).ToArray() ?? new int[0];
			var newArr = newElements.Select(o => o.ToDiffHash()).ToArray() ?? new int[0];
			var results = Diff.DiffInt(oldArr, newArr).OrderBy(d => d.StartA).ToArray();

			var createdElements = new List<LyricsEditorTextElement>();
			var resultIndex = 0;
			var oldIndex = 0;
			var newIndex = 0;
			while (oldIndex < oldArr.Length || newIndex < newArr.Length)
			{
				Diff.Item? result = results.Length > resultIndex ? results[resultIndex] : null;
				// we have a diff! handle it
				if (result != null && result?.StartA == oldIndex && result?.StartB == newIndex)
				{
					var item = result.Value;

					var minSizeNeeded = NEW_ELEMENT_SIZE * item.insertedB;
					var timeStart = 0.0;
					var timeEnd = length;
					if(oldElements.Length > 0)
					{
						// we deleted items, so we can insert the new items in the bounds of the elements we just deleted
						if (item.deletedA > 0)
						{
							timeStart = oldElements[item.StartA].StartTime;
							var endIndex = item.StartA + item.deletedA;
							timeEnd = endIndex < oldElements.Length ? oldElements[endIndex].StartTime : oldElements[oldElements.Length - 1].EndTime;
						}
						// we're at the start but there's enough room to insert it between the start of the song and the first event,
						// or there's enough room to insert it between the last event and this event
						else if ((item.StartA == 0 && oldElements[item.StartA].StartTime > minSizeNeeded) || (item.StartA > 0 && (oldElements[item.StartA].StartTime - oldElements[item.StartA - 1].EndTime) >= minSizeNeeded))
						{
							timeEnd = oldElements[item.StartA].StartTime;
							timeStart = timeEnd - minSizeNeeded;
						}
						// just kinda throw em in there and let god sort it out
						else
						{
							timeStart = oldElements[item.StartA].StartTime;
							timeEnd = item.StartA + 1 < oldElements.Length ? oldElements[item.StartA + 1].StartTime : timeStart + minSizeNeeded;
						}
					}

					var totalElem = newElements.Skip(item.StartB).Take(item.insertedB);
					var totalLen = totalElem.Select(r => r.Type == KaraokeEventType.Lyric ? r.Tokens.Sum(t => t.Length) : 1).Sum();
					var startPos = 0;
					if(totalLen > 0)
					{
						// handle all the skipped elements
						foreach (var elem in totalElem)
						{
							createdElements.Add(ToTextElement(elem, timeStart, timeEnd, startPos, totalLen, nextEventId: ref nextEventId, nextElementId: ref nextElementId, out var len));
							startPos += len;
						}
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

				createdElements.Add(new LyricsEditorTextElement(nextElementId, oldElem.Type, events));
				oldIndex++;
				newIndex++;
			}

			// turn the array of elements into a stream of events
			var lastEndTime = 0.0;
			KaraokeEvent? lastEvent = null;
			var createdEvents = new List<KaraokeEvent>();
			foreach(var element in createdElements)
			{
				foreach(var ev in element.Events)
				{
					if(lastEvent != null && lastEndTime > ev.StartTimeSeconds)
					{
						// if they overlap but we can move the start time of ev without losing it entirely, let's do that
						if(lastEndTime < ev.EndTimeSeconds)
						{
							ev.SetTiming(lastEvent.EndTime, ev.EndTime);
						}
						// if they overlap but we can move the end time of the last event without losing it entirely, let's do that
						else if(lastEvent.StartTimeSeconds < ev.StartTimeSeconds)
						{
							lastEvent.SetTiming(lastEvent.StartTime, ev.StartTime);
						}
						// otherwise, just move this event to the end of the last one
						else
						{
							ev.SetTiming(lastEvent.EndTime, new TimeSpanTimecode(lastEvent.EndTimeSeconds + ev.LengthSeconds));
						}
					}

					createdEvents.Add(ev);

					lastEvent = ev;
					lastEndTime = ev.EndTimeSeconds;
				}
			}

			return createdEvents;
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
