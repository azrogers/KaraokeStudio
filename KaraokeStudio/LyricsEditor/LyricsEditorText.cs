using KaraokeLib.Lyrics;
using System.Text;

namespace KaraokeStudio.LyricsEditor
{
	internal static class LyricsEditorText
	{
		public static LyricsEditorTextElement[] CreateElements(IEnumerable<LyricsEvent> events)
		{
			var elements = new List<LyricsEditorTextElement>();
			var currentLyricEvents = new List<LyricsEvent>();
			var lastLyricEventId = int.MinValue;
			var nextId = 0;

			foreach (var e in events)
			{
				if (e.Type == LyricsEventType.Lyric && e.LinkedId == lastLyricEventId)
				{
					// this is another syllable of the existing word
					currentLyricEvents.Add(e);
					lastLyricEventId = e.Id;
					continue;
				}

				if (currentLyricEvents.Any())
				{
					// we're starting a new element, push the old one
					elements.Add(new LyricsEditorTextElement(nextId++, LyricsEventType.Lyric, currentLyricEvents));
					currentLyricEvents.Clear();
					lastLyricEventId = int.MinValue;
				}

				if (e.Type == LyricsEventType.Lyric)
				{
					// this is the first lyric of a new word
					currentLyricEvents.Add(e);
					lastLyricEventId = e.Id;
				}
				else
				{
					elements.Add(new LyricsEditorTextElement(nextId++, e.Type, new LyricsEvent[] { e }));
				}
			}

			// push the last word
			if (currentLyricEvents.Any())
			{
				elements.Add(new LyricsEditorTextElement(nextId++, LyricsEventType.Lyric, currentLyricEvents));
			}

			return elements.ToArray();
		}

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

				startOfLine = str.EndsWith(LyricsEditorTextElement.NEW_LINE);
			}

			return new LyricsEditorTextResult(builder.ToString(), offsets);
		}

		public static LyricsEditorTextElement[] UpdateFromString(string text, ref LyricsEditorTextElement[] previousElements)
		{
			return new LyricsEditorTextElement[0];
			var prevPtr = 0;
			var reader = new StringReader(text);
			var next = -1;
			while((next = reader.Read()) != -1)
			{
				  
			}
		}
	}

	public record LyricsEditorTextResult(string Text, Dictionary<int, int> EventOffsets);
}
