using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Project;
using NLog;

namespace KaraokeStudio.Timeline
{
    public partial class TimelineContainerControl : UserControl
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		/// <summary>
		/// Called when a track has its track settings changed.
		/// </summary>
		public event Action<KaraokeTrack>? OnTrackSettingsChanged;

		/// <summary>
		/// Called when a track has an event change.
		/// </summary>
		public event Action<KaraokeTrack>? OnTrackEventsChanged;

		private int _selectedTrackId = -1;
		private KaraokeProject? _currentProject;
		private List<TrackHeaderControl> _trackHeaders = new List<TrackHeaderControl>();

		public TimelineContainerControl()
		{
			InitializeComponent();

			headersContainer.SuspendLayout();
			BackColor = VisualStyle.NeutralDarkColor;

			SelectionManager.OnSelectedTracksChanged += OnSelectedTracksChanged;
			timeline.OnTrackPositioningChanged += timeline_OnTrackPositioningChanged;
			timeline.OnTrackEventsChanged += Timeline_OnTrackEventsChanged;
		}

		~TimelineContainerControl()
		{
			SelectionManager.OnSelectedTracksChanged -= OnSelectedTracksChanged;
		}

		private void OnSelectedTracksChanged()
		{
			var selectedTrackIds = SelectionManager.SelectedTracks.Select(t => t.Id).ToHashSet();
			foreach (var header in _trackHeaders)
			{
				header.SetSelected(selectedTrackIds.Contains(header.Track?.Id ?? -1));
			}
		}

		private void timeline_OnTrackPositioningChanged()
		{
			RepositionTracks();
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			timeline.OnProjectChanged(project);

			_currentProject = project;
			_selectedTrackId = -1;
			RecreateTracks();
		}

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			timeline.OnProjectEventsChanged(project);

			RecreateTracks();
		}

		private void RecreateTracks()
		{
			foreach(var header in _trackHeaders)
			{
				// remove event listeners before destroying
				header.Click -= OnHeaderClick;
				header.OnTrackConfigChanged -= OnHeaderTrackSettingsChanged;
			}

			headersContainer.Controls.Clear();
			_trackHeaders.Clear();

			if (_currentProject == null)
			{
				return;
			}

			foreach (var track in _currentProject.Tracks)
			{
				var control = new TrackHeaderControl();
				control.Track = track;
				control.Project = _currentProject;
				control.Click += OnHeaderClick;
				control.OnTrackConfigChanged += OnHeaderTrackSettingsChanged;
				headersContainer.Controls.Add(control);
				_trackHeaders.Add(control);
			}

			RepositionTracks();
		}

		private void OnHeaderTrackSettingsChanged(KaraokeTrack obj)
		{
			OnTrackSettingsChanged?.Invoke(obj);
		}

		private void OnHeaderClick(object? sender, EventArgs e)
		{
			var headerControl = sender as TrackHeaderControl;
			if (headerControl == null || headerControl.Track == null)
			{
				SelectionManager.Deselect();
				return;
			}

			SelectionManager.Select(headerControl.Track, !ModifierKeys.HasFlag(Keys.Shift));
		}

		private void Timeline_OnTrackEventsChanged(KaraokeTrack obj)
		{
			OnTrackEventsChanged?.Invoke(obj);
		}

		private void RepositionTracks()
		{
			foreach (var header in _trackHeaders)
			{
				if (header.Track == null)
				{
					Logger.Warn($"Header missing a Track value?");
					continue;
				}

				var rect = timeline.GetTrackRect(header.Track.Id);
				header.Size = new Size(
					headersContainer.Width,
					(int)rect.Height);
				header.Location = new Point(0, (int)rect.Y);
			}
		}

		private void headersContainer_Layout(object sender, LayoutEventArgs e)
		{
			RepositionTracks();
		}

		private void headersContainer_SizeChanged(object sender, EventArgs e)
		{
			RepositionTracks();
		}
	}
}
