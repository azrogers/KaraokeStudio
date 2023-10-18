using KaraokeLib.Lyrics;

namespace KaraokeStudio.LyricsEditor
{
	/// <summary>
	/// Represents a single section of text within the lyrics editor.
	/// This is a single line break, single paragraph break, or a word with one or more syllables.
	/// </summary>
	internal class LyricsEditorTextElement
	{
		public const string NEW_LINE = "\n";

		private const char SYLLABLE_SEPERATOR = '-';
		private const char ESCAPE_CHAR = '\\';
		private const string NEW_LINE_TWICE = NEW_LINE + NEW_LINE;

		/// <summary>
		/// The <see cref="LyricsEventType"/> of the underlying <see cref="LyricsEvent" /> objects.
		/// </summary>
		/// <remarks>
		/// A <see cref="LyricsEditorTextElement"/> contains only a single type of events.
		/// </remarks>
		public LyricsEventType Type { get; private set; }
		/// <summary>
		/// The ID of this element.
		/// </summary>
		public int Id { get; private set; }
		/// <summary>
		/// The time the first event this element represents starts at.
		/// </summary>
		public double StartTime { get; private set; }
		/// <summary>
		/// The time the last event this element represents ends at.
		/// </summary>
		public double EndTime { get; private set; }

		private LyricsEvent[] _events;

		public LyricsEditorTextElement(int id, LyricsEventType type, IEnumerable<LyricsEvent> events)
		{
			Id = id;
			Type = type;
			if (!events.All(e => e.Type == type))
			{
				throw new InvalidDataException("Mismatched event types in LyricsEditorTextElement");
			}

			_events = events.ToArray();
			StartTime = _events.Min(e => e.StartTimeSeconds);
			EndTime = _events.Max(e => e.EndTimeSeconds);
		}

		/// <summary>
		/// Returns the normalized position within this element of the given position in seconds.
		/// </summary>
		public double GetNormalizedPosition(double position) =>
			Math.Clamp((position - StartTime) / (EndTime - StartTime), 0, 1);

		public override string ToString()
		{
			switch (Type)
			{
				case LyricsEventType.LineBreak:
					return NEW_LINE;
				case LyricsEventType.ParagraphBreak:
					return NEW_LINE_TWICE;
				case LyricsEventType.Lyric:
					return string.Join(SYLLABLE_SEPERATOR, _events.Select(e => EscapeStr(e.RawText ?? "")));
				default:
					throw new NotImplementedException($"Unknown event type {Type}");
			}
		}

		private static string EscapeStr(string str)
		{
			return str.Replace(SYLLABLE_SEPERATOR.ToString(), new string(new char[] { ESCAPE_CHAR, SYLLABLE_SEPERATOR }));
		}
	}
}
