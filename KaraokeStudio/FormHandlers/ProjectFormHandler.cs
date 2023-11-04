using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files;
using KaraokeLib.Tracks;
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
		/// Called when a track is added or removed.
		/// </summary>
		public event Action<KaraokeTrack>? OnTrackChanged;

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

		public void SetConfig(KaraokeConfig config)
		{
			if (Project != null)
			{
				Project.Config = config;
				IsPendingChanges = true;
			}
		}

		public void SetEvents(KaraokeTrack track, IEnumerable<KaraokeEvent> events)
		{
			track.ReplaceEvents(events);
			RecalculateProjectLength();
			_loadedProject?.UpdateMixer();
			IsPendingChanges = true;
		}

		public void UpdateEvents(KaraokeTrack track)
		{
			track.UpdateEvents();
			RecalculateProjectLength();
			_loadedProject?.UpdateMixer();
			IsPendingChanges = true;
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

		public void UpdateTrackSettings(KaraokeTrack track)
		{
			IsPendingChanges = true;
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

		public void OpenProject(string file)
		{
			if (!AlertPendingChanges())
			{
				return;
			}

			if (!File.Exists(file))
			{
				return;
			}

			_loadedProject = KaraokeProject.Load(file);
			_loadedProjectPath = _loadedProject != null ? file : null;
			IsPendingChanges = false;
			if (_loadedProjectPath != null)
			{
				AppSettings.Instance.AddRecentFile(_loadedProjectPath);
			}
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
			IsPendingChanges = true;
			OnProjectChanged?.Invoke(_loadedProject);
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
			if (_loadedProjectPath == null || _loadedProject == null)
			{
				return false;
			}

			var audioFile = OpenAudioFile(_loadedProjectPath);
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

			var track = _loadedProject.AddTrack(KaraokeTrackType.Audio);
			track.AddAudioClipEvent(settings, new TimeSpanTimecode(0), new TimeSpanTimecode(info.LengthSeconds));

			IsPendingChanges = true;
			_loadedProject?.UpdateMixer();
			OnTrackChanged?.Invoke(track);

			return true;
		}

		private string? OpenAudioFile(string? baseDir)
		{
			var dialog = new VistaOpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Multiselect = false;
			dialog.Title = "Open audio file";
			dialog.Filter = "Audio file|*.mp3;*.ogg;*.wav|All files|*.*";
			dialog.InitialDirectory = baseDir;
			if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName))
			{
				return null;
			}

			return dialog.FileName;
		}
	}
}
