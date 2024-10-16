﻿using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Managers;
using KaraokeStudio.Util;
using System.Diagnostics;

namespace KaraokeStudio.Project
{
    /// <summary>
    /// Keeps track of playback position and handles audio playback across the project.
    /// </summary>
    internal class ProjectPlaybackState : IDisposable
	{
		private bool _isPlayingInternal = false;

		private System.Windows.Forms.Timer _timer;

		private double _position;
		private KaraokeProject _project;
		private float _playbackRate = 2.0f;

		private Stopwatch _stopwatch = new Stopwatch();
		private Stack<object> _claims = new Stack<object>();

		private UpdateDispatcher.Handle _projectConfigHandle;

		public event Action<bool>? OnPlayStateChanged;
		public event Action<double>? OnPositionChanged;

		/// <summary>
		/// The current object that's allowed to play video, if any.
		/// See <see cref="AcquireVideoPlaybackClaim(object)"/> and <see cref="ReleaseVideoPlaybackClaim(object)"/>.
		/// </summary>
		public object? CurrentVideoPlaybackClaim => _claims.Any() ? _claims.Peek() : null;

		/// <summary>
		/// The current playback position in seconds.
		/// </summary>
		public double Position => _position;

		public bool IsPlaying
		{
			get => _isPlayingInternal;
			private set
			{
				_isPlayingInternal = value;
				_stopwatch.Restart();
				_timer.Enabled = _isPlayingInternal;
				OnPlayStateChanged?.Invoke(_isPlayingInternal);

				AudioManager.Instance.SetPlaybackState(value);
			}
		}

		public ProjectPlaybackState(KaraokeProject project, IEnumerable<KaraokeTrack> tracks)
		{
			_playbackRate = AppSettings.Instance.PlaybackRate;
			AppSettings.Instance.OnPlaybackRateChanged += OnPlaybackRateChanged;

			_timer = new System.Windows.Forms.Timer();
			_timer.Enabled = false;
			_timer.Tick += OnTimerTick;

			_project = project;
			_timer.Interval = (int)Math.Round(1.0 / project.Config.FrameRate * 1000);

			_projectConfigHandle = UpdateDispatcher.RegisterHandler<ProjectConfigUpdate>(update =>
			{
				_project = update.Project;
				_timer.Interval = (int)Math.Round(1.0 / project.Config.FrameRate * 1000);
			});
		}

		private void OnPlaybackRateChanged(float rate)
		{
			_playbackRate = rate;
		}

		public void Dispose()
		{
			_projectConfigHandle.Release();
			_timer.Tick -= OnTimerTick;
			AppSettings.Instance.OnPlaybackRateChanged -= OnPlaybackRateChanged;
		}

		public void UpdateProjectLength()
		{
			_position = Math.Min(_position, _project.Length.TotalSeconds);
			OnPositionChanged?.Invoke(_position);
		}

		/// <summary>
		/// Cleanup objects for a clean exit.
		/// </summary>
		public void Cleanup()
		{
			_timer.Stop();
			_timer.Dispose();
		}

		/// <summary>
		/// Acquires a claim to be the object (a control, generally) currently playing video.
		/// </summary>
		/// <remarks>
		/// Internally, <see cref="ProjectPlaybackState"/> keeps a queue of the current object allowed to play video.
		/// The object acquiring the claim should reference <see cref="CurrentVideoPlaybackClaim"/> to see if its claim is currently priority.
		/// </remarks>
		/// <param name="owner">The object that wants to play video.</param>
		public void AcquireVideoPlaybackClaim(object owner)
		{
			if (_claims.Any(c => c == owner))
			{
				_claims = new Stack<object>(_claims.Where(c => c != owner));
			}

			_claims.Push(owner);
		}

		/// <summary>
		/// Release a previously held playback claim.
		/// See <see cref="AcquireVideoPlaybackClaim(object)"/> for more information.
		/// </summary>
		public void ReleaseVideoPlaybackClaim(object owner)
		{
			if (!_claims.Any(c => c == owner))
			{
				throw new ArgumentException($"Object {owner} tried to release claim on video playback but no claim acquired?");
			}

			_claims = new Stack<object>(_claims.Where(c => c != owner));
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

		public void OnTick()
		{
			if (!IsPlaying)
			{
				return;
			}

			var elapsed = _stopwatch.Elapsed.TotalSeconds;
			_stopwatch.Restart();

			_position += elapsed * _playbackRate;
			_position = Math.Min(_project.Length.TotalSeconds, _position);
			OnPositionChanged?.Invoke(_position);
		}

		public void SeekAbsolute(double position)
		{
			SetPosition(position);
		}

		public void SeekRelative(double offset) => SetPosition(_position + offset);

		private void SetPosition(double position)
		{
			position = Math.Clamp(position, 0, _project.Length.TotalSeconds);
			_position = position;
			_stopwatch.Restart();
			AudioManager.Instance.Seek(position);
			OnPositionChanged?.Invoke(position);
		}

		private void OnTimerTick(object? sender, EventArgs e)
		{
			OnTick();
		}
	}
}
