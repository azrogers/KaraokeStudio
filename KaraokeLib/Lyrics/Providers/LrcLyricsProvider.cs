namespace KaraokeLib.Lyrics.Providers
{
	public class LrcLyricsFile : LyricsFile<LrcLyricsProvider>
	{
		public LrcLyricsFile(string filename) : base(new LrcLyricsProvider(filename)) { }

		public LrcLyricsFile(Stream stream) : base(new LrcLyricsProvider(stream)) { }
		public LrcLyricsFile(ILyricsFile otherFile) : base(new LrcLyricsProvider(otherFile.GetTracks())) { }
		public LrcLyricsFile(IEnumerable<LyricsTrack> tracks) : base(new LrcLyricsProvider(tracks)) { }
	}

	public class LrcLyricsProvider : ILyricsProvider
	{
		private LyricsTrack _track;

		public LrcLyricsProvider(string fileName)
		{
			using (var stream = File.OpenRead(fileName))
			{
				_track = new LyricsTrack(0, LyricsTrackType.Lyrics);
				Load(stream);
			}
		}

		public LrcLyricsProvider(Stream stream)
		{
			_track = new LyricsTrack(0, LyricsTrackType.Lyrics);
			Load(stream);
		}

		public LrcLyricsProvider(IEnumerable<LyricsTrack> tracks)
		{
			_track = tracks.Where(t => t.Type == LyricsTrackType.Lyrics).FirstOrDefault() ?? new LyricsTrack(0, LyricsTrackType.Lyrics);
		}

		public double GetLengthSeconds()
		{
			return _track.Events.Max(ev => ev.EndTimeSeconds);
		}

		public IEnumerable<LyricsTrack> GetTracks()
		{
			yield return _track;
		}

		public void Save(Stream outStream)
		{
			var lineEvents = new List<LyricsEvent>();
			var lines = new List<LyricsEvent[]>();
			foreach (var ev in _track.Events)
			{
				if (ev.Type == LyricsEventType.LineBreak || ev.Type == LyricsEventType.ParagraphBreak)
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
						writer.Write((ev.LinkedId != -1 ? "" : " ") + ev.RawText);
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
