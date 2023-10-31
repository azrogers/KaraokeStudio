using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files.Ksf;

namespace KaraokeLib.Files
{
	/// <summary>
	/// Represents a single .ksf file.
	/// </summary>
	public class KsfKaraokeFile : KaraokeFile<KsfKaraokeProvider>
	{
		/// <summary>
		/// Sets the given key-value metadata on the file.
		/// </summary>
		public void SetMetadata(string key, string value) => _provider.FileObject.SetMetadata(key, value);
		/// <summary>
		/// Obtains the value of metadata for the given key, if present.
		/// </summary>
		public string? GetMetadata(string key) => _provider.FileObject.GetMetadata(key);
		/// <summary>
		/// Removes metadata with the given key, if present.
		/// </summary>
		public void RemoveMetadata(string key) => _provider.FileObject.RemoveMetadata(key);

		public KaraokeConfig Config
		{
			get => _provider.FileObject.Config;
			set => _provider.FileObject.Config = value;
		}

		public double LengthSeconds
		{
			get => _provider.FileObject.Length;
			set => _provider.FileObject.Length = value;
		}

		public KsfKaraokeFile(string filename)
			: base(new KsfKaraokeProvider(filename))
		{
		}

		public KsfKaraokeFile(Stream stream)
			: base(new KsfKaraokeProvider(stream))
		{

		}

		public KsfKaraokeFile(IKaraokeFile otherFile)
			: base(new KsfKaraokeProvider(otherFile.GetTracks()))
		{

		}

		public KsfKaraokeFile()
			: base(new KsfKaraokeProvider(Enumerable.Empty<KaraokeTrack>()))
		{

		}
	}

	/// <summary>
	/// Reads and writes .ksf (Karaoke Studio File) format
	/// </summary>
	public class KsfKaraokeProvider : IKaraokeProvider
	{
		internal KsfFileObject FileObject { get; private set; }

		/// <inheritdoc />
		public bool CanRead => true;

		/// <inheritdoc />
		public bool CanWrite => true;

		public KsfKaraokeProvider(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				FileObject = Load(stream);
			}
		}

		public KsfKaraokeProvider(Stream stream)
		{
			FileObject = Load(stream);
		}

		public KsfKaraokeProvider(IEnumerable<KaraokeTrack> tracks)
		{
			var length = tracks.Any() ? tracks.Max(t => t.Events.Any() ? t.Events.Max(t => t.EndTimeSeconds) : 0) : 0;
			FileObject = new KsfFileObject(new Config.KaraokeConfig(), length);
			FileObject.SetTracks(tracks);
		}

		/// <inheritdoc/>
		public IEnumerable<KaraokeTrack> GetTracks() => FileObject.Tracks;

		/// <inheritdoc/>
		public double GetLengthSeconds() => FileObject.Length;

		/// <inheritdoc />
		public KaraokeTrack AddTrack(IKaraokeFile file, KaraokeTrackType type)
		{
			var track = new KaraokeTrack(file, type);
			FileObject.SetTracks(FileObject.Tracks.Concat(new KaraokeTrack[] { track }));
			return track;
		}

		/// <inheritdoc/>
		public void Save(Stream outStream)
		{
			var serializer = new KsfSerializer(outStream);
			serializer.Write(FileObject);
		}

		private KsfFileObject Load(Stream stream)
		{
			var serializer = new KsfSerializer(stream);
			return serializer.Read();
		}
	}
}
