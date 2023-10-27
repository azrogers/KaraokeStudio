namespace KaraokeLib.Lyrics
{
	/// <summary>
	/// A single track within a LyricsFile. 
	/// A track can be more than just text - see <see cref="LyricsTrackType"/>.
	/// </summary>
	public class LyricsTrack
	{
		private static readonly Dictionary<LyricsTrackType, HashSet<LyricsEventType>> ValidEvents = new Dictionary<LyricsTrackType, HashSet<LyricsEventType>>()
		{
			{ LyricsTrackType.Lyrics, new HashSet<LyricsEventType>() { LyricsEventType.Lyric, LyricsEventType.ParagraphBreak, LyricsEventType.LineBreak } },
			{ LyricsTrackType.Audio, new HashSet<LyricsEventType>() { LyricsEventType.AudioClip } },
			{ LyricsTrackType.Graphics, new HashSet<LyricsEventType>() { } }
		};

		private List<LyricsEvent> _events;

		/// <summary>
		/// The events on this track.
		/// </summary>
		public List<LyricsEvent> Events => _events;

		/// <summary>
		/// The type of this track.
		/// </summary>
		public LyricsTrackType Type { get; private set; }

		/// <summary>
		/// The id used to uniquely identify this track.
		/// </summary>
		public int Id { get; private set; }

		public LyricsTrack(int id, LyricsTrackType type)
		{
			_events = new List<LyricsEvent>();
			Type = type;
			Id = id;
		}

		public LyricsTrack(int id, LyricsTrackType type, IEnumerable<LyricsEvent> events)
		{
			_events = new List<LyricsEvent>(events);
			Id = id;
		}

		public void AddEvents(IEnumerable<LyricsEvent> events)
		{
			_events.AddRange(events);
			ValidateEvents();
			ConformEvents();
		}

		public void ReplaceEvents(IEnumerable<LyricsEvent> events)
		{
			_events.Clear();
			AddEvents(events);
		}

		private void ConformEvents()
		{
			_events = _events.OrderBy(ev => ev.StartTimeMilliseconds).ToList();
			if(_events.Count < 2)
			{
				// nothing to do
				return;
			}

			// fix overlapping events
			for (var i = 1; i < _events.Count; i++)
			{
				if (_events[i].StartTimeSeconds < _events[i - 1].EndTimeSeconds)
				{
					_events[i - 1].SetTiming(_events[i - 1].StartTime, new TimeSpanTimecode(_events[i].StartTimeSeconds));
				}
			}
		}

		private void ValidateEvents()
		{
			var validEvents = ValidEvents[Type];
			if(validEvents == null)
			{
				throw new NotSupportedException($"Missing valid events for {Type}");
			}

			foreach(var ev in _events)
			{
				if(!validEvents.Contains(ev.Type))
				{
					throw new InvalidDataException($"Can't have event of type {ev.Type} on track of type {Type}!");
				}
			}
		}
	}

	public enum LyricsTrackType : byte
	{
		Lyrics = 0x00,
		Graphics = 0x01,
		Audio = 0x02
	}
}
