using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaraokeLib.Lyrics.Providers;

namespace KaraokeLib.Lyrics
{
	public interface ILyricsFile
	{
		IEnumerable<LyricsTrack> GetTracks();

		double GetLengthSeconds();
	}

    public class LyricsFile<T> : ILyricsFile where T : ILyricsProvider
	{
		private T _provider;
		private LyricsTrack[] _tracks;

		public LyricsFile(T provider)
		{
			_provider = provider;
			_tracks = _provider.GetTracks().ToArray();
		}

		public IEnumerable<LyricsTrack> GetTracks()
		{
			return _tracks;
		}

		public double GetLengthSeconds()
		{
			return _provider.GetLengthSeconds();
		}
	}

	public class MidiLyricsFile : LyricsFile<MidiLyricsProvider>
	{
		public MidiLyricsFile(string filename)
			: base(new MidiLyricsProvider(filename)) { }

		public MidiLyricsFile(Stream stream)
			: base(new MidiLyricsProvider(stream)) { }
	}
}
