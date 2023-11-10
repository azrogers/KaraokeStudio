using KaraokeLib.Video;
using KaraokeLib.Video.Encoders;
using KaraokeStudio.Project;
using Ookii.Dialogs.WinForms;
using System.Data;

namespace KaraokeStudio
{
	public partial class ExportVideoForm : Form
	{
		private readonly Dictionary<VideoEncoderType, IVideoEncoder> _encoders = new Dictionary<VideoEncoderType, IVideoEncoder>()
		{
			{
				VideoEncoderType.Ffmpeg,
				new FfmpegVideoEncoder()
			},
			{
				VideoEncoderType.ImageStrip,
				new ImageStripVideoEncoder()
			}
		};

		private VideoEncoderType EncoderType => Enum.Parse<VideoEncoderType>(typeComboBox.SelectedItem?.ToString() ?? nameof(VideoEncoderType.Ffmpeg));

		private VideoExporter _exporter;
		private KaraokeProject? _project;

		public ExportVideoForm()
		{
			InitializeComponent();

			_exporter = new VideoExporter();
			_exporter.OnExportMessage += _exporter_OnExportMessage;
			_exporter.OnExportProgress += _exporter_OnExportProgress;
			_exporter.OnExportStateChanged += _exporter_OnExportStateChanged;

			typeComboBox.Items.Clear();
			typeComboBox.Items.AddRange(_encoders.Keys.Select(k => k.ToString()).ToArray());
			typeComboBox.SelectedIndex = 0;
			UpdateConfig();
			UpdateButtons();
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			if (project == null)
			{
				Close();
				return;
			}

			_project = project;
			exportProgress.Minimum = 0;
			exportProgress.Maximum = (int)(project.Length.TotalSeconds * project.Config.FrameRate);
		}

		private string? GetOutputFolder()
		{
			var dialog = new VistaFolderBrowserDialog();
			dialog.Description = "Output folder";
			dialog.UseDescriptionForTitle = true;
			return dialog.ShowDialog() == DialogResult.OK ? dialog.SelectedPath : null;
		}

		private string? GetOutputFile(IVideoEncoder encoder)
		{
			var dialog = new VistaSaveFileDialog();
			dialog.Filter = string.Join("|", encoder.GetOutputExtensions().Select(info =>
			{
				var (ext, title) = info;
				return $"{title} (*{ext})|*{ext}";
			}).Concat(new string[] { "All Files (*.*)|*.*" }));
			return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
		}

		private void DoEncode()
		{
			if (_project == null || _exporter.State == VideoExporter.ExportState.Exporting)
			{
				return;
			}

			var encoder = _encoders[EncoderType];
			var outputFile = encoder.DoesEncodeMultipleFiles ? GetOutputFolder() : GetOutputFile(encoder);
			if (outputFile == null)
			{
				return;
			}

			_exporter.Export(_project.File, _project.Config, _encoders[EncoderType], outputFile, 0.0, _project.Length.TotalSeconds);
		}

		private void UpdateConfig()
		{
			encoderConfigEditor.Config = _encoders[EncoderType].GetConfigObject();
		}

		private void UpdateButtons()
		{
			exportButton.Enabled = _exporter.State != VideoExporter.ExportState.Exporting;
			cancelButton.Enabled = _exporter.State == VideoExporter.ExportState.Exporting;
		}

		private void _exporter_OnExportStateChanged(VideoExporter.ExportState obj)
		{
			_exporter_OnExportMessage($"Status changed: {obj}");
			exportButton.Invoke(() =>
			{
				UpdateButtons();
			});
		}

		private void _exporter_OnExportProgress(float obj)
		{
			messageBox.Invoke(() =>
			{
				exportProgress.Value = (int)(obj * exportProgress.Maximum);
			});
		}

		private void _exporter_OnExportMessage(string obj)
		{
			messageBox.Invoke(() =>
			{
				messageBox.AppendText(obj + Environment.NewLine);
			});
		}

		private void exportButton_Click(object sender, EventArgs e)
		{
			DoEncode();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			_exporter.Cancel();
		}

		private void typeComboBox_SelectedIndexChanged(object sender, EventArgs e)
		{
			UpdateConfig();
		}
	}
}
