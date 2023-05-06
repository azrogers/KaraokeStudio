using KaraokeStudio.Config;
using Ookii.Dialogs.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaraokeStudio
{
	public partial class StyleForm : Form
	{
		private ProjectConfig _temporaryConfig = new ProjectConfig();
		private KaraokeProject? _currentProject = null;
		private ConfigFields _fields;
		private List<BaseConfigControl> _controls = new List<BaseConfigControl>();
		private ConfigPreviewHandler _previewHandler = new ConfigPreviewHandler();

		private bool _isDirty = false;

		internal event Action<ProjectConfig>? OnProjectConfigApplied;

		public StyleForm()
		{
			InitializeComponent();
			_fields = new ConfigFields(typeof(ProjectConfig));
			CreateFields(_fields);

			// disable horizontal scroll on configContainer
			configContainer.HorizontalScroll.Maximum = 0;
			configContainer.AutoScroll = false;
			configContainer.VerticalScroll.Visible = false;
			configContainer.AutoScroll = true;

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

			if(project == null)
			{
				_temporaryConfig = new ProjectConfig();
			}
			else
			{
				_temporaryConfig = ProjectConfig.Copy(project.Config);
			}

			UpdateControls();
			UpdateDirtyFlag(false);
			UpdatePreview();
		}

		private void CreateFields(ConfigFields fields)
		{
			_controls.Clear();

			configContainer.ColumnCount = 2;

			foreach(var field in fields.Fields)
			{
				switch(field.ControlType)
				{
					case ConfigFields.ControlType.Size:
						AddField(field.Name, new SizeConfigControl() { Field = field });
						break;
					case ConfigFields.ControlType.Numeric:
						AddField(field.Name, new NumericConfigControl() { Field = field });
						break;
					case ConfigFields.ControlType.Range:
						AddField(field.Name, new RangeConfigControl() { Field = field });
						break;
					case ConfigFields.ControlType.Color:
						AddField(field.Name, new ColorConfigControl() { Field = field });
						break;
					case ConfigFields.ControlType.Font:
						AddField(field.Name, new FontConfigControl() { Field = field });
						break;
				}
			}

			configContainer.PerformLayout();
		}

		private void AddField(string name, BaseConfigControl control)
		{
			control.Dock = DockStyle.Fill;
			control.AutoSize = true;
			control.AutoSizeMode = AutoSizeMode.GrowAndShrink;

			control.OnValueChanged += (_) =>
			{
				UpdateDirtyFlag(true);
				control.SetValue(_temporaryConfig);
				UpdatePreview();
			};

			var label = new Label();
			label.Text = Util.HumanizeCamelCase(name);
			label.Size = new Size(100, 15);
			label.AutoSize = true;

			configContainer.RowCount = _controls.Count + 1;
			configContainer.RowStyles.Add(new RowStyle(SizeType.AutoSize, 15.0f));
			configContainer.Controls.Add(label, 0, _controls.Count);
			configContainer.Controls.Add(control, 1, _controls.Count);

			control.UpdateValue(_temporaryConfig);
			_controls.Add(control);
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

		private void UpdateControls()
		{
			foreach (var c in _controls)
			{
				c.UpdateValue(_temporaryConfig);
			}
		}

		private void UpdatePanelSize()
		{
			var size = (_temporaryConfig.VideoSize.Width, _temporaryConfig.VideoSize.Height);
			Util.ResizeContainerAspectRatio(videoPanel, previewSkiaControl, size, false);
		}

		private void UpdatePreview()
		{
			UpdatePanelSize();
			_previewHandler.UpdatePreview(_temporaryConfig, (previewSkiaControl.Width, previewSkiaControl.Height));
			previewSkiaControl.Invalidate();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Hide();
		}

		private void revertButton_Click(object sender, EventArgs e)
		{
			if(_currentProject == null)
			{
				_temporaryConfig = new ProjectConfig();
			}
			else
			{
				_temporaryConfig = ProjectConfig.Copy(_currentProject.Config);
			}

			UpdateControls();
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
			if(_currentProject == null)
			{
				UpdateDirtyFlag(false);
				return;
			}

			OnProjectConfigApplied?.Invoke(_temporaryConfig);
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
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				_temporaryConfig = ProjectConfig.Load(dialog.FileName);
				UpdateDirtyFlag(true);
				UpdateControls();
				UpdatePreview();
			}
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			var dialog = new VistaSaveFileDialog();
			dialog.Title = "Export style config";
			dialog.Filter = "Style config|*.json|All files|*.*";
			dialog.DefaultExt = ".json";
			dialog.AddExtension = true;
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				_temporaryConfig.Save(dialog.FileName);
			}
		}
	}
}
