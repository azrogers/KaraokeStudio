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

		void Save(Stream outStream);
	}

    public class LyricsFile<T> : ILyricsFile where T : ILyricsProvider
	{
		protected T _provider;
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

		public void Save(Stream outStream)
		{
			_provider.Save(outStream);
		}
	}

	public class MidiLyricsFile : LyricsFile<MidiLyricsProvider>
	{
		public MidiLyricsFile(string filename)
			: base(new MidiLyricsProvider(filename)) { }

		public MidiLyricsFile(Stream stream)
			: base(new MidiLyricsProvider(stream)) { }
	}

	public class LrcLyricsFile : LyricsFile<LrcLyricsProvider>
	{
		public LrcLyricsFile(string filename) : base(new LrcLyricsProvider(filename)) { }

		public LrcLyricsFile(Stream stream) : base(new LrcLyricsProvider(stream)) { }
		public LrcLyricsFile(ILyricsFile otherFile) : base(new LrcLyricsProvider(otherFile.GetTracks())) { }
		public LrcLyricsFile(IEnumerable<LyricsTrack> tracks) : base(new LrcLyricsProvider(tracks)) { }
	}

	public class KsfLyricsFile : LyricsFile<KsfLyricsProvider>
	{
		public void SetMetadata(string key, string value) => _provider.SetMetadata(key, value);
		public string? GetMetadata(string key) => _provider.GetMetadata(key);
		public void RemoveMetadata(string key) => _provider.RemoveMetadata(key);

		public KsfLyricsFile(string filename)
			: base(new KsfLyricsProvider(filename))
		{

		}

		public KsfLyricsFile(Stream stream)
			: base(new KsfLyricsProvider(stream))
		{

		}

		public KsfLyricsFile(ILyricsFile otherFile)
			: base(new KsfLyricsProvider(otherFile.GetTracks()))
		{

		}

		public KsfLyricsFile()
			: base(new KsfLyricsProvider(Enumerable.Empty<LyricsTrack>()))
		{

		}
	}
}
