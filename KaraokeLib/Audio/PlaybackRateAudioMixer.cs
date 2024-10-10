using CSCore;
using KaraokeLib.Audio.SoundTouch;
using KaraokeLib.Tracks;

namespace KaraokeLib.Audio
{
	/// <summary>
	/// Wrapper for <see cref="AudioMixer"/> allowing for playback rate to be changed.
	/// </summary>
	public class PlaybackRateAudioMixer : IWaveSource
	{
		private VarispeedSampleProvider _provider;
		private AudioMixer _mixer;

		public PlaybackRateAudioMixer(IEnumerable<KaraokeTrack> tracks, float playbackRate, int sampleRate)
		{
			_mixer = new AudioMixer(tracks, sampleRate);
			_provider = new VarispeedSampleProvider(_mixer, 1000, new SoundTouchProfile(true, true));
			_provider.PlaybackRate = playbackRate;
		}

		public WaveFormat WaveFormat => _mixer.WaveFormat;

		public long Length => _mixer.Length;

		public long Position { get => _mixer.Position; set => _mixer.Position = value; }

		public bool CanSeek => true;

		public void Dispose()
		{
			_provider.Dispose();
			_mixer.Dispose();
		}

		public int Read(byte[] buffer, int offset, int count)
		{
			return _mixer.ToWaveSource().Read(buffer, offset, count);
		}
	}
}
