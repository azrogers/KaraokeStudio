using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Lyrics.Providers
{
    public interface ILyricsProvider
    {
        /// <summary>
        /// Obtains the tracks this provider represents.
        /// </summary>
		IEnumerable<LyricsTrack> GetTracks();

        /// <summary>
        /// The total duration in seconds of the lyrics content of this provider.
        /// </summary>
        double GetLengthSeconds();

        /// <summary>
        /// Saves this provider to the given stream.
        /// Not every provider supports saving.
        /// </summary>
        void Save(Stream outStream);
    }
}
