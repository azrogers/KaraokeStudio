using KaraokeLib.Audio.SoundTouch;
using KaraokeLib.Tracks;
using NAudio.Wave;

namespace KaraokeLib.Audio
{
	/// <summary>
	/// Wrapper for <see cref="AudioMixer"/> allowing for playback rate to be changed.
	/// </summary>
	public class PlaybackRateAudioMixer : WaveStream
	{
		private VarispeedSampleProvider _provider;
		private AudioMixer _mixer;

		public PlaybackRateAudioMixer(IEnumerable<KaraokeTrack> tracks, float playbackRate)
		{
			_mixer = new AudioMixer(tracks);
			var lengthMs = (int)(_mixer.Length / (double)_mixer.WaveFormat.SampleRate * 1000.0);

			_provider = new VarispeedSampleProvider(_mixer.ToSampleProvider(), 100, new SoundTouchProfile(true, true));
			_provider.PlaybackRate = playbackRate;
		}

		public override WaveFormat WaveFormat => _mixer.WaveFormat;

		public override long Length => _mixer.Length;

		public override long Position { get => _mixer.Position; set => _mixer.Position = value; }

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _provider.ToWaveProvider().Read(buffer, offset, count);
		}
	}
}
