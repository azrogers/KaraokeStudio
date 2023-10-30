using KaraokeLib.Events;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;

namespace KaraokeLib.Files
{
	public class MidiKaraokeFile : KaraokeFile<MidiKaraokeProvider>
	{
		public MidiKaraokeFile(string filename)
			: base(new MidiKaraokeProvider(filename)) { }

		public MidiKaraokeFile(Stream stream)
			: base(new MidiKaraokeProvider(stream)) { }
	}

	/// <summary>
	/// Reads and writes Guitar Hero/Rock Band notes.mid files.
	/// </summary>
	public class MidiKaraokeProvider : IKaraokeProvider
	{
		private MidiFile _file;
		private List<TrackChunk> _lyricChunks = new List<TrackChunk>();

		/// <inheritdoc />
		public bool CanRead => true;

		/// <inheritdoc />
		public bool CanWrite => false;

		public MidiKaraokeProvider(string filename)
		{
			_file = MidiFile.Read(filename);
			PopulateLyricChunks();
		}

		public MidiKaraokeProvider(Stream inputStream)
		{
			_file = MidiFile.Read(inputStream);
			PopulateLyricChunks();
		}

		/// <inheritdoc />
		public double GetLengthSeconds()
		{
			return _file.GetDuration<MetricTimeSpan>().TotalSeconds;
		}

		/// <inheritdoc />
		public KaraokeTrack AddTrack(IKaraokeFile file, KaraokeTrackType type) => 
			throw new NotImplementedException("MidiKaraokeProvider.AddTrack not implemented");

		/// <inheritdoc />
		public void Save(Stream outStream)
		{
			throw new NotImplementedException("Can't save MidiKaraokeProvider");
		}

		/// <inheritdoc />
		public IEnumerable<KaraokeTrack> GetTracks()
		{
			foreach (var track in _lyricChunks)
			{
				var events = new List<KaraokeEvent>();

				long timer = 0;

				var expectingFollowingLyric = false;
				var previousId = 0;
				var nextId = 0;
				string? lyricText = null;
				var noteDepth = 0;
				IEventTimecode? startTimecode = null;
				var nextTrackId = 0;
				foreach (var ev in track.Events)
				{
					timer += ev.DeltaTime;

					if (ev is NoteOnEvent noteOn)
					{
						noteDepth++;
						startTimecode = new MidiTimecode(_file.GetTempoMap(), timer);
					}
					else if (ev is LyricEvent lyric)
					{
						if (lyric.Text.Trim() == "+")
						{
							var timecode = new MidiTimecode(_file.GetTempoMap(), timer);
							var breakEvent = new KaraokeEvent(
								KaraokeEventType.LineBreak,//KaraokeEventType.ParagraphBreak,
								nextId,
								timecode,
								timecode,
								-1);
							events.Add(breakEvent);

							previousId = nextId;
							nextId++;
							expectingFollowingLyric = false;
							lyricText = null;
							continue;
						}

						lyricText = lyric.Text;
					}
					else if (ev is NoteOffEvent noteOff)
					{
						noteDepth--;

						if (noteDepth == 0)
						{
							var timecode = new MidiTimecode(_file.GetTempoMap(), timer);
							var nlEvent = new KaraokeEvent(
								KaraokeEventType.LineBreak,
								nextId,
								timecode,
								timecode,
								-1);
							events.Add(nlEvent);
						}

						if (!ContainsValidLyric(lyricText))
						{
							lyricText = null;
							continue;
						}

						var endTimecode = new MidiTimecode(_file.GetTempoMap(), timer);

						if (startTimecode == null)
						{
							throw new NullReferenceException("startTimecode unset?");
						}

						var text = (lyricText ?? "").TrimEnd('#');
						var newEvent = new KaraokeEvent(
							KaraokeEventType.Lyric,
							nextId,
							startTimecode,
							endTimecode,
							expectingFollowingLyric ? previousId : -1
						);

						expectingFollowingLyric = text.EndsWith('-');

						newEvent.RawValue = text.TrimEnd('-').Replace('=', '-');

						previousId = nextId;
						nextId++;
						lyricText = null;

						events.Add(newEvent);
					}
				}

				// insert paragraph and line breaks based on event timing
				var lyricsEvents = events.Where(ev => ev.Type == KaraokeEventType.Lyric).ToList();
				var distBetweenEvents = new List<double>();
				var timecodes = new List<(IEventTimecode, IEventTimecode)>();
				for (var i = 1; i < lyricsEvents.Count; i++)
				{
					if (events[i].StartTimeMilliseconds == events[i - 1].StartTimeMilliseconds)
					{
						// ignore events that occur at the same time
						continue;
					}

					distBetweenEvents.Add(events[i].StartTimeSeconds - events[i - 1].EndTimeSeconds);
					timecodes.Add((events[i - 1].EndTime, events[i].StartTime));
				}

				var distMean = distBetweenEvents.Sum() / distBetweenEvents.Count;
				var stdDev = Math.Sqrt(distBetweenEvents.Sum(f => Math.Pow(f - distMean, 2)) / distBetweenEvents.Count);

				for (var i = 0; i < distBetweenEvents.Count; i++)
				{
					if (distBetweenEvents[i] >= stdDev * 3)
					{
						events.Add(new KaraokeEvent(KaraokeEventType.ParagraphBreak, nextId++, timecodes[i].Item1, timecodes[i].Item2));
					}
					else if (distBetweenEvents[i] >= stdDev * 2)
					{
						events.Add(new KaraokeEvent(KaraokeEventType.LineBreak, nextId++, timecodes[i].Item1, timecodes[i].Item2));
					}
				}

				var newTrack = new KaraokeTrack(nextTrackId++, KaraokeTrackType.Lyrics);
				newTrack.AddEvents(events);
				yield return newTrack;
			}
		}

		private void PopulateLyricChunks()
		{
			TrackChunk?[] chunks = _file.GetTrackChunks().ToArray();
			foreach (var chunk in chunks)
			{
				if (chunk != null && IsLyricChunk(chunk))
				{
					_lyricChunks.Add(chunk);
				}
			}
		}

		private bool IsLyricChunk(TrackChunk chunk)
		{
			return chunk.Events.Any(ev =>
				ev.EventType == MidiEventType.Lyric &&
				ContainsValidLyric(((LyricEvent)ev).Text));
		}

		private bool ContainsValidLyric(string? text)
		{
			if (text == null)
			{
				return false;
			}

			var trimmed = text.Trim();
			return
				trimmed != "+" &&
				!(trimmed.StartsWith('[') && trimmed.EndsWith(']'));
		}
	}

	internal class MidiTimecode : IEventTimecode
	{
		private MetricTimeSpan _timeSpan;

		public MidiTimecode(TempoMap map, long time)
		{
			_timeSpan = TimeConverter.ConvertTo<MetricTimeSpan>(time, map);
		}

		public double GetTimeSeconds()
		{
			return _timeSpan.TotalSeconds;
		}

		public ulong GetTimeMilliseconds()
		{
			return (ulong)(_timeSpan.TotalMicroseconds / 1000L);
		}

		public int CompareTo(IEventTimecode? other)
		{
			if (other == null)
			{
				return 1;
			}

			return GetTimeMilliseconds().CompareTo(other.GetTimeMilliseconds());
		}
	}
}
