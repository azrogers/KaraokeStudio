using KaraokeLib.Config;
using KaraokeLib.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Files.Ksf
{
    [KsfSerializable(KsfObjectType.File)]
	internal class KsfFileObject
	{
		[KsfSerialize]
		public KaraokeConfig Config { get; set; }

		[KsfSerialize]
		public double Length { get; set; }

		[KsfSerialize]
		private List<KaraokeTrack> _tracks = new List<KaraokeTrack>();

		[KsfSerialize]
		private Dictionary<string, string> _metadata = new Dictionary<string, string>();

		public IEnumerable<KaraokeTrack> Tracks => _tracks;

		public void SetMetadata(string key, string value) => _metadata[key] = value;
		public string? GetMetadata(string key) => _metadata.ContainsKey(key) ? _metadata[key] : null;
		public void RemoveMetadata(string key) => _metadata.Remove(key);

		public void SetTracks(IEnumerable<KaraokeTrack> tracks)
		{
			_tracks = tracks.ToList();
		}

		public KsfFileObject(KaraokeConfig config, double lengthSeconds)
		{
			Config = config;
			Length = lengthSeconds;
		}

		// used for serialization
		private KsfFileObject()
		{
			Config = new KaraokeConfig();
			Length = 0;
		}
	}
}
