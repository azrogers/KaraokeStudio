using KaraokeStudio.FormHandlers;
using Ookii.Dialogs.WinForms;
using KaraokeStudio.Timeline;

namespace KaraokeStudio
{
    public partial class MainForm : Form
	{
		private ProjectFormHandler _projectHandler;
		private StyleForm _styleForm;

		public MainForm()
		{
			Logger.ParentForm = this;

			InitializeComponent();

			_projectHandler = new ProjectFormHandler();

			_styleForm = new StyleForm();
			_styleForm.OnProjectConfigApplied += OnProjectConfigApplied;

			_projectHandler.OnProjectChanged += OnProjectChanged;
			_projectHandler.OnPendingStateChanged += OnPendingStateChanged;

			video.OnPositionChanged += OnPositionChanged;

			OnProjectChanged(null);
		}

		public void LoadProject(string path)
		{
			_projectHandler.OpenProject(path);
		}

		private void OnPositionChanged(double newTime)
		{
			timeline.OnPositionChanged(newTime);
			lyricsEditor.OnPositionChanged(newTime);
		}

		private void OnProjectConfigApplied(ProjectConfig obj)
		{
			_projectHandler.SetConfig(obj);
			video.UpdateGenerationContext();
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
			timeline.OnProjectChanged(project);
			lyricsEditor.OnProjectChanged(project);
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

			exportToolStripMenuItem.Enabled = project != null;

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