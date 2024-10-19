using Ookii.Dialogs.WinForms;

namespace KaraokeStudio.Config.Controls
{
	public partial class FileConfigControl : BaseConfigControl
	{
		private string _path = string.Empty;

		public FileConfigControl()
		{
			InitializeComponent();
		}

		private void fileButton_Click(object sender, EventArgs e)
		{
			var dialog = new VistaOpenFileDialog();
			var extensions = Field?.ConfigFile?.AllowedExtensions ?? [];
			dialog.Multiselect = false;
			if (!extensions.Any())
			{
				dialog.Filter = "All files (*.*)|*.*";
			}
			else
			{
				dialog.Filter = $"Supported files ({string.Join(", ", extensions.Select(e => "*." + e))})|{string.Join(";", extensions.Select(e => "*." + e))}|All files (*.*)|*.*";
			}

			if (dialog.ShowDialog() == DialogResult.OK)
			{
				_path = dialog.FileName;
				SendValueChanged();
			}
		}

		internal override void UpdateValue(object config)
		{
			var val = Field?.GetValue<string>(config);
			if (val != null)
			{
				_path = val;
				fileBox.Text = val;
			}
		}

		internal override void SetValue(object config)
		{
			Field?.SetValue(config, _path);
		}

		private void fileBox_TextChanged(object sender, EventArgs e)
		{
			_path = fileBox.Text;
			SendValueChanged();
		}
	}
}
