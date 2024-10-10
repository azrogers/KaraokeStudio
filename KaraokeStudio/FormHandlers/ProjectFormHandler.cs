using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;
using KaraokeStudio.Util;
using Ookii.Dialogs.WinForms;

namespace KaraokeStudio.FormHandlers
{
    // handles behaviors for the MainForm related to the current project
    internal class ProjectFormHandler
	{
		private KaraokeProject? _loadedProject;
		private string? _loadedProjectPath = null;
		private bool _pendingChangesInternal = false;

		public KaraokeProject? Project => _loadedProject;

		public string? ProjectPath => _loadedProjectPath;

		/// <summary>
		/// Called when the state of pending changes in the project changes.
		/// </summary>
		public event Action<bool>? OnPendingStateChanged;

		/// <summary>
		/// Called when the project changes (new project, open project)
		/// </summary>
		public event Action<KaraokeProject?>? OnProjectChanged;

		/// <summary>
		/// Callback that's run when the project is about to change.
		/// Return false to block the change.
		/// </summary>
		public Func<bool>? OnProjectWillChangeCallback;

		/// <summary>
		/// Are there unsaved changes to the project?
		/// </summary>
		public bool IsPendingChanges
		{
			get => _pendingChangesInternal;
			private set
			{
				_pendingChangesInternal = value;
				OnPendingStateChanged?.Invoke(value);
			}
		}

		public ProjectFormHandler()
		{
			UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				var idLookup = new HashSet<int>(update.EventIds);
				if(Project != null)
				{
					var relevantTracks = Project.Tracks.Where(t => t.Events.Any(e => idLookup.Contains(e.Id)));
					foreach(var track in relevantTracks)
					{
						track.UpdateEvents();
					}
				}

				RecalculateProjectLength();
				_loadedProject?.UpdateMixer();
				IsPendingChanges = true;
			});

			UpdateDispatcher.RegisterHandler<TrackConfigUpdate>(update =>
			{
				IsPendingChanges = true;
			});

			UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				if(_loadedProject != null)
				{
					_loadedProject.Dispose();
				}

				_loadedProject = update.Project;
				_loadedProjectPath = update.Project != null ? update.ProjectPath : null;
				if (_loadedProjectPath != null)
				{
					AppSettings.Instance.AddRecentFile(_loadedProjectPath);
				}
				IsPendingChanges = update.Project != null && update.ProjectPath == null;
			});

			UpdateDispatcher.RegisterHandler<ProjectConfigUpdate>(update =>
			{
				IsPendingChanges = true;
			});
		}

		public void RecalculateProjectLength()
		{
			if(_loadedProject == null)
			{
				return;
			}

			var lengthSeconds = _loadedProject.Tracks.Any() ? _loadedProject.Tracks.Max(t => t.Events.Any() ? t.Events.Max(e => e.EndTimeSeconds) : 0) : 0;
			_loadedProject.Length = TimeSpan.FromSeconds(lengthSeconds);
			_loadedProject.PlaybackState.UpdateProjectLength();
		}

		// create new project
		public void NewProject()
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			CommandDispatcher.Dispatch(new NewProjectCommand());
		}

		public void OpenProject(string file)
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			CommandDispatcher.Dispatch(new OpenProjectCommand(file));
		}

		// open existing project
		public void OpenProject()
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			var dialog = new VistaOpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Multiselect = false;
			dialog.Title = "Open Karaoke Studio file";
			dialog.Filter = "Karaoke Studio file|*.ksf|All files|*.*";
			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			OpenProject(dialog.FileName);
		}

		public void LoadMidiFile()
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			CommandDispatcher.Dispatch(new LoadMidiFileCommand());
		}

		public void ExportLrcFile()
		{
			if (_loadedProject == null)
			{
				return;
			}

			var dialog = new VistaOpenFileDialog();
			dialog.CheckFileExists = false;
			dialog.Multiselect = false;
			dialog.Title = "Export LRC file";
			dialog.Filter = "Enhanced LRC file|*.lrc|All files|*.*";
			if (dialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			var outPath = dialog.FileName;
			var lrcFile = new LrcKaraokeFile(_loadedProject.Tracks);
			using (var output = File.OpenWrite(outPath))
			{
				lrcFile.Save(output);
			}
		}

		// returns true if we can continue, or false if we're cancelling the operation after alerting of pending changes
		public bool AlertPendingChanges()
		{
			if (!OnProjectWillChangeCallback?.Invoke() ?? false)
			{
				return false;
			}

			if (!IsPendingChanges)
			{
				return true;
			}

			var response = MessageBox.Show(
				"Pending changes will be lost! Would you like to save?",
				"Pending Changes",
				MessageBoxButtons.YesNoCancel,
				MessageBoxIcon.Warning);

			if (response == DialogResult.Cancel)
			{
				return false;
			}
			else if (response == DialogResult.No)
			{
				return true;
			}

			return SaveProject();
		}

		// returns true if we saved successfully, or false if cancelled
		public bool SaveProject(bool forceNewPath = false)
		{
			if (_loadedProject == null)
			{
				IsPendingChanges = false;
				return true;
			}

			if (_loadedProjectPath == null || forceNewPath)
			{
				var dialog = new VistaSaveFileDialog();
				dialog.Title = "Save project file";
				dialog.Filter = "Karaoke Studio projects|*.ksf|All files|*.*";
				dialog.AddExtension = true;
				dialog.DefaultExt = "ksf";
				if (dialog.ShowDialog() != DialogResult.OK || !Directory.Exists(Path.GetDirectoryName(dialog.FileName)))
				{
					return false;
				}

				_loadedProjectPath = dialog.FileName;
				if (string.IsNullOrWhiteSpace(Path.GetExtension(_loadedProjectPath)))
				{
					_loadedProjectPath += ".ksf";
				}
			}

			_loadedProject.Save(_loadedProjectPath);
			AppSettings.Instance.AddRecentFile(_loadedProjectPath);
			IsPendingChanges = false;
			return true;
		}

		public bool AddNewAudioTrack()
		{
			if (_loadedProject == null)
			{
				return false;
			}

			var audioFile = ProjectUtil.OpenAudioFile(_loadedProjectPath ?? Environment.CurrentDirectory);
			if (audioFile == null)
			{
				return false;
			}

			var info = AudioUtil.GetFileInfo(audioFile);
			if (info == null)
			{
				return false;
			}

			var settings = new AudioClipSettings(audioFile);
			CommandDispatcher.Dispatch(new AddAudioTrackCommand(settings, new TimeSpanTimecode(0), new TimeSpanTimecode(info.LengthSeconds)));

			IsPendingChanges = true;
			_loadedProject?.UpdateMixer();

			return true;
		}
	}
}
