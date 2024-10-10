using CSCore;
using KaraokeLib.Audio.SoundTouch;
using KaraokeLib.Tracks;

namespace KaraokeLib.Audio
{
	/// <summary>
	/// Wrapper for <see cref="AudioMixer"/> allowing for playback rate to be changed.
	/// </summary>
	public class PlaybackRateAudioMixer : ISampleSource
	{
		private VarispeedSampleProvider _provider;
		private AudioMixer _mixer;

		public PlaybackRateAudioMixer(IEnumerable<KaraokeTrack> tracks, float playbackRate, int sampleRate)
		{
			_mixer = new AudioMixer(tracks, sampleRate);
			_provider = new VarispeedSampleProvider(_mixer, 15, new SoundTouchProfile(true, true));
			_provider.PlaybackRate = playbackRate;
		}

		public WaveFormat WaveFormat => _mixer.WaveFormat;

		public AudioMixer InternalMixer => _mixer;

		public long Length => _mixer.Length;

		public long Position
		{
			get => _mixer.Position;
			set
			{
				_mixer.Position = value;
				_provider.Reposition();
			}
		}

		public float PlaybackRate
		{
			get => _provider.PlaybackRate;
			set => _provider.PlaybackRate = value;
		}

		public bool CanSeek => true;

		public void Dispose()
		{
			_provider.Dispose();
			_mixer.Dispose();
		}

		public int Read(float[] buffer, int offset, int count)
		{
			return _provider.Read(buffer, offset, count);
		}
	}
}
