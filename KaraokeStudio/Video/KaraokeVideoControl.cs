﻿using FontAwesome.Sharp;
using KaraokeLib.Video;
using KaraokeStudio.Util;
using NAudio.Wave;
using SkiaSharp;
using System.Diagnostics;

namespace KaraokeStudio.Video
{
	internal partial class KaraokeVideoControl : UserControl
	{
		private bool _isPlayingInternal = false;

		private WaveOutEvent _output;

		private System.Windows.Forms.Timer _timer;

		private double _currentVideoPosition;
		private TimeSpan? _lastLoadedTimespan = null;
		private KaraokeProject? _lastLoadedProject = null;
		private (int, int)? _lastUpdateSize;

		private Stopwatch _stopwatch = new Stopwatch();

		private IVideoGenerator _videoGenerator = new KaraokeProjectVideoGenerator();

		public event Action<bool>? OnPlayStateChanged;
		public event Action<double>? OnSeek;
		public event Action<double>? OnPositionChangedEvent;

		public double Position => _currentVideoPosition;

		public bool IsPlaying
		{
			get => _isPlayingInternal;
			private set
			{
				_isPlayingInternal = value;
				UpdateButtons();
				_stopwatch.Restart();
				_timer.Enabled = _isPlayingInternal;
				OnPlayStateChanged?.Invoke(_isPlayingInternal);

				if (_lastLoadedProject != null)
				{
					if (_isPlayingInternal && _output.PlaybackState != PlaybackState.Playing)
					{
						_output.Play();
					}
					else if (!_isPlayingInternal && _output.PlaybackState == PlaybackState.Playing)
					{
						_output.Pause();
					}
				}
			}
		}

		public KaraokeVideoControl()
		{
			InitializeComponent();

			_output = new WaveOutEvent();
			_output.Volume = AppSettings.Instance.Volume;
			volumeSlider.Volume = AppSettings.Instance.Volume;

			_timer = new System.Windows.Forms.Timer();
			_timer.Enabled = false;
			_timer.Tick += OnTimerTick;
		}

		public void SetVideoGenerator(IVideoGenerator generator)
		{
			_videoGenerator = generator;
			videoSkiaControl.Invalidate();
		}

		public void Rewind()
		{
			_currentVideoPosition -= 10.0;
			_currentVideoPosition = Math.Max(_currentVideoPosition, 0.0);
			UpdateVideoPosition();
			HandleSeek();
		}

		public void FastForward()
		{
			_currentVideoPosition += 10.0;
			_currentVideoPosition = Math.Min(_currentVideoPosition, _lastLoadedTimespan?.TotalSeconds ?? 0.0);
			UpdateVideoPosition();
			HandleSeek();
		}

		public void TogglePlay()
		{
			if (IsPlaying)
			{
				Pause();
			}
			else
			{
				Play();
			}
		}

		public void Play()
		{
			IsPlaying = true;
		}

		public void Pause()
		{
			IsPlaying = false;
		}

		public void Seek(double newPosition)
		{
			if (_lastLoadedTimespan == null)
			{
				return;
			}

			_currentVideoPosition = Math.Min(_lastLoadedTimespan.Value.TotalSeconds, newPosition);
			UpdateVideoPosition();
			videoSkiaControl.Invalidate();
			HandleSeek();
		}

		public void OnTick()
		{
			if (!IsPlaying || _lastLoadedTimespan == null)
			{
				return;
			}

			var elapsed = _stopwatch.Elapsed.TotalSeconds;
			_stopwatch.Restart();

			_currentVideoPosition += elapsed;
			_currentVideoPosition = Math.Min(_lastLoadedTimespan.Value.TotalSeconds, _currentVideoPosition);
			OnPositionChangedEvent?.Invoke(_currentVideoPosition);
			UpdateVideoPosition();
		}

		public void UpdateState()
		{
			var isDirty = false;

			var newSize = (videoPanel.Size.Width, videoPanel.Size.Height);
			if (_lastUpdateSize == null || _lastUpdateSize != newSize)
			{
				UpdatePanelSize();
				_lastUpdateSize = newSize;
				isDirty = true;
			}

			// TODO: handle updating video elements

			if (isDirty)
			{
				UpdateGenerationContext();
			}
		}

		public void Render(SKSurface surface)
		{
			if (_lastLoadedProject == null)
			{
				surface.Canvas.Clear();
				return;
			}

			_videoGenerator.Render(
				_lastLoadedProject,
				new VideoTimecode(_currentVideoPosition, _lastLoadedProject.Config.FrameRate),
				surface);
		}

