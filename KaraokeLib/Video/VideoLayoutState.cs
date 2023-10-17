using KaraokeLib.Lyrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{

	public class VideoLayoutState
	{
		private HashSet<int> _hyphenatedIds = new HashSet<int>();

		internal void Clear() => _hyphenatedIds.Clear();

		internal void SetHyphenated(LyricsEvent ev) => _hyphenatedIds.Add(ev.Id);

		internal bool IsHyphenated(LyricsEvent ev) => _hyphenatedIds.Contains(ev.Id);
	}
}
