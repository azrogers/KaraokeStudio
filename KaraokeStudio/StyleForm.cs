using KaraokeLib.Config;
using KaraokeStudio.Config;
using KaraokeStudio.Util;
using Ookii.Dialogs.WinForms;

namespace KaraokeStudio
{
    public partial class StyleForm : Form
	{
		private KaraokeProject? _currentProject = null;
		private ConfigPreviewHandler _previewHandler = new ConfigPreviewHandler();

		private bool _isDirty = false;

		internal event Action<KaraokeConfig>? OnProjectConfigApplied;

		public StyleForm()
		{
			InitializeComponent();

			configEditor.Config = new KaraokeConfig();

			_isDirty = false;
			UpdateDirtyUI();
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_currentProject = project;

			// switching from one project to another with pending changes, don't overwrite the changes
			if (_isDirty)
			{
				UpdateDirtyFlag(true);
				return;
			}

			if (project == null)
			{
				configEditor.Config = new KaraokeConfig();
			}
			else
			{
				configEditor.Config = project.Config.CopyTyped();
			}

			UpdateDirtyFlag(false);
			UpdatePreview();
		}


		private void UpdateDirtyFlag(bool isDirty)
		{
			_isDirty = isDirty;
			UpdateDirtyUI();
		}

		private void UpdateDirtyUI()
		{
			applyButton.Enabled = _currentProject != null && _isDirty;
			revertButton.Enabled = _isDirty;
		}

		private void UpdatePanelSize(KaraokeConfig config)
		{
			var size = (config.VideoSize.Width, config.VideoSize.Height);
			Utility.ResizeContainerAspectRatio(videoPanel, previewSkiaControl, size, false);
		}

		private void UpdatePreview()
		{
			var config = configEditor.GetConfig<KaraokeConfig>();
			UpdatePanelSize(config);
			_previewHandler.UpdatePreview(config, (previewSkiaControl.Width, previewSkiaControl.Height));
			previewSkiaControl.Invalidate();
		}

		private void configEditor_OnValueChanged()
		{
			UpdateDirtyFlag(true);
			UpdatePreview();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void revertButton_Click(object sender, EventArgs e)
		{
			if (_currentProject == null)
			{
				configEditor.Config = new KaraokeConfig();
			}
			else
			{
				configEditor.Config = _currentProject.Config.CopyTyped();
			}

			UpdatePreview();
			UpdateDirtyFlag(false);
		}

		private void StyleForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			Hide();
			e.Cancel = true;
		}

		private void applyButton_Click(object sender, EventArgs e)
		{
			if (_currentProject == null)
			{
				UpdateDirtyFlag(false);
				return;
			}

			OnProjectConfigApplied?.Invoke(_currentProject.Config);
			UpdateDirtyFlag(false);
		}

		private void videoPanel_Paint(object sender, PaintEventArgs e)
		{
			UpdatePreview();
		}

		private void previewSkiaControl_PaintSurface(object sender, SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs e)
		{
			_previewHandler.Render(e.Surface);
		}

		private void importButton_Click(object sender, EventArgs e)
		{
			var dialog = new VistaOpenFileDialog();
			dialog.Title = "Import style config";
			dialog.Filter = "Style config|*.json|All files|*.*";
			dialog.Multiselect = false;
			dialog.CheckFileExists = true;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				configEditor.Config = new KaraokeConfig(File.ReadAllText(dialog.FileName));
				UpdateDirtyFlag(true);
				UpdatePreview();
			}
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			var config = configEditor.GetConfig<KaraokeConfig>();

			var dialog = new VistaSaveFileDialog();
			dialog.Title = "Export style config";
			dialog.Filter = "Style config|*.json|All files|*.*";
			dialog.DefaultExt = ".json";
			dialog.AddExtension = true;
			if (dialog.ShowDialog() == DialogResult.OK)
			{
				config.Save(dialog.FileName);
			}
		}
	}
}