		public void OnPositionChanged(double pos)
		{
			_stopwatch.Restart();
			_currentVideoPosition = pos;
			videoSkiaControl.Invalidate();
			UpdateVideoPosition();
			
			if (_lastLoadedProject != null)
			{
				_lastLoadedProject.Mixer.CurrentTime = TimeSpan.FromSeconds(_currentVideoPosition);
			}
		}

		public void OnProjectChanged(KaraokeProject? project)
		{
			if (_lastLoadedProject != project)
			{
				_currentVideoPosition = 0;
				OnPositionChangedEvent?.Invoke(_currentVideoPosition);
				IsPlaying = false;
				_output.Stop();
			}
			else if (_lastLoadedProject != null && project != null && _lastLoadedTimespan != project.Length)
			{
				// project length changed, we need to adjust
				if (_currentVideoPosition > project.Length.TotalSeconds)
				{
					_currentVideoPosition = project.Length.TotalSeconds;
					OnPositionChangedEvent?.Invoke(_currentVideoPosition);
				}
			}

			if (project != null)
			{
				_timer.Interval = (int)Math.Round((1.0 / project.Config.FrameRate) * 1000);
			}

			volumeSlider.Enabled = project?.Tracks.Any(t => t.Type == KaraokeLib.KaraokeTrackType.Audio) ?? false;

			if(project != null)
			{
				_output.Init(project.Mixer);
			}

			_lastLoadedProject = project;
			_lastLoadedTimespan = project?.Length;
			UpdateVideoPosition();
			UpdateButtons();
			UpdateGenerationContext();

			videoSkiaControl.Invalidate();
		}

		public void OnProjectEventsChanged(KaraokeProject? project)
		{
			_videoGenerator.Invalidate();
			videoSkiaControl.Invalidate();
		}

		private void HandleSeek()
		{
			OnSeek?.Invoke(_currentVideoPosition);
			OnPositionChangedEvent?.Invoke(_currentVideoPosition);
			if (_lastLoadedProject != null)
			{
				_lastLoadedProject.Mixer.CurrentTime = TimeSpan.FromSeconds(_currentVideoPosition);
			}
		}

		private void UpdateVideoPosition()
		{
			if (_lastLoadedTimespan != null)
			{
				endPosLabel.Text = Utility.FormatTimespan(_lastLoadedTimespan.Value);
				positionBar.Maximum = (int)Math.Ceiling(_lastLoadedTimespan.Value.TotalSeconds);
				positionBar.Value = (int)Math.Round(_currentVideoPosition);
			}
			else
			{
				endPosLabel.Text = "0:00";
				positionBar.Maximum = 0;
				positionBar.Value = 0;
			}

			currentPosLabel.Text = Utility.FormatTimespan(TimeSpan.FromSeconds(_currentVideoPosition), true);
		}

		private void UpdateButtons()
		{
			backButton.Enabled = playPauseButton.Enabled = forwardButton.Enabled = _lastLoadedProject != null;
			playPauseButton.IconChar = IsPlaying ? IconChar.Pause : IconChar.Play;
		}

		public void UpdateGenerationContext()
		{
			if (_lastLoadedProject == null)
			{
				return;
			}

			_videoGenerator.UpdateContext(_lastLoadedProject, videoSkiaControl.Size);
		}

		private void UpdatePanelSize()
		{
			var size = (16, 9);
			if (_lastLoadedProject != null)
			{
				size = (_lastLoadedProject.Config.VideoSize.Width, _lastLoadedProject.Config.VideoSize.Height);
			}

			Utility.ResizeContainerAspectRatio(videoPanel, videoSkiaControl, size);
		}

		private void OnTimerTick(object? sender, EventArgs e)
		{
			// tell the video panel we need to repaint
			videoSkiaControl.Invalidate();
		}

		private void backButton_Click(object sender, EventArgs e) => Rewind();

		private void forwardButton_Click(object sender, EventArgs e) => FastForward();

		private void playPauseButton_Click(object sender, EventArgs e) => TogglePlay();

		private void positionBar_Scroll(object sender, EventArgs e) => Seek(positionBar.Value);

		private void videoSkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			OnTick();
			Render(e.Surface);
		}

		private void videoPanel_Paint(object sender, PaintEventArgs e)
		{
			UpdateState();
		}

		private void volumeSlider_VolumeChanged(object sender, EventArgs e)
		{
			_output.Volume = volumeSlider.Volume;
			AppSettings.Instance.SetVolume(volumeSlider.Volume);
		}
	}
}
