using KaraokeLib.Events;
using KaraokeLib.Tracks;
using NLog;

namespace KaraokeStudio.Timeline
{
    public partial class TimelineContainerControl : UserControl
	{
		private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

		// TODO: merge selection systems together? unified for multiple types?

		/// <summary>
		/// Called when the currently selected event has changed.
		/// </summary>
		public event Action<KaraokeEvent?>? OnEventSelectionChanged;

		/// <summary>
		/// Called when the currently selected header has changed.
		/// </summary>
		public event Action<KaraokeTrack?>? OnTrackSelectionChanged;

		/// <summary>
		/// Called when a track has its track settings changed.
		/// </summary>
		public event Action<KaraokeTrack>? OnTrackSettingsChanged;

		private int _selectedTrackId = -1;
		private KaraokeProject? _currentProject;
		private List<TrackHeaderControl> _trackHeaders = new List<TrackHeaderControl>();

		public TimelineContainerControl()
		{
			InitializeComponent();

			headersContainer.SuspendLayout();
			BackColor = VisualStyle.NeutralDarkColor;

			timeline.OnTrackPositioningChanged += timeline_OnTrackPositioningChanged;
			timeline.OnEventSelectionChanged += timeline_OnEventSelectionChanged;
		}

		private void timeline_OnEventSelectionChanged(KaraokeEvent? obj)
		{
			if(obj != null)
			{
				SetSelectedTrack(null);
			}
			OnEventSelectionChanged?.Invoke(obj);
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

		private void SetSelectedTrack(KaraokeTrack? track)
		{
			var oldTrackId = _selectedTrackId;
			_selectedTrackId = track?.Id ?? -1;
			if (_selectedTrackId != oldTrackId)
			{
				OnTrackSelectionChanged?.Invoke(track);
			}

			if(track != null)
			{
				timeline.DeselectEvent();
			}

			foreach (var header in _trackHeaders)
			{
				header.SetSelected(header.Track?.Id == _selectedTrackId);
			}
		}

		private void RecreateTracks()
		{
			foreach(var header in _trackHeaders)
			{
				// remove event listeners before destroying
				header.Click -= OnHeaderClick;
				header.OnTrackSettingsChanged -= OnHeaderTrackSettingsChanged;
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
				control.Click += OnHeaderClick;
				control.OnTrackSettingsChanged += OnHeaderTrackSettingsChanged;
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
			SetSelectedTrack(null);
			var headerControl = sender as TrackHeaderControl;
			if (headerControl == null)
			{
				return;
			}

			SetSelectedTrack(headerControl.Track);
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
