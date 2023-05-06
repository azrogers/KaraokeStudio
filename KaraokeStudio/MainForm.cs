using KaraokeStudio.FormHandlers;
using Ookii.Dialogs.WinForms;

namespace KaraokeStudio
{
    public partial class MainForm : Form
	{
		private ProjectFormHandler _projectHandler;
		private VideoFormHandler _videoHandler;
		private StyleForm _styleForm;
		private System.Windows.Forms.Timer _timer;

		public MainForm()
		{
			Logger.ParentForm = this;

			InitializeComponent();

			_projectHandler = new ProjectFormHandler();
			_videoHandler = new VideoFormHandler(
				videoPanel,
				videoSkiaControl,
				positionBar,
				new Label[] { startPosLabel, currentPosLabel, endPosLabel },
				new Button[] { backButton, playPauseButton, forwardButton });
			_styleForm = new StyleForm();
			_styleForm.OnProjectConfigApplied += OnProjectConfigApplied;

			_videoHandler.OnPlayStateChanged += OnPlayStateChanged;

			_projectHandler.OnProjectChanged += OnProjectChanged;
			_projectHandler.OnPendingStateChanged += OnPendingStateChanged;

			_timer = new System.Windows.Forms.Timer();
			_timer.Enabled = false;
			_timer.Tick += OnTimerTick;

			OnProjectChanged(null);
		}

		private void OnProjectConfigApplied(ProjectConfig obj)
		{
			_projectHandler.SetConfig(obj);
		}

		private void OnTimerTick(object? sender, EventArgs e)
		{
			// tell the video panel we need to repaint
			videoSkiaControl.Invalidate();
		}

		private void OnPlayStateChanged(bool isPlaying)
		{
			_timer.Enabled = isPlaying;
		}

		private void OnPendingStateChanged(bool obj)
		{
			UpdateTitleAndMenu();
		}

		private void OnProjectChanged(KaraokeProject? project)
		{
			UpdateTitleAndMenu();

			_videoHandler.OnProjectChanged(project);
			_styleForm.OnProjectChanged(project);

			// set ms to desired framerate
			if(project != null)
			{
				_timer.Interval = (int)Math.Round((1.0 / project.Config.FrameRate) * 1000);
			}
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

		private void backButton_Click(object sender, EventArgs e) => _videoHandler.Rewind();

		private void forwardButton_Click(object sender, EventArgs e) => _videoHandler.FastForward();

		private void playPauseButton_Click(object sender, EventArgs e) => _videoHandler.TogglePlay();

		private void positionBar_Scroll(object sender, EventArgs e) => _videoHandler.Seek(positionBar.Value);

		private void videoSkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			_videoHandler.OnTick();
			_videoHandler.Render(e.Surface);
		}

		private void videoPanel_Paint(object sender, PaintEventArgs e)
		{
			_videoHandler.UpdateState();
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
		}
	}
}