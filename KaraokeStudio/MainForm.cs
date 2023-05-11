using KaraokeStudio.FormHandlers;
using Ookii.Dialogs.WinForms;

namespace KaraokeStudio
{
    public partial class MainForm : Form
	{
		private ProjectFormHandler _projectHandler;
		private AudioFormHandler _audioHandler;
		private StyleForm _styleForm;

		public MainForm()
		{
			Logger.ParentForm = this;

			InitializeComponent();

			_projectHandler = new ProjectFormHandler();
			_audioHandler = new AudioFormHandler();

			_styleForm = new StyleForm();
			_styleForm.OnProjectConfigApplied += OnProjectConfigApplied;

			video.OnSeek += OnVideoSeek;
			video.OnPlayStateChanged += OnPlayStateChanged;

			_projectHandler.OnProjectChanged += OnProjectChanged;
			_projectHandler.OnPendingStateChanged += OnPendingStateChanged;

			OnProjectChanged(null);
		}

		private void OnVideoSeek(double pos)
		{
			_audioHandler.OnPlaybackStateChanged(video.IsPlaying, pos);
		}

		private void OnProjectConfigApplied(ProjectConfig obj)
		{
			_projectHandler.SetConfig(obj);
			video.UpdateGenerationContext();
		}

		private void OnPlayStateChanged(bool isPlaying)
		{
			_audioHandler.OnPlaybackStateChanged(isPlaying, video.Position);
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
			_audioHandler.OnProjectChanged(project);
		}

		#region UI Events
		private void newToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.NewProject();

		private void openToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.OpenProject();

		private void saveToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.SaveProject();

		private void saveAsToolStripMenuitem_Click(object sender, EventArgs e) => _projectHandler.SaveProject(forceNewPath: true);

		private void midiToolStripMenuItem_Click(object sender, EventArgs e) => _projectHandler.LoadMidiFile();

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Close();
		}

		private void editStyleToolStripMenuItem_Click(object sender, EventArgs e)
		{
			_styleForm.Show();
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if(!_projectHandler.AlertPendingChanges())
			{
				e.Cancel = true;
			}
		}
		#endregion
	
		private void UpdateTitleAndMenu()
		{
			var project = _projectHandler.Project;
			if(project == null)
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

			openRecentToolStripMenuItem.DropDownItems.Clear();
			foreach(var file in AppSettings.Instance.RecentFiles)
			{
				openRecentToolStripMenuItem.DropDownItems.Add(file, null, (o, e) =>
				{
					_projectHandler.OpenProject(file);
				});
			}

			openRecentToolStripMenuItem.Enabled = openRecentToolStripMenuItem.HasDropDownItems;
		}
	}
}