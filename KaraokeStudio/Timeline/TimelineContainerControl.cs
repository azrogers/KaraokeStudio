using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Managers;
using KaraokeStudio.Project;
using NLog;

namespace KaraokeStudio.Timeline
{
    public partial class TimelineContainerControl : UserControl
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		private UpdateDispatcher.Handle _projectHandle;
		private UpdateDispatcher.Handle _tracksUpdateHandle;

		private KaraokeProject? _currentProject;
		private List<TrackHeaderControl> _trackHeaders = new List<TrackHeaderControl>();

		public TimelineContainerControl()
		{
			InitializeComponent();

			Disposed += OnDispose;

			headersContainer.SuspendLayout();
			BackColor = VisualStyle.NeutralDarkColor;

			SelectionManager.OnSelectedTracksChanged += OnSelectedTracksChanged;
			timeline.OnTrackPositioningChanged += timeline_OnTrackPositioningChanged;

			_projectHandle = UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				_currentProject = update.Project;
				RecreateTracks();
			});

			_tracksUpdateHandle = UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				RecreateTracks();
			});
		}

		private void OnDispose(object? sender, EventArgs e)
		{
			SelectionManager.OnSelectedTracksChanged -= OnSelectedTracksChanged;
			_tracksUpdateHandle.Release();
			_projectHandle.Release();
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

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			timeline.OnProjectEventsChanged(project);

			RecreateTracks();
		}

		private void RecreateTracks()
		{
			foreach (var header in _trackHeaders)
			{
				// remove event listeners before destroying
				header.Click -= OnHeaderClick;
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
				headersContainer.Controls.Add(control);
				_trackHeaders.Add(control);
			}

			RepositionTracks();
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
