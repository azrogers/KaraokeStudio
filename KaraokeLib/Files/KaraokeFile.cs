namespace KaraokeLib.Files
{
	public interface IKaraokeFile
	{
		IEnumerable<KaraokeTrack> GetTracks();

		double GetLengthSeconds();

		void Save(Stream outStream);
	}

	public interface IKaraokeProvider
	{
		/// <summary>
		/// Obtains the tracks this provider represents.
		/// </summary>
		IEnumerable<KaraokeTrack> GetTracks();

		/// <summary>
		/// Adds a track of the given type to the provider.
		/// </summary>
		KaraokeTrack AddTrack(KaraokeTrackType type);

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

	public class KaraokeFile<T> : IKaraokeFile where T : IKaraokeProvider
	{
		protected T _provider;

		public KaraokeFile(T provider)
		{
			_provider = provider;
		}

		public IEnumerable<KaraokeTrack> GetTracks()
		{
			return _provider.GetTracks();
		}

		/// <summary>
		/// Adds a new track of the given type to the file and returns it.
		/// </summary>
		public KaraokeTrack AddTrack(KaraokeTrackType newTrackType)
		{
			return _provider.AddTrack(newTrackType);
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
