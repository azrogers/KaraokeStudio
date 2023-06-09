﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace KaraokeLib.Lyrics.Providers
{
	/// <summary>
	/// Reads and writes .ksf (Karaoke Studio File) format
	/// </summary>
	public class KsfLyricsProvider : ILyricsProvider
	{
		// = "KSPF"
		private const uint MAGIC_NUMBER = 0x4650534B;
		private const byte VERSION = 0;

		private Dictionary<string, string> _metadata = new Dictionary<string, string>();

		private LyricsTrack[] _tracks;

		public KsfLyricsProvider(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				_tracks = new LyricsTrack[0];
				Load(stream);
			}
		}

		public KsfLyricsProvider(Stream stream)
		{
			_tracks = new LyricsTrack[0];
			Load(stream);
		}

		public KsfLyricsProvider(IEnumerable<LyricsTrack> tracks)
		{
			_tracks = tracks.ToArray();
		}

		public void SetMetadata(string key, string value) => _metadata[key] = value;
		public string? GetMetadata(string key) => _metadata.ContainsKey(key) ? _metadata[key] : null;
		public void RemoveMetadata(string key) => _metadata.Remove(key);

		public IEnumerable<LyricsTrack> GetTracks()
		{
			return _tracks;
		}

		public double GetLengthSeconds()
		{
			return _tracks.Max(t => t.Events.Max(e => e.EndTimeSeconds));
		}

		/// <summary>
		/// Saves this provider to the given stream.
		/// </summary>
		public void Save(Stream outStream)
		{
			var writer = new BinaryWriter(outStream);

			writer.Write(MAGIC_NUMBER);
			writer.Write(VERSION);
			// write metadata
			writer.Write((ushort)_metadata.Count);
			foreach(var kv in _metadata)
			{
				writer.WriteNullTerminatedString(kv.Key);
				writer.WriteNullTerminatedString(kv.Value);
			}

			// write tracks
			writer.Write((byte)_tracks.Length);
			foreach(var track in _tracks)
			{
				writer.Write((uint)track.Events.Count());
				foreach(var ev in track.Events)
				{
					// 0 = no optional fields, 1 = no text, 2 = no linked id, 3 = both
					if(ev.LinkedId == -1 && ev.Text == null)
					{
						writer.Write((byte)0);
					}
					else if(ev.LinkedId != -1 && ev.Text == null)
					{
						writer.Write((byte)1);
					}
					else if(ev.LinkedId == -1 && ev.Text != null)
					{
						writer.Write((byte)2);
					}
					else
					{
						writer.Write((byte)3);
					}

					// write event
					writer.Write((byte)ev.Type);
					writer.Write(ev.Id);
					writer.Write((uint)ev.StartTimeMilliseconds);
					writer.Write((uint)ev.EndTimeMilliseconds);
					if(ev.LinkedId != -1)
					{
						writer.Write(ev.LinkedId);
					}
					if(ev.RawText != null)
					{
						writer.WriteNullTerminatedString(ev.RawText);
					}
				}
			}
		}

		private void Load(Stream stream)
		{
			var reader = new BinaryReader(stream);

			var magic = reader.ReadUInt32();
			if(magic != MAGIC_NUMBER)
			{
				throw new InvalidDataException($"Unknown magic number {magic:X}");
			}

			var version = reader.ReadByte();
			if(version > VERSION)
			{
				throw new NotImplementedException($"Can't read format version {version}");
			}
			else if(version < VERSION)
			{
				throw new NotImplementedException($"No conversion from format version {version} implemented");
			}

			var metadataCount = reader.ReadUInt16();
			_metadata.EnsureCapacity(metadataCount);
			for(var i = 0; i < metadataCount; i++)
			{
				var key = reader.ReadNullTerminatedString();
				_metadata[key] = reader.ReadNullTerminatedString();
			}

			var trackCount = reader.ReadByte();
			_tracks = new LyricsTrack[trackCount];
			for(var i = 0; i < trackCount; i++)
			{
				var events = new List<LyricsEvent>();

				var eventCount = reader.ReadUInt32();
				for(var j = 0; j < eventCount; j++)
				{
					var evFields = reader.ReadByte();

					var type = (LyricsEventType)reader.ReadByte();
					var id = reader.ReadInt32();
					var startTimeMs = reader.ReadUInt32();
					var endTimeMs = reader.ReadUInt32();
					var linkedId = -1;
					if(evFields == 1 || evFields == 3)
					{
						linkedId = reader.ReadInt32();
					}
					var ev = new LyricsEvent(type, id, new TimeSpanTimecode(startTimeMs), new TimeSpanTimecode(endTimeMs), linkedId);

					if(evFields == 2 || evFields == 3)
					{
						ev.Text = reader.ReadNullTerminatedString();
					}

					events.Add(ev);
				}

				_tracks[i] = new LyricsTrack();
				_tracks[i].AddEvents(events);
			}
		}
	}
}
