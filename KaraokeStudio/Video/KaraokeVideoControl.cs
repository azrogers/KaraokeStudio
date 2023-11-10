using FontAwesome.Sharp;
using KaraokeLib.Tracks;
using KaraokeLib.Video;
using KaraokeStudio.Project;
using KaraokeStudio.Util;
using SkiaSharp;

namespace KaraokeStudio.Video
{
    internal partial class KaraokeVideoControl : UserControl
	{
		private static SKPaint _notActiveOverlayPaint = new SKPaint()
		{
			Color = new SKColor(0, 0, 0, 50)
		};

		private (int, int)? _lastUpdateSize;
		private KaraokeProject? _project;
		private SKImage? _lastFrame;

		private IVideoGenerator _videoGenerator = new KaraokeProjectVideoGenerator();

		public KaraokeVideoControl()
		{
			InitializeComponent();

			volumeSlider.Volume = AppSettings.Instance.Volume;
		}

		~KaraokeVideoControl()
		{
			if (_project != null)
			{
				_project.PlaybackState.OnPositionChanged -= PlaybackState_OnPositionChanged;
				_project.PlaybackState.ReleaseVideoPlaybackClaim(this);
			}

			if(_lastFrame != null)
			{
				_lastFrame.Dispose();
			}
		}

		public void ForceRerender() => videoSkiaControl.Invalidate();

		public void OnProjectChanged(KaraokeProject? project)
		{
			if (_project != null)
			{
				// remove old event listeners
				_project.PlaybackState.OnPlayStateChanged -= PlaybackState_OnPlayStateChanged;
				_project.PlaybackState.OnPositionChanged -= PlaybackState_OnPositionChanged;
				_project.PlaybackState.ReleaseVideoPlaybackClaim(this);
			}

			if (project != null)
			{
				project.PlaybackState.OnPlayStateChanged += PlaybackState_OnPlayStateChanged;
				project.PlaybackState.OnPositionChanged += PlaybackState_OnPositionChanged;
				project.PlaybackState.AcquireVideoPlaybackClaim(this);
			}

			volumeSlider.Enabled = project?.Tracks.Any(t => t.Type == KaraokeTrackType.Audio) ?? false;

			_lastFrame = null;
			_project = project;
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

		public void SetVideoGenerator(IVideoGenerator generator)
		{
			_videoGenerator = generator;
			videoSkiaControl.Invalidate();
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

			if (isDirty)
			{
				UpdateGenerationContext();
			}
		}

		public void Render(SKSurface surface)
		{
			if (_project == null)
			{
				surface.Canvas.Clear();
				return;
			}

			// not active - draw a disabled overlay
			if(_project.PlaybackState.CurrentVideoPlaybackClaim != this)
			{
				if(_lastFrame != null)
				{
					surface.Canvas.DrawImage(_lastFrame, SKPoint.Empty);
				}

				surface.Canvas.DrawRect(new SKRect(0, 0, ClientSize.Width, ClientSize.Height), _notActiveOverlayPaint);
				return;
			}

			_videoGenerator.Render(
				_project,
				new VideoTimecode(_project.PlaybackState.Position, _project.Config.FrameRate),
				surface);

			if(_lastFrame != null)
			{
				_lastFrame.Dispose();
			}
			_lastFrame = surface.Snapshot();
		}

		private void PlaybackState_OnPositionChanged(double obj)
		{
			videoSkiaControl.Invalidate();
			UpdateVideoPosition();
		}

		private void PlaybackState_OnPlayStateChanged(bool obj)
		{
			videoSkiaControl.Invalidate();
			UpdateButtons();
		}

		private void UpdateVideoPosition()
		{
			if (_project != null)
			{
				endPosLabel.Text = Utility.FormatTimespan(_project.Length);
				positionBar.Maximum = (int)Math.Ceiling(_project.Length.TotalSeconds);
				positionBar.Value = (int)Math.Round(_project.PlaybackState.Position);
				currentPosLabel.Text = Utility.FormatTimespan(TimeSpan.FromSeconds(_project.PlaybackState.Position), true);
			}
			else
			{
				endPosLabel.Text = "0:00";
				positionBar.Maximum = 0;
				positionBar.Value = 0;
				currentPosLabel.Text = Utility.FormatTimespan(TimeSpan.Zero, true);
			}
		}

		private void UpdateButtons()
		{
			backButton.Enabled = playPauseButton.Enabled = forwardButton.Enabled = _project != null;
			playPauseButton.IconChar = (_project?.PlaybackState.IsPlaying ?? false) ? IconChar.Pause : IconChar.Play;
		}

		public void UpdateGenerationContext()
		{
			if (_project == null)
			{
				return;
			}

			_videoGenerator.UpdateContext(_project, videoSkiaControl.Size);
		}

		private void UpdatePanelSize()
		{
			var size = (16, 9);
			if (_project != null)
			{
				size = (_project.Config.VideoSize.Width, _project.Config.VideoSize.Height);
			}

			Utility.ResizeContainerAspectRatio(videoPanel, videoSkiaControl, size);
		}

		private void backButton_Click(object sender, EventArgs e)
		{
			_project?.PlaybackState.AcquireVideoPlaybackClaim(this);
			_project?.PlaybackState.SeekRelative(-10.0f);
		}

		private void forwardButton_Click(object sender, EventArgs e)
		{
			_project?.PlaybackState.AcquireVideoPlaybackClaim(this);
			_project?.PlaybackState.SeekRelative(10.0f);
		}

		private void playPauseButton_Click(object sender, EventArgs e)
		{
			_project?.PlaybackState.AcquireVideoPlaybackClaim(this);
			_project?.PlaybackState.TogglePlay();
		}

		private void positionBar_Scroll(object sender, EventArgs e)
		{
			_project?.PlaybackState.AcquireVideoPlaybackClaim(this);
			_project?.PlaybackState.SeekAbsolute(positionBar.Value);
		}

		private void videoSkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			Render(e.Surface);
		}

		private void videoPanel_Paint(object sender, PaintEventArgs e)
		{
			UpdateState();
		}

		private void volumeSlider_VolumeChanged(object sender, EventArgs e)
		{
			AppSettings.Instance.SetVolume(volumeSlider.Volume);
		}
	}
}
