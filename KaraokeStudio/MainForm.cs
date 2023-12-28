using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.FormHandlers;
using KaraokeStudio.LyricsEditor;
using KaraokeStudio.Project;
using KaraokeStudio.Util;

namespace KaraokeStudio
{
	public partial class MainForm : Form
	{
		private ProjectFormHandler _projectHandler;
		private StyleForm _styleForm;
		private ConsoleForm _consoleForm;
		private ExportVideoForm? _exportVideoForm;

		private UpdateDispatcher.Handle _eventUpdateHandle;

		// designer doesn't like handling this one itself so we set it up manually
		private LyricsEditorControl lyricsEditor;

		public static MainForm? Instance { get; private set; } = null;

		public MainForm()
		{
			ExceptionLogger.ParentForm = this;
			if (Instance == null)
			{
				Instance = this;
			}

			InitializeComponent();

			lyricsEditor = new LyricsEditorControl();
			videoSplit.Panel1.Controls.Add(lyricsEditor);
			lyricsEditor.Dock = DockStyle.Fill;
			lyricsEditor.Location = new Point(0, 0);
			lyricsEditor.Name = "lyricsEditor";
			lyricsEditor.Size = new Size(549, 297);
			lyricsEditor.TabIndex = 0;

			lyricsEditor.OnLyricsEventsChanged += OnLyricsEventsChanged;

			SelectionManager.OnSelectedEventsChanged += OnEventSelectionChanged;
			SelectionManager.OnSelectedTracksChanged += OnTrackSelectionChanged;

			UndoHandler.OnUndoItemsChanged += OnUndoItemsChanged;

			// handles the project itself
			_projectHandler = new ProjectFormHandler();
			_projectHandler.OnProjectChanged += OnProjectChanged;
			_projectHandler.OnPendingStateChanged += OnPendingStateChanged;
			_projectHandler.OnProjectWillChangeCallback = WindowManager.OnProjectWillChange;

			_styleForm = new StyleForm();
			_styleForm.OnProjectConfigApplied += OnProjectConfigApplied;

			_consoleForm = new ConsoleForm();

			_eventUpdateHandle = UpdateDispatcher.RegisterHandler<EventTimingsUpdate>(update =>
			{
				video.OnProjectEventsChanged(_projectHandler.Project);
			});

			UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				video.OnProjectEventsChanged(_projectHandler.Project);
			});

			OnProjectChanged(null);
		}

		public void LoadProject(string path)
		{
			_projectHandler.OpenProject(path);
		}

		private void OnProjectConfigApplied(KaraokeConfig obj)
		{
			_projectHandler.SetConfig(obj);

			if (_projectHandler.Project != null)
			{
				_projectHandler.Project.PlaybackState.OnProjectConfigChanged(_projectHandler.Project);
			}

			video.UpdateGenerationContext();
		}

		private void OnLyricsEventsChanged((KaraokeTrack Track, IEnumerable<KaraokeEvent> NewEvents) obj)
		{
			UndoHandler.Clear();
			SelectionManager.Deselect();
			_projectHandler.SetEvents(obj.Track, obj.NewEvents);
			video.OnProjectEventsChanged(_projectHandler.Project);
			timelineContainer.OnProjectEventsChanged(_projectHandler.Project);
			lyricsEditor.OnProjectEventsChanged(_projectHandler.Project);
			if (_projectHandler.Project != null)
			{
				WindowManager.OnLyricsEventsChanged(_projectHandler.Project, obj);
			}
		}

		private void OnUndoItemsChanged()
		{
			UpdateTitleAndMenu();
		}

		private void OnPendingStateChanged(bool obj)
		{
			UpdateTitleAndMenu();
		}

		private void OnProjectChanged(KaraokeProject? project)
		{
			UpdateTitleAndMenu();
			CommandDispatcher.CurrentContext.Project = project;

			UndoHandler.Clear();
			video.OnProjectChanged(project);
			_styleForm.OnProjectChanged(project);
			timelineContainer.OnProjectChanged(project);
			lyricsEditor.OnProjectChanged(project);
			WindowManager.OnProjectChanged(project);
			SelectionManager.Deselect();
			if (_exportVideoForm != null && !_exportVideoForm.IsDisposed)
			{
				_exportVideoForm.OnProjectChanged(_projectHandler.Project);
			}
		}

		private void OnTrackSelectionChanged()
		{
			UpdateTitleAndMenu();
		}

		private void OnEventSelectionChanged()
		{
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
			if (_styleForm.Visible)
			{
				_styleForm.Focus();
			}
			else
			{
				_styleForm.Show();
			}
		}

		private void syncLyricsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_projectHandler.Project == null || SelectionManager.SelectedTracks.Count() == 1)
			{
				return;
			}

			CommandDispatcher.Dispatch(new OpenSyncFormCommand(_projectHandler.Project, SelectionManager.SelectedTracks.First()));
		}

		private void audioToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_projectHandler.AddNewAudioTrack();
		}

		private void removeTrackToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!SelectionManager.SelectedTracks.Any())
			{
				return;
			}

			// remove track
		}

		private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_consoleForm.Show();
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UndoHandler.Undo();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (!_projectHandler.AlertPendingChanges())
			{
				e.Cancel = true;
			}
		}

		private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_projectHandler.Project != null)
			{
				_projectHandler.Project.PlaybackState.Cleanup();
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

			syncLyricsToolStripMenuItem.Enabled = SelectionManager.SelectedTracks.Count() == 1 && SelectionManager.SelectedTracks.First().Type == KaraokeTrackType.Lyrics;
			removeTrackToolStripMenuItem.Enabled = SelectionManager.SelectedTracks.Any();
			trackPropertiesToolStripMenuItem.Enabled = SelectionManager.SelectedTracks.Any();

			addAudioClipToolStripMenuItem.Enabled = SelectionManager.SelectedTracks.Any(t => t.Type == KaraokeTrackType.Audio);
			removeEventToolStripMenuItem.Enabled = SelectionManager.SelectedEvents.Any();
			eventPropertiesToolStripMenuItem.Enabled = SelectionManager.SelectedEvents.Any();

			undoToolStripMenuItem.Enabled = UndoHandler.CurrentItem != null;
			undoToolStripMenuItem.Text = UndoHandler.CurrentItem == null ? "Undo" : "Undo " + UndoHandler.CurrentItem.Value.Action;
		}

		private void exportVideoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_exportVideoForm != null && _exportVideoForm.Visible)
			{
				_exportVideoForm.Focus();
				return;
			}

			if (_exportVideoForm == null || _exportVideoForm.IsDisposed)
			{
				_exportVideoForm = new ExportVideoForm();
			}

			_exportVideoForm.OnProjectChanged(_projectHandler.Project);
			_exportVideoForm.Show();
		}

		private void addAudioClipToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void removeEventToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void eventPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void trackPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if(!SelectionManager.SelectedTracks.Any())
			{
				return;
			}

			CommandDispatcher.Dispatch(new OpenTrackSettingsCommand(SelectionManager.SelectedTracks.First()));
		}
	}
}