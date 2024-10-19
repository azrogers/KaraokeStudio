using CSCore;
using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Files;
using KaraokeLib.Video.Encoders;
using KaraokeLib.Video.Plan;
using SkiaSharp;

namespace KaraokeLib.Video
{
	public class VideoExporter
	{
		public event Action<ExportState>? OnExportStateChanged;

		public event Action<float>? OnExportProgress;
		public event Action<string>? OnExportMessage;

		public ExportState State
		{
			get => _exportState;
			private set
			{
				_exportState = value;
				OnExportStateChanged?.Invoke(value);
			}
		}

		private ExportState _exportState = ExportState.Idle;

		public VideoExporter()
		{

		}

		public void Cancel() => _exportState = ExportState.Cancelled;

		public void Export(IKaraokeFile file, KaraokeConfig config, IVideoEncoder encoder, string outFile, double startSeconds, double lengthSeconds)
		{
			if (State == ExportState.Exporting)
			{
				throw new InvalidOperationException("Can't export again while an export is ongoing!");
			}

			State = ExportState.Exporting;
			Task.Run(() =>
			{
				try
				{
					DoExport(file, config, encoder, outFile, startSeconds, lengthSeconds);
				}
				catch (Exception ex)
				{
					State = ExportState.Failure;
					LogMessage($"Exception: {ex.Message}");
					throw;
				}
				finally
				{
					OnExportProgress?.Invoke(1.0f);
				}
			});
		}

		private void DoExport(IKaraokeFile file, KaraokeConfig config, IVideoEncoder encoder, string outFile, double startSeconds, double lengthSeconds)
		{
			encoder.SetExporterObject(this);
			using (var mixer = new AudioMixer(file.GetTracks().Where(t => t.Type == Tracks.KaraokeTrackType.Audio), 48000))
			{
				encoder.StartRender(outFile, mixer.ToWaveSource(), config.VideoSize.Width, config.VideoSize.Height, config.FrameRate, lengthSeconds);
			}

			var tracks = file.GetTracks().ToArray();
			var startTimecode = new VideoTimecode(startSeconds, config.FrameRate);
			var endTimecode = new VideoTimecode(startSeconds + lengthSeconds, config.FrameRate);
			var context = new VideoContext(new VideoStyle(config), config, endTimecode);

			using (var renderer = new VideoRenderer(context, tracks))
			using (var bitmap = new SKBitmap(config.VideoSize.Width, config.VideoSize.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul))
			using (var canvas = new SKCanvas(bitmap))
			{
				var position = startTimecode;
				while (position <= endTimecode)
				{
					if (State == ExportState.Cancelled)
					{
						encoder.EndRender(false);
						State = ExportState.Idle;
						return;
					}

					var progressNormalized = (position.ToSeconds() - startTimecode.ToSeconds()) / (endTimecode.ToSeconds() - startTimecode.ToSeconds());
					OnExportProgress?.Invoke((float)Math.Clamp(progressNormalized, 0, 1));

					// render the current frame
					renderer.RenderFrame(position, canvas);

					encoder.RenderFrame(position, bitmap);

					canvas.Clear();
					position++;
				}
			}

			encoder.EndRender(true);
			State = ExportState.Success;
		}

		internal void LogMessage(string msg)
		{
			OnExportMessage?.Invoke(msg);
		}

		public enum ExportState
		{
			Idle,
			Exporting,
			Cancelled,
			Success,
			Failure
		}
	}
}
