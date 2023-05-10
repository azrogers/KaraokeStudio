using KaraokeLib.Video;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.FormHandlers
{
	internal class VideoFormHandler
	{
		private bool _isPlayingInternal = false;

		private Panel _videoPanel;
		private SKGLControl _skiaControl;
		private TrackBar _positionBar;

		private Label _startPosLabel;
		private Label _currentPosLabel;
		private Label _endPosLabel;

		private Button _rewindButton;
		private Button _playPauseButton;
		private Button _forwardButton;

		private double _currentVideoPosition;
		private TimeSpan? _lastLoadedTimespan = null;
		private KaraokeProject? _lastLoadedProject = null;
		private VideoGenerationState _generationState = new VideoGenerationState();
		private (int, int)? _lastUpdateSize;

		private Stopwatch _stopwatch = new Stopwatch();

		public event Action<bool>? OnPlayStateChanged;
		public event Action<double>? OnSeek;

		public double Position => _currentVideoPosition;

		public bool IsPlaying
		{
			get => _isPlayingInternal;
			private set
			{
				_isPlayingInternal = value;
				UpdateButtons();
				_stopwatch.Restart();
				OnPlayStateChanged?.Invoke(_isPlayingInternal);
			}
		}

		public VideoFormHandler(
			Panel videoPanel,
			SKGLControl skiaControl,
			TrackBar positionBar,
			Label[] labels,
			Button[] buttons)
		{
			_videoPanel = videoPanel;
			_skiaControl = skiaControl;

			_positionBar = positionBar;
			_startPosLabel = labels[0];
			_currentPosLabel = labels[1];
			_endPosLabel = labels[2];

			_rewindButton = buttons[0];
			_playPauseButton = buttons[1];
			_forwardButton = buttons[2];

			UpdateState();
		}

		public void Rewind()
		{
			_currentVideoPosition -= 10.0;
			_currentVideoPosition = Math.Max(_currentVideoPosition, 0.0);
			UpdateVideoPosition();
			OnSeek?.Invoke(_currentVideoPosition);
		}

		public void FastForward()
		{
			_currentVideoPosition += 10.0;
			_currentVideoPosition = Math.Min(_currentVideoPosition, _lastLoadedTimespan?.TotalSeconds ?? 0.0);
			UpdateVideoPosition();
			OnSeek?.Invoke(_currentVideoPosition);
		}

		public void TogglePlay()
		{
			if(IsPlaying)
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
			if(_lastLoadedTimespan == null)
			{
				return;
			}

			_currentVideoPosition = Math.Min(_lastLoadedTimespan.Value.TotalSeconds, newPosition);
			UpdateVideoPosition();
			_skiaControl.Invalidate();
			OnSeek?.Invoke(_currentVideoPosition);
		}

		public void OnTick()
		{
			if(!IsPlaying || _lastLoadedTimespan == null)
			{
				return;
			}

			var elapsed = _stopwatch.Elapsed.TotalSeconds;
			_stopwatch.Restart();

			_currentVideoPosition += elapsed;
			_currentVideoPosition = Math.Min(_lastLoadedTimespan.Value.TotalSeconds, _currentVideoPosition);
			UpdateVideoPosition();
		}

		public void UpdateState()
		{
			var isDirty = false;

			var newSize = (_videoPanel.Size.Width, _videoPanel.Size.Height);
			if (_lastUpdateSize == null || _lastUpdateSize != newSize)
			{
				UpdatePanelSize();
				_lastUpdateSize = newSize;
				isDirty = true;
			}

			// TODO: handle updating video elements

			if(isDirty)
			{
				UpdateGenerationContext();
			}
		}

		public void Render(SKSurface surface)
		{
			if(_lastLoadedProject == null)
			{
				surface.Canvas.Clear();
				return;
			}

			_generationState.Render(
				_lastLoadedProject.Tracks, 
				new VideoTimecode(_currentVideoPosition, _lastLoadedProject.Config.FrameRate), 
				surface);
		}

		public void OnProjectChanged(KaraokeProject? project)
		{
			if(_lastLoadedProject != project)
			{
				_currentVideoPosition = 0;
				IsPlaying = false;
			}
			else if(_lastLoadedProject != null && project != null && _lastLoadedTimespan != project.Length)
			{
				// project length changed, we need to adjust
				if(_currentVideoPosition > project.Length.TotalSeconds)
				{
					_currentVideoPosition = project.Length.TotalSeconds;
				}
			}

			_lastLoadedProject = project;
			_lastLoadedTimespan = project?.Length;
			UpdateVideoPosition();
			UpdateButtons();
			UpdateGenerationContext();

			_skiaControl.Invalidate();
		}

		private void UpdateVideoPosition()
		{
			if(_lastLoadedTimespan != null)
			{
				_endPosLabel.Text = Util.FormatTimespan(_lastLoadedTimespan.Value);
				_positionBar.Maximum = (int)Math.Ceiling(_lastLoadedTimespan.Value.TotalSeconds);
				_positionBar.Value = (int)Math.Round(_currentVideoPosition);
			}
			else
			{
				_endPosLabel.Text = "0:00";
				_positionBar.Maximum = 0;
				_positionBar.Value = 0;
			}

			_currentPosLabel.Text = Util.FormatTimespan(TimeSpan.FromSeconds(_currentVideoPosition), true);
		}

		private void UpdateButtons()
		{
			_rewindButton.Enabled = _playPauseButton.Enabled = _forwardButton.Enabled = _lastLoadedProject != null;
			_playPauseButton.Text = IsPlaying ? "Pause" : "Play";
		}

		public void UpdateGenerationContext()
		{
			if(_lastLoadedProject == null)
			{
				return;
			}

			_generationState.UpdateVideoContext(
				_lastLoadedProject.Length.TotalSeconds, 
				_lastLoadedProject.Config, 
				(_skiaControl.Size.Width, _skiaControl.Size.Height));
		}

		private void UpdatePanelSize()
		{
			var size = (16, 9);
			if(_lastLoadedProject != null)
			{
				size = (_lastLoadedProject.Config.VideoSize.Width, _lastLoadedProject.Config.VideoSize.Height);
			}

			Util.ResizeContainerAspectRatio(_videoPanel, _skiaControl, size);
		}
	}
}
