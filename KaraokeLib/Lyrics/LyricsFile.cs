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
		private List<LyricsTrack> _tracks;

		public LyricsFile(T provider)
		{
			_provider = provider;
			_tracks = _provider.GetTracks().ToList();
		}

		public IEnumerable<LyricsTrack> GetTracks()
		{
			return _tracks;
		}

		/// <summary>
		/// Adds a new track of the given type to the file and returns it.
		/// </summary>
		public LyricsTrack AddTrack(LyricsTrackType newTrackType)
		{
			var nextId = _tracks.Any() ? _tracks.Select(t => t.Id).Max() + 1 : 0;
			var track = new LyricsTrack(nextId, newTrackType);
			_tracks.Add(track);
			return track;
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
}
