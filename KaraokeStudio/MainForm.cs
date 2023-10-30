using KaraokeLib;
using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeStudio.FormHandlers;
using KaraokeStudio.Util;
using KaraokeStudio.Video;
using KaraokeStudio.LyricsEditor;

namespace KaraokeStudio
{
	public partial class MainForm : Form
	{
		private ProjectFormHandler _projectHandler;
		private StyleForm _styleForm;
		private SyncForm _syncForm;
		private ConsoleForm _consoleForm;

		// designer doesn't like handling this one itself so we set it up manually
		private LyricsEditorControl lyricsEditor;

		private KaraokeEvent? _selectedEvent = null;
		private KaraokeTrack? _selectedTrack = null;

		public MainForm()
		{
			ExceptionLogger.ParentForm = this;

			InitializeComponent();

			lyricsEditor = new LyricsEditorControl();
			videoSplit.Panel1.Controls.Add(lyricsEditor);
			lyricsEditor.Dock = DockStyle.Fill;
			lyricsEditor.Location = new Point(0, 0);
			lyricsEditor.Name = "lyricsEditor";
			lyricsEditor.Size = new Size(549, 297);
			lyricsEditor.TabIndex = 0;

			lyricsEditor.OnLyricsEventsChanged += OnLyricsEventsChanged;

			video.OnPositionChangedEvent += OnPositionChanged;
			timelineContainer.OnPositionChangedEvent += (newTime) =>
			{
				timelineContainer.OnPositionChanged(newTime);
				video.OnPositionChanged(newTime);
			};

			timelineContainer.OnEventSelectionChanged += OnEventSelectionChanged;
			timelineContainer.OnTrackSelectionChanged += OnTrackSelectionChanged;

			// handles the project itself
			_projectHandler = new ProjectFormHandler();
			_projectHandler.OnProjectChanged += OnProjectChanged;
			_projectHandler.OnPendingStateChanged += OnPendingStateChanged;
			_projectHandler.OnProjectWillChangeCallback = OnProjectWillChange;
			_projectHandler.OnTrackChanged += OnTrackChanged;

			_styleForm = new StyleForm();
			_styleForm.OnProjectConfigApplied += OnProjectConfigApplied;

			_syncForm = new SyncForm();
			_syncForm.OnSyncDataApplied += OnSyncDataApplied;

			_consoleForm = new ConsoleForm();

			OnProjectChanged(null);
		}

		public void LoadProject(string path)
		{
			_projectHandler.OpenProject(path);
		}

		private void OnSyncDataApplied(KaraokeTrack obj)
		{
			_projectHandler.SetEvents(obj, obj.Events);
		}

		private bool OnProjectWillChange()
		{
			return _syncForm.OnProjectWillChange();
		}

		private void OnPositionChanged(double newTime)
		{
			timelineContainer.OnPositionChanged(newTime);
			lyricsEditor.OnPositionChanged(newTime);
		}

		private void OnProjectConfigApplied(KaraokeConfig obj)
		{
			_projectHandler.SetConfig(obj);
			video.UpdateGenerationContext();
		}

		private void OnLyricsEventsChanged((KaraokeTrack Track, IEnumerable<KaraokeEvent> NewEvents) obj)
		{
			_projectHandler.SetEvents(obj.Track, obj.NewEvents);
			video.OnProjectEventsChanged(_projectHandler.Project);
			timelineContainer.OnProjectEventsChanged(_projectHandler.Project);
			lyricsEditor.OnProjectEventsChanged(_projectHandler.Project);
			_syncForm.OnProjectEventsChanged(_projectHandler.Project);
		}

		private void OnTrackChanged(KaraokeTrack obj)
		{
			video.OnProjectEventsChanged(_projectHandler.Project);
			timelineContainer.OnProjectEventsChanged(_projectHandler.Project);
			lyricsEditor.OnProjectEventsChanged(_projectHandler.Project);
		}

		private void OnPendingStateChanged(bool obj)
		{
			UpdateTitleAndMenu();
		}

		private void OnProjectChanged(KaraokeProject? project)
		{
			UpdateTitleAndMenu();

			video.OnProjectChanged(project);
			_styleForm.OnProjectChanged(project);
			timelineContainer.OnProjectChanged(project);
			lyricsEditor.OnProjectChanged(project);
			_syncForm.Hide();
		}

		private void OnTrackSelectionChanged(KaraokeTrack? obj)
		{
			_selectedTrack = obj;
			UpdateTitleAndMenu();
		}

		private void OnEventSelectionChanged(KaraokeEvent? obj)
		{
			_selectedEvent = obj;
			UpdateTitleAndMenu();
		}

		#region UI Events
		private void newToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.NewProject();

		private void openToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.OpenProject();

		private void saveToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.SaveProject();

		private void saveAsToolStripMenuitem_Click(object sender, EventArgs e) => _projectHandler.SaveProject(forceNewPath: true);

		private void midiToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.LoadMidiFile();

		private void lRCFileToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.ExportLrcFile();

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void editStyleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_styleForm.Show();
		}

		private void syncLyricsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_projectHandler.Project == null || _selectedTrack == null)
			{
				return;
			}

			if (_syncForm.IsDisposed)
			{
				_syncForm = new SyncForm();
			}

			_syncForm.Open(_projectHandler.Project, _selectedTrack);
		}

		private void audioToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_projectHandler.AddNewAudioTrack();
		}

		private void removeTrackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(_selectedTrack == null)
			{
				return;
			}

			// remove track
		}

		private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_consoleForm.Show();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!_projectHandler.AlertPendingChanges())
			{
				e.Cancel = true;
			}
		}
		#endregion

		private void UpdateTitleAndMenu()
		{
			var project = _projectHandler.Project;
			if (project == null)
			{
				Text = "Karaoke Studio";
			}
			else
			{
				Text = $"Karaoke Studio - {_projectHandler.ProjectPath ?? "New Project"}{(_projectHandler.IsPendingChanges ? " *" : "")}";
			}

			saveToolStripMenuItem.Enabled = project != null && _projectHandler.IsPendingChanges;
			// if we haven't saved once, save will open a dialog, so add the ...
			saveToolStripMenuItem.Text = (_projectHandler.ProjectPath != null ? "Save" : "Save...");
			// don't enable save as unless we've saved once
			saveAsToolStripMenuItem.Enabled = _projectHandler.ProjectPath != null;

			exportToolStripMenuItem.Enabled = project != null;

			openRecentToolStripMenuItem.DropDownItems.Clear();
			foreach (var file in AppSettings.Instance.RecentFiles)
			{
				openRecentToolStripMenuItem.DropDownItems.Add(file, null, (o, e) =>
				{
					_projectHandler.OpenProject(file);
				});
			}

			openRecentToolStripMenuItem.Enabled = openRecentToolStripMenuItem.HasDropDownItems;

			syncLyricsToolStripMenuItem.Enabled = _selectedTrack?.Type == KaraokeTrackType.Lyrics;
			removeTrackToolStripMenuItem.Enabled = _selectedTrack != null;
		}
	}
}