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

		public void SetConfig(ProjectConfig config)
		{
			if(Project != null)
			{
				Project.Config = config;
				IsPendingChanges = true;
			}
		}

		// create new project
		public void NewProject()
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			var audioFile = OpenAudioFile(null);
			if (audioFile == null)
			{
				return;
			}

			_loadedProject = KaraokeProject.Create(audioFile);
			_loadedProjectPath = null;
			IsPendingChanges = true;
			OnProjectChanged?.Invoke(_loadedProject);
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
			if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName))
			{
				return;
			}

			_loadedProject = KaraokeProject.Load(dialog.FileName);
			_loadedProjectPath = dialog.FileName;
			IsPendingChanges = false;
			OnProjectChanged?.Invoke(_loadedProject);
		}

		public void LoadMidiFile()
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			var dialog = new VistaOpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Multiselect = false;
			dialog.Title = "Open MIDI file";
			dialog.Filter = "Guitar Hero/Rock Band MIDI|notes.mid|MIDI files|*.mid|All files|*.*";
			if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName))
			{
				return;
			}

			var midiFile = dialog.FileName;
			var audioFile = OpenAudioFile(Path.GetDirectoryName(midiFile));
			if (audioFile == null)
			{
				return;
			}

			_loadedProject = KaraokeProject.FromMidi(midiFile, audioFile);
			_loadedProjectPath = null;
			IsPendingChanges = false;
			OnProjectChanged?.Invoke(_loadedProject);
		}

		// returns true if we can continue, or false if we're cancelling the operation after alerting of pending changes
		public bool AlertPendingChanges()
		{
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
			IsPendingChanges = false;
			return true;
		}

		private string? OpenAudioFile(string? baseDir)
		{
			var dialog = new VistaOpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Multiselect = false;
			dialog.Title = "Open audio file";
			dialog.Filter = "Vorbis audio|*.ogg|Wave audio|*.wav|All files|*.*";
			dialog.InitialDirectory = baseDir;
			if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName))
			{
				return null;
			}

			return dialog.FileName;
		}
	}
}
