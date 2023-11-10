using KaraokeLib.Events;
using KaraokeLib.Tracks;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace KaraokeLib.Audio
{
    /// <summary>
    /// Mixes together multiple Audio KaraokeTracks into a single audio stream playable by NAudio.
    /// </summary>
    public class AudioMixer : WaveStream
	{
		private const int TARGET_SAMPLE_RATE = 44100;
		private const int TARGET_CHANNELS = 2;

		private KaraokeTrack[] _audioTracks;
		private Dictionary<int, WaveStream> _loadedStreams;
		private double _duration;
		private double _position;

		public AudioMixer(IEnumerable<KaraokeTrack> tracks)
		{
			_audioTracks = tracks.Where(t => t.Type == KaraokeTrackType.Audio).ToArray();
			_loadedStreams = new Dictionary<int, WaveStream>();
			foreach (var track in _audioTracks)
			{
				foreach (var ev in track.Events.Where(ev => ev.Type == KaraokeEventType.AudioClip))
				{
					var audioClipEvent = (AudioClipKaraokeEvent)ev;
					var audioFile = audioClipEvent?.Settings?.LoadAudioFile();
					if(audioFile != null)
					{
						_loadedStreams[ev.Id] = audioFile;
					}
				}
			}

			_duration = _audioTracks.Any() ? _audioTracks.Max(t => t.Events.Any() ? t.Events.Max(t => t.EndTimeSeconds) : 0) : 0;
		}

		public override WaveFormat WaveFormat => WaveFormat.CreateIeeeFloatWaveFormat(TARGET_SAMPLE_RATE, TARGET_CHANNELS);

		public override long Length => (long)(WaveFormat.AverageBytesPerSecond * _duration);

		public override long Position
		{
			get => (long)(_position * WaveFormat.AverageBytesPerSecond);
			set => _position = value / (double)WaveFormat.AverageBytesPerSecond;
		}

		public override bool HasData(int count)
		{
			return Position < Length;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if(Position >= Length)
			{
				return 0;
			}

			var durationSeconds = count / (double)WaveFormat.AverageBytesPerSecond;
			var relevantClips = new List<AudioClipKaraokeEvent>();
			var relevantClipVolumes = new List<float>();
			foreach (var track in _audioTracks)
			{
				var settings = track.GetTrackConfig<AudioTrackSettings>();
				// ignore this track, we're muted
				if(settings.Muted)
				{
					continue;
				}

				var foundClips = track
					.GetRelevantEvents((_position, _position + durationSeconds))
					.Where(e => e.Type == KaraokeEventType.AudioClip)
					.Cast<AudioClipKaraokeEvent>()
					.ToArray();
				relevantClips.AddRange(foundClips);
				relevantClipVolumes.AddRange(foundClips.Select(f => settings.Volume));
			}

			var currentPos = _position;
			Position += count;

			for (var i = 0; i < count; i++)
			{
				buffer[offset + i] = 0;
			}

			// nothing to do
			if (!relevantClips.Any())
			{
				return count;
			}

			var clips = relevantClips.ToArray();
			var clipVolumes = relevantClipVolumes.ToArray();
			var buffers = new byte[clips.Length][];
			for (var i = 0; i < clips.Length; i++)
			{
				var clip = clips[i];
				var stream = _loadedStreams[clip.Id];
				var streamBuffer = new byte[(int)Math.Ceiling(durationSeconds * stream.WaveFormat.AverageBytesPerSecond)];
				for (var j = 0; j < streamBuffer.Length; j++)
				{
					streamBuffer[j] = 0;
				}

				var streamPosition = currentPos - clip.StartTimeSeconds + (clip.Settings?.Offset ?? 0);
				// offset from the start of the audio file
				stream.Position = Math.Max(0, (long)(streamPosition * stream.WaveFormat.AverageBytesPerSecond));
				// offset from the start of the buffer
				var byteOffset = (int)(Math.Max(-streamPosition, 0) * stream.WaveFormat.AverageBytesPerSecond);
				stream.Read(streamBuffer, byteOffset, streamBuffer.Length - byteOffset);

				var outBuffer = new byte[count];
				for (var j = 0; j < outBuffer.Length; j++)
				{
					outBuffer[j] = 0;
				}

				var volumeSampler = new VolumeSampleProvider(new RawSourceWaveStream(new MemoryStream(streamBuffer), stream.WaveFormat).ToSampleProvider());
				volumeSampler.Volume = clipVolumes[i];

				// resample if needed
				if (stream.WaveFormat != WaveFormat)
				{
					using (var resampler = new MediaFoundationResampler(volumeSampler.ToWaveProvider(), WaveFormat))
					{
						resampler.Read(outBuffer, 0, count);
					}
				}
				else
				{
					volumeSampler.ToWaveProvider().Read(outBuffer, 0, count);
					//Array.CopyTyped(streamBuffer, outBuffer, count);
				}

				buffers[i] = outBuffer;
			}

			var readers = new BinaryReader[clips.Length];
			for (var j = 0; j < clips.Length; j++)
			{
				readers[j] = new BinaryReader(new MemoryStream(buffers[j]));
			}

			var bufferWriter = new BinaryWriter(new MemoryStream(buffer, offset, count));

			// read each float from the stream and sum them
			for (var i = 0; i < count / sizeof(float); i++)
			{
				var total = 0.0f;
				for (var j = 0; j < clips.Length; j++)
				{
					total += readers[j].ReadSingle();
				}

				if (i >= buffer.Length)
				{
					break;
				}

				bufferWriter.Write(total);
			}

			return count;
		}
	}
}
