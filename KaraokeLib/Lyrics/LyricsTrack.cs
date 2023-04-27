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

		public IEnumerable<LyricsEvent> Events => _events;

		public LyricsTrack()
		{
			_events = new List<LyricsEvent>();
		}

		public void AddEvents(IEnumerable<LyricsEvent> events)
		{
			_events.AddRange(events);
			_events = _events.OrderBy(ev => ev.StartTimeMilliseconds).ToList();
		}
	}
}
