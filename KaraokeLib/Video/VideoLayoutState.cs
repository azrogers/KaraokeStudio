using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaraokeLib.Events;

namespace KaraokeLib.Video
{

    public class VideoLayoutState
	{
		private HashSet<int> _hyphenatedIds = new HashSet<int>();

		public void Clear() => _hyphenatedIds.Clear();

		public void SetHyphenated(KaraokeEvent ev) => _hyphenatedIds.Add(ev.Id);

		public bool IsHyphenated(KaraokeEvent ev) => _hyphenatedIds.Contains(ev.Id);
	}
}
