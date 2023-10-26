using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Lyrics
{
	public class LyricsTrack
	{
		private List<LyricsEvent> _events;

		public List<LyricsEvent> Events => _events;

		public LyricsTrackType Type { get; private set; }

		public LyricsTrack(LyricsTrackType type)
		{
			_events = new List<LyricsEvent>();
			Type = type;
		}

		public LyricsTrack(IEnumerable<LyricsEvent> events)
		{
			_events = new List<LyricsEvent>(events);
		}

		public void AddEvents(IEnumerable<LyricsEvent> events)
		{
			_events.AddRange(events);
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
	}

	public enum LyricsTrackType : byte
	{
		Lyrics = 0x00,
		Graphics = 0x01
	}
}
