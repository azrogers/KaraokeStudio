using KaraokeLib.Events;

namespace KaraokeLib.Files
{
	public class LrcKaraokeFile : KaraokeFile<LrcKaraokeProvider>
	{
		public LrcKaraokeFile(string filename) : base(new LrcKaraokeProvider(filename)) { }

		public LrcKaraokeFile(Stream stream) : base(new LrcKaraokeProvider(stream)) { }
		public LrcKaraokeFile(IKaraokeFile otherFile) : base(new LrcKaraokeProvider(otherFile.GetTracks())) { }
		public LrcKaraokeFile(IEnumerable<KaraokeTrack> tracks) : base(new LrcKaraokeProvider(tracks)) { }
	}

	public class LrcKaraokeProvider : IKaraokeProvider
	{
		private KaraokeTrack _track;

		public LrcKaraokeProvider(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				_track = new KaraokeTrack(0, KaraokeTrackType.Lyrics);
				Load(stream);
			}
		}

		public LrcKaraokeProvider(Stream stream)
		{
			_track = new KaraokeTrack(0, KaraokeTrackType.Lyrics);
			Load(stream);
		}

		public LrcKaraokeProvider(IEnumerable<KaraokeTrack> tracks)
		{
			_track = tracks.Where(t => t.Type == KaraokeTrackType.Lyrics).FirstOrDefault() ?? new KaraokeTrack(0, KaraokeTrackType.Lyrics);
		}

		public double GetLengthSeconds()
		{
			return _track.Events.Max(ev => ev.EndTimeSeconds);
		}

		public IEnumerable<KaraokeTrack> GetTracks()
		{
			yield return _track;
		}

		public KaraokeTrack AddTrack(KaraokeTrackType type) => throw new NotImplementedException("LrcLyricsProvider.AddTrack not implemented");

		public void Save(Stream outStream)
		{
			var lineEvents = new List<KaraokeEvent>();
			var lines = new List<KaraokeEvent[]>();
			foreach (var ev in _track.Events)
			{
				if (ev.Type == KaraokeEventType.LineBreak || ev.Type == KaraokeEventType.ParagraphBreak)
				{
					if (lineEvents.Any())
					{
						lines.Add(lineEvents.ToArray());
						lineEvents.Clear();
					}

					continue;
				}

				lineEvents.Add(ev);
			}

			using (var writer = new StreamWriter(outStream))
			{
				writer.WriteLine("[ti: Exported from KaraokeStudio]");

				IEventTimecode lastTimecode = new TimeSpanTimecode(0);
				foreach (var line in lines)
				{
					if (!line.Any())
					{
						continue;
					}

					writer.Write(ToLrcTimecode(lastTimecode, true) + " ");

					foreach (var ev in line)
					{
						writer.Write(ToLrcTimecode(ev.StartTime, false));
						writer.Write((ev.LinkedId != -1 ? "" : " ") + ev.RawValue);
					}

					writer.WriteLine(ToLrcTimecode(line.Last().EndTime, false));
					lastTimecode = line.Last().EndTime;
				}
			}
		}

		private void Load(Stream inStream)
		{
			throw new NotImplementedException();
		}

		private string ToLrcTimecode(IEventTimecode time, bool isLineTimecode)
		{
			var ts = TimeSpan.FromMilliseconds(time.GetTimeMilliseconds());
			var str = ts.ToString(@"mm\:ss\.ff");
			return isLineTimecode ? $"[{str}]" : $"<{str}>";
		}
	}
}
