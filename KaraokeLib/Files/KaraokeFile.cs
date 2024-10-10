using KaraokeLib.Tracks;

namespace KaraokeLib.Files
{
	public interface IKaraokeFile
	{
		/// <summary>
		/// Keeps track of IDs for events and tracks.
		/// </summary>
		FileIdTracker IdTracker { get; }

		/// <summary>
		/// Returns all tracks within this file.
		/// </summary>
		IEnumerable<KaraokeTrack> GetTracks();

		/// <summary>
		/// Gets the length in seconds of the events in this file.
		/// </summary>
		double GetLengthSeconds();

		/// <summary>
		/// Saves this file to the given stream.
		/// </summary>
		/// <remarks>If this provider doesn't support writing, it will throw an <see cref="InvalidOperationException"/>.</remarks>
		/// <param name="outStream">The stream that will be written to. Should support writing.</param>
		/// <exception cref="InvalidOperationException" />
		void Save(Stream outStream);
	}

	public class KaraokeFile<T> : IKaraokeFile where T : IKaraokeProvider
	{
		protected T _provider;

		public FileIdTracker IdTracker { get; private set; } = new FileIdTracker();

		public KaraokeFile(T provider)
		{
			_provider = provider;
			var tracks = _provider.GetTracks().ToArray();
			foreach (var track in tracks)
			{
				track.SetFile(this);
			}

			IdTracker.BuildMaps(tracks);
		}

		/// <inheritdoc />
		public IEnumerable<KaraokeTrack> GetTracks()
		{
			return _provider.GetTracks();
		}

		/// <summary>
		/// Adds a new track of the given type to the file and returns it.
		/// </summary>
		public KaraokeTrack AddTrack(KaraokeTrackType newTrackType)
		{
			return _provider.AddTrack(this, newTrackType);
		}

		/// <summary>
		/// Adds an existing track object to this file.
		/// </summary>
		public KaraokeTrack AddTrack(KaraokeTrack track) => _provider.AddTrack(track);

		/// <summary>
		/// Removes the track with the given ID from this file.
		/// </summary>
		public void RemoveTrack(int trackId)
		{
			_provider.RemoveTrack(trackId);
		}

		/// <inheritdoc />
		public double GetLengthSeconds()
		{
			return _provider.GetLengthSeconds();
		}

		/// <inheritdoc />
		public void Save(Stream outStream)
		{
			if (!_provider.CanWrite)
			{
				throw new InvalidOperationException($"Can't write to a provider of type {_provider.GetType()}");
			}

			_provider.Save(outStream);
		}
	}
}
