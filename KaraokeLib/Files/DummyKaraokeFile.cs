using KaraokeLib.Tracks;

namespace KaraokeLib.Files
{

    public class DummyKaraokeProvider : IKaraokeProvider
	{
		public bool CanRead => true;

		public bool CanWrite => false;

		private List<KaraokeTrack> _tracks = new List<KaraokeTrack>();

		public KaraokeTrack AddTrack(IKaraokeFile file, KaraokeTrackType type)
		{
			var track = new KaraokeTrack(file, type);
			_tracks.Add(track);
			return track;
		}

		public void RemoveTrack(int trackId)
		{
			_tracks.RemoveAll(t => t.Id == trackId);
		}

		public double GetLengthSeconds()
		{
			return _tracks.Any() ? _tracks.Max(t => t.Events.Any() ? t.Events.Max(t => t.EndTimeSeconds) : 0) : 0;
		}

		public IEnumerable<KaraokeTrack> GetTracks()
		{
			return _tracks;
		}

		public void Save(Stream outStream)
		{
			throw new NotImplementedException();
		}
	}

	public class DummyKaraokeFile : KaraokeFile<DummyKaraokeProvider>
	{
		public DummyKaraokeFile() : base(new DummyKaraokeProvider()) { }
	}
}
