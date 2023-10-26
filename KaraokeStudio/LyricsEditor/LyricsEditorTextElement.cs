using KaraokeLib.Lyrics;
using System.Text;

namespace KaraokeStudio.LyricsEditor
{
	/// <summary>
	/// Represents a single section of text within the lyrics editor.
	/// This is a single line break, single paragraph break, or a word with one or more syllables.
	/// </summary>
	internal class LyricsEditorTextElement
	{
		private static readonly FastHashes.XxHash32 Hash = new FastHashes.XxHash32();

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

		public LyricsEvent[] Events => _events;

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

		public double CharIndexToPosition(int index)
		{
			var currentIndex = 0;

			if(index == 0)
			{
				return StartTime;
			}

			foreach(var ev in _events)
			{
				var len = 0;
				if(ev.Type == LyricsEventType.LineBreak)
				{
					len = 1;
				}
				else if(ev.Type == LyricsEventType.ParagraphBreak)
				{
					len = 2;
				}
				else if (ev.Type == LyricsEventType.Lyric)
				{
					len = ev.RawText?.Length ?? 0;
				}

				if(index >= currentIndex && index < currentIndex + len)
				{
					var normalizedPos = (index - currentIndex) / (double)len;
					return Util.Lerp(ev.StartTimeSeconds, ev.EndTimeSeconds, normalizedPos);
				}
			}

			return EndTime;
		}

		public override string ToString()
		{
			switch (Type)
			{
				case LyricsEventType.LineBreak:
					return "\n";
				case LyricsEventType.ParagraphBreak:
					return "\n\n";
				case LyricsEventType.Lyric:
					return string.Join(LyricsConstants.SYLLABLE_SEPERATOR, _events.Select(e => EscapeStr(e.RawText ?? "")));
				default:
					throw new NotImplementedException($"Unknown event type {Type}");
			}
		}

		public int ToHash()
		{
			var bytes = new List<byte>
			{
				(byte)Type
			};

			if (Type == LyricsEventType.Lyric)
			{
				bytes.AddRange(BitConverter.GetBytes(Events.Length));
				foreach(var ev in Events)
				{
					bytes.AddRange(Encoding.UTF8.GetBytes(ev.RawText ?? ""));
				}
			}

			return BitConverter.ToInt32(Hash.ComputeHash(bytes.ToArray()));
		}

		private static string EscapeStr(string str)
		{
			return str.Replace(LyricsConstants.SYLLABLE_SEPERATOR.ToString(), new string(new char[] { LyricsConstants.ESCAPE_CHAR, LyricsConstants.SYLLABLE_SEPERATOR }));
		}
	}
}
