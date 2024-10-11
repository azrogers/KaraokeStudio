using KaraokeLib.Audio;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.FormHandlers;
using KaraokeStudio.LyricsEditor;
using KaraokeStudio.Managers;
using KaraokeStudio.Project;
using KaraokeStudio.Util;

namespace KaraokeStudio
{
	public partial class MainForm : Form
	{
		private ProjectFormHandler _projectHandler;

		private UpdateDispatcher.Handle _eventUpdateHandle;

		// designer doesn't like handling this one itself so we set it up manually
		private LyricsEditorControl lyricsEditor;

		private ContextMenuStrip _trackRightClickMenu;
		private ContextMenuStrip _eventRightClickMenu;

		public static MainForm? Instance { get; private set; } = null;

		public MainForm()
		{
			ExceptionLogger.ParentForm = this;
			if (Instance == null)
			{
				Instance = this;
			}

			InitializeComponent();

			_trackRightClickMenu = new ContextMenuStrip();
			_trackRightClickMenu.Items.AddRange(trackToolStripMenuItem.DropDownItems);

			_eventRightClickMenu = new ContextMenuStrip();
			_eventRightClickMenu.Items.AddRange(eventToolStripMenuItem.DropDownItems);

			lyricsEditor = new LyricsEditorControl();
			videoSplit.Panel1.Controls.Add(lyricsEditor);
			lyricsEditor.Dock = DockStyle.Fill;
			lyricsEditor.Location = new Point(0, 0);
			lyricsEditor.Name = "lyricsEditor";
			lyricsEditor.Size = new Size(549, 297);
			lyricsEditor.TabIndex = 0;

			SelectionManager.OnSelectedEventsChanged += OnEventSelectionChanged;
			SelectionManager.OnSelectedTracksChanged += OnTrackSelectionChanged;

			UndoHandler.OnUndoItemsChanged += OnUndoItemsChanged;

			// handles the project itself
			_projectHandler = new ProjectFormHandler();
			_projectHandler.OnPendingStateChanged += OnPendingStateChanged;
			_projectHandler.OnProjectWillChangeCallback = WindowManager.OnProjectWillChange;

			_eventUpdateHandle = UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				video.OnProjectEventsChanged(_projectHandler.Project);
			});

			UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				video.OnProjectEventsChanged(_projectHandler.Project);
			});

			UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				UndoHandler.Clear();
				SelectionManager.Deselect();
				video.OnProjectChanged(update.Project);
			});

			// send a project update to get the ball rolling
			UpdateDispatcher.Dispatch(new ProjectUpdate(null, null));
		}

		public void LoadProject(string path)
		{
			_projectHandler.OpenProject(path);
		}

		public void OpenRightClickMenu(RightClickMenu menu)
		{
			if(menu == RightClickMenu.Track)
			{
				_trackRightClickMenu.Show(Cursor.Position);
			}
			else if(menu == RightClickMenu.Event)
			{
				_eventRightClickMenu.Show(Cursor.Position);
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

		private void audioSettingsToolStripMenuItem_Click(object sender, EventArgs e) => CommandDispatcher.Dispatch(new OpenAudioSettingsCommand());

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void editStyleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandDispatcher.Dispatch(new OpenStyleCommand());
		}

		private void syncLyricsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (_projectHandler.Project == null || SelectionManager.SelectedTracks.Count() != 1)
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
			if (_projectHandler.Project == null || !SelectionManager.SelectedTracks.Any())
			{
				return;
			}

			CommandDispatcher.Dispatch(new RemoveTracksCommand(SelectionManager.SelectedTracks.ToArray()));
		}

		private void consoleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandDispatcher.Dispatch(new OpenConsoleCommand());
		}

		private void undoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UndoHandler.Undo();
		}

		private void redoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UndoHandler.Redo();
		}

		private void exportVideoToolStripMenuItem_Click(object sender, EventArgs e)
		{
			CommandDispatcher.Dispatch(new OpenExportVideoCommand());
		}

		private void addAudioClipToolStripMenuItem_Click(object sender, EventArgs e)
		{
			var track = SelectionManager.SelectedTracks.FirstOrDefault(t => t.Type == KaraokeTrackType.Audio);
			if (track == null)
			{
				return;
			}

			var audioClip = ProjectUtil.OpenAudioFile(_projectHandler.ProjectPath);
			if (audioClip == null)
			{
				return;
			}

			var audioInfo = AudioUtil.GetFileInfo(audioClip);
			if (audioInfo == null)
			{
				MessageBox.Show("Failed to obtain info from audio file - not a supported format?", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			var start = new TimeSpanTimecode(_projectHandler.Project?.PlaybackState.Position ?? 0);
			var end = new TimeSpanTimecode(start.GetTimeSeconds() + audioInfo.LengthSeconds);
			CommandDispatcher.Dispatch(new AddAudioClipEventCommand(track, new AudioClipSettings(audioClip), start, end));
		}

		private void removeEventToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!SelectionManager.SelectedEvents.Any())
			{
				return;
			}

			CommandDispatcher.Dispatch(new RemoveEventsCommand(SelectionManager.SelectedEvents.Select(ev => ev.Id).ToArray()));
		}

		private void eventPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{

		}

		private void trackPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!SelectionManager.SelectedTracks.Any())
			{
				return;
			}

			CommandDispatcher.Dispatch(new OpenTrackSettingsCommand(SelectionManager.SelectedTracks.First()));
		}

		private void moveUpToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!SelectionManager.SelectedTracks.Any())
			{
				return;
			}

			var orderedTracks = _projectHandler.Project?.Tracks.OrderBy(t => t.Order).ToArray() ?? [];
			var selectedIds = new HashSet<int>(SelectionManager.SelectedTracks.Select(t => t.Id));
			var indexAbove = -1;
			for(var i = 0; i < orderedTracks.Length; i++)
			{
				if (selectedIds.Contains(orderedTracks[i].Id))
				{
					indexAbove = i - 1;
					break;
				}
			}

            if (indexAbove >= 0)
            {
				CommandDispatcher.Dispatch(new RepositionTracksCommand(SelectionManager.SelectedTracks.ToArray(), indexAbove));
            }
        }

		private void moveDownToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!SelectionManager.SelectedTracks.Any())
			{
				return;
			}

			var orderedTracks = _projectHandler.Project?.Tracks.OrderBy(t => t.Order).ToArray() ?? [];
			var selectedIds = new HashSet<int>(SelectionManager.SelectedTracks.Select(t => t.Id));
			var indexBelow = -1;
			for (var i = orderedTracks.Length - 1; i >= 0; i--)
			{
				if (selectedIds.Contains(orderedTracks[i].Id))
				{
					indexBelow = i + 1;
					break;
				}
			}

			if (indexBelow >= 0)
			{
				CommandDispatcher.Dispatch(new RepositionTracksCommand(SelectionManager.SelectedTracks.ToArray(), indexBelow));
			}
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
			_eventUpdateHandle.Release();

			if (_projectHandler.Project != null)
			{
				_projectHandler.Project.PlaybackState.Cleanup();
			}

			AudioManager.Instance.Cleanup();
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

			redoToolStripMenuItem.Enabled = UndoHandler.CurrentRedoItem != null;
			redoToolStripMenuItem.Text = UndoHandler.CurrentRedoItem == null ? "Redo" : "Redo " + UndoHandler.CurrentRedoItem.Value.Action;

			moveUpToolStripMenuItem.Enabled = SelectionManager.SelectedTracks.Any();
			moveDownToolStripMenuItem.Enabled = SelectionManager.SelectedTracks.Any();
		}

		public enum RightClickMenu
		{
			Track,
			Event
		}
	}
}