using KaraokeLib.Tracks;

namespace KaraokeLib.Files
{
	/// <summary>
	/// Represents a single format that karaoke data can be read from and written to.
	/// </summary>
	public interface IKaraokeProvider
	{
		/// <summary>
		/// Obtains the tracks this provider represents.
		/// </summary>
		IEnumerable<KaraokeTrack> GetTracks();

		/// <summary>
		/// Adds a track of the given type to the provider.
		/// </summary>
		KaraokeTrack AddTrack(IKaraokeFile file, KaraokeTrackType type);

		/// <summary>
		/// Adds an existing track object to the provider.
		/// The ID of this track and its events must not overlap with any existing IDs.
		/// </summary>
		KaraokeTrack AddTrack(KaraokeTrack track);

		/// <summary>
		/// Removes a track with the given ID.
		/// </summary>
		void RemoveTrack(int trackId);

		/// <summary>
		/// The total duration in seconds of the lyrics content of this provider.
		/// </summary>
		double GetLengthSeconds();

		/// <summary>
		/// Saves this provider to the given stream.
		/// Not every provider supports saving.
		/// </summary>
		void Save(Stream outStream);

		/// <summary>
		/// Does this provider read events from a source?
		/// </summary>
		bool CanRead { get; }
		/// <summary>
		/// Does this provider write events to a destination?
		/// </summary>
		bool CanWrite { get; }
	}
}
