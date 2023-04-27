using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Lyrics.Providers
{
    public interface ILyricsProvider
    {
		IEnumerable<LyricsTrack> GetTracks();

        double GetLengthSeconds();
    }
}
