using Concentus.Common;
using CSCore;
using CSCore.Codecs.RAW;
using CSCore.DSP;
using CSCore.Streams;
using KaraokeLib.Audio.SoundTouch;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using System.Diagnostics;

namespace KaraokeLib.Audio
{
	/// <summary>
	/// Mixes together multiple Audio KaraokeTracks into a single audio stream playable by NAudio.
	/// </summary>
	public class AudioMixer : ISampleSource
	{
		private KaraokeTrack[] _audioTracks = [];
		private Dictionary<int, float[]> _loadedStreams;
		private double _duration;
		private WaveFormat _waveFormat;

		public AudioMixer(IEnumerable<KaraokeTrack> tracks, int sampleRate)
		{
			_loadedStreams = new Dictionary<int, float[]>();
			_waveFormat = new WaveFormat(sampleRate, 16, 2);
			RebuildTracks(tracks);
		}

		public WaveFormat WaveFormat => _waveFormat;

		public long Length => (long)(_waveFormat.SampleRate * _duration);

		public long Position { get; set; }
		public double PositionSeconds
		{
			get => Position / (double)WaveFormat.SampleRate;
		}

		public bool CanSeek => throw new NotImplementedException();

		public void UpdateTracks(IEnumerable<KaraokeTrack> tracks)
		{
			if (TracksNeedRebuild(tracks))
			{
				RebuildTracks(tracks);
			}
		}

		private bool TracksNeedRebuild(IEnumerable<KaraokeTrack> tracks)
		{
			var currentIds = _audioTracks.Select(t => t.Id).ToHashSet();
			foreach (var track in tracks)
			{
				if (currentIds.Contains(track.Id))
				{
					return true;
				}
			}

			return tracks.Any(t => t.Type == KaraokeTrackType.Audio && !currentIds.Contains(t.Id));
		}

		public void RebuildTracks(IEnumerable<KaraokeTrack> tracks)
		{
			_audioTracks = tracks.Where(t => t.Type == KaraokeTrackType.Audio).ToArray();

			var loadedEvents = new HashSet<int>();
			foreach (var track in _audioTracks)
			{
				foreach (var ev in track.Events.Where(ev => ev.Type == KaraokeEventType.AudioClip))
				{
					var audioClipEvent = (AudioClipKaraokeEvent)ev;
					var audioFile = audioClipEvent?.Settings?.LoadAudioFile();
					if (audioFile != null)
					{
						var stream = audioFile.WaveFormat == _waveFormat ? audioFile.ToSampleSource() : new DmoResampler(audioFile, _waveFormat.SampleRate).ToSampleSource();
						var buffer = new float[stream.Length];
						stream.Read(buffer, 0, (int)stream.Length);
						_loadedStreams[ev.Id] = buffer;
						loadedEvents.Add(ev.Id);
					}
				}
			}

			var eventsToRemove = new List<int>();
			foreach (var k in _loadedStreams.Keys)
			{
				if (!loadedEvents.Contains(k))
				{
					eventsToRemove.Add(k);
				}
			}

			foreach (var ev in eventsToRemove)
			{
				_loadedStreams.Remove(ev);
			}

			_duration = _audioTracks.Any() ? _audioTracks.Max(t => t.Events.Any() ? t.Events.Max(t => t.EndTimeSeconds) : 0) : 0;
		}

		public int Read(float[] buffer, int offset, int count)
		{
			for (var i = 0; i < count; i++)
			{
				buffer[offset + i] = 0;
			}

			if (Position >= Length)
			{
				return count;
			}

			var relevantClips = new List<AudioClipKaraokeEvent>();
			var relevantClipVolumes = new List<float>();
			var durationSeconds = count / (double)WaveFormat.SampleRate;
			foreach (var track in _audioTracks)
			{
				var settings = track.GetTrackConfig<AudioTrackSettings>();
				// ignore this track, we're muted
				if (settings.Muted)
				{
					continue;
				}

				var foundClips = track
					.GetRelevantEvents((PositionSeconds, PositionSeconds + durationSeconds))
					.Where(e => e.Type == KaraokeEventType.AudioClip)
					.Cast<AudioClipKaraokeEvent>()
					.ToArray();
				relevantClips.AddRange(foundClips);
				relevantClipVolumes.AddRange(foundClips.Select(f => settings.Volume));
			}

			var currentPos = Position;
			Position += count;

			// nothing to do
			if (!relevantClips.Any())
			{
				return count;
			}

			var clips = relevantClips.ToArray();
			var clipVolumes = relevantClipVolumes.ToArray();

			var workBuffer = new float[count];
			for (var i = 0; i < clips.Length; i++)
			{
				Array.Clear(workBuffer);

				var clip = clips[i];
				var stream = _loadedStreams[clip.Id];
				var offsetSamples = (long)((clip.Settings?.Offset ?? 0) * _waveFormat.SampleRate);
				var startTimeSamples = (long)(clip.StartTimeSeconds * _waveFormat.SampleRate);

				// offset in samples of the stream from the current position of the mixer
				var streamPositionSamples = currentPos - startTimeSamples + offsetSamples;

				// offset from the start of the buffer
				var sampleOffset = (int)Math.Max(-streamPositionSamples, 0);
				var samplePosition = Math.Max(0, streamPositionSamples);
				// add samples to output buffer
				for (var j = 0; j < workBuffer.Length - sampleOffset; j++)
				{
					buffer[offset + sampleOffset + j] += stream[streamPositionSamples + j] * clipVolumes[i];
				}
			}

			return count;
		}

		public void Dispose()
		{
			foreach (var stream in _loadedStreams.Values)
			{
				//stream.Dispose();
			}

			_loadedStreams.Clear();
			_audioTracks = [];
		}
	}
}
