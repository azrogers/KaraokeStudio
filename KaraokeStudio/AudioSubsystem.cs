﻿using CSCore;
using CSCore.CoreAudioAPI;
using CSCore.SoundOut;
using KaraokeLib.Audio;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;
using KaraokeStudio.Util;

namespace KaraokeStudio
{
	internal class AudioSubsystem
	{
		public static readonly AudioSubsystem Instance = new AudioSubsystem();

		private MMDevice? _audioDevice;
		private ISoundOut? _waveOut;
		private AudioMixer _mixer = new AudioMixer([], 48000);
		private IWaveSource _mixerSource;
		private KaraokeProject? _project;
		private int _sampleRate;

		private bool _isPlaying = false;

		public void Initialize()
		{
			UpdateAudioDevice();
			_mixer?.Dispose();
			_mixer = new AudioMixer([], _sampleRate);
			_mixerSource = _mixer.ToWaveSource();
			_mixer.SetPosition(TimeSpan.FromSeconds(_project?.PlaybackState.Position ?? 0));
			UpdateAudioOutput();

			UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				_project = update.Project;
				_mixer.RebuildTracks(_project?.Tracks ?? []);
			});

			UpdateDispatcher.RegisterHandler<TracksUpdate>(update => _mixer?.RebuildTracks(_project?.Tracks ?? []));
			UpdateDispatcher.RegisterHandler<AudioSettingsUpdate>(update =>
			{
				var currentSampleRate = _sampleRate;
				UpdateAudioDevice();

				// We need to recreate the mixer if the sample rate has changed.
				if (currentSampleRate != _sampleRate)
				{
					_mixer?.Dispose();
					_mixer = new AudioMixer(_project?.Tracks ?? [], _sampleRate);
					_mixer.SetPosition(TimeSpan.FromSeconds(_project?.PlaybackState.Position ?? 0));
				}

				UpdateAudioOutput();
			});

			AppSettings.Instance.OnVolumeChanged += volume =>
			{
				if(_waveOut != null)
				{
					_waveOut.Volume = volume;
				}
			};
		}

		public void Cleanup()
		{
			_waveOut?.Dispose();
			_mixer?.Dispose();
			_audioDevice?.Dispose();
		}

		public void SetPlaybackState(bool isPlaying)
		{
			if(_isPlaying && !isPlaying)
			{
				_waveOut?.Pause();
			}
			else if(!_isPlaying && isPlaying)
			{
				_waveOut?.Play();
			}

			_isPlaying = isPlaying;
		}

		public void Seek(double position)
		{
			_mixer?.SetPosition(TimeSpan.FromSeconds(position));
		}

		private void UpdateAudioDevice()
		{
			var device = MMDeviceEnumerator.EnumerateDevices(DataFlow.Render).FirstOrDefault(device => device.DeviceID == AppSettings.Instance.AudioSettings.AudioDevice);
			if (device == null)
			{
				device = MMDeviceEnumerator.TryGetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
				AppSettings.Instance.AudioSettings.AudioDevice = device.DeviceID;
				AppSettings.Instance.Save();
			}

			_audioDevice?.Dispose();
			_audioDevice = device;
			_sampleRate = _audioDevice?.DeviceFormat.SampleRate ?? 48000;
		}

		private void UpdateAudioOutput()
		{
			_waveOut?.Dispose();

			switch (AppSettings.Instance.AudioSettings.OutputType)
			{
				case AudioSettings.AudioOutputType.DirectSound:
					_waveOut = new DirectSoundOut(AppSettings.Instance.AudioSettings.Latency);
					break;
				case AudioSettings.AudioOutputType.WaveOut:
					_waveOut = new WaveOut(AppSettings.Instance.AudioSettings.Latency);
					break;
				case AudioSettings.AudioOutputType.Wasapi:
					_waveOut = new WasapiOut(true, AudioClientShareMode.Shared, AppSettings.Instance.AudioSettings.Latency);
					break;
			}

			_waveOut?.Initialize(_mixerSource);
			if(_waveOut != null)
			{
				_waveOut.Volume = AppSettings.Instance.Volume;
			}

			if (_isPlaying)
			{
				_waveOut?.Play();
			}
		}
	}
}