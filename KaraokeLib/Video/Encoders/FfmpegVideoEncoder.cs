using CSCore;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using SkiaSharp;

namespace KaraokeLib.Video.Encoders
{
	public class FfmpegVideoEncoder : IVideoEncoder
	{
		private const int H264_CRF_MIN = 18;
		private const int H264_CRF_MAX = 28;
		private const int VP9_CRF_MIN = 15;
		private const int VP9_CRF_MAX = 35;

		public VideoEncoderType EncoderType => VideoEncoderType.Ffmpeg;

		public bool DoesEncodeMultipleFiles => false;

		private FfmpegVideoEncoderSettings _settings = new FfmpegVideoEncoderSettings();
		private bool _hasStartedRender = false;
		private string? _videoTempFile;
		private string? _audioTempFile;
		private MediaOutput? _output;
		private string _outputFile = "";
		private double _length = 0;

		private byte[] _buffer = new byte[0];

		private VideoExporter? _currentExporter;

		public IEditableConfig GetConfigObject()
		{
			return _settings;
		}

		public void SetConfigObject(IEditableConfig configObject)
		{
			_settings = (FfmpegVideoEncoderSettings)configObject;
		}

		public string[] ValidateConfigObject(IEditableConfig config)
		{
			return new string[0];
		}

		public void StartRender(string outFile, IWaveSource audio, int width, int height, int frameRate, double length)
		{
			if (_hasStartedRender)
			{
				throw new InvalidOperationException("Can't start a render before the previous one has finished!");
			}

			_hasStartedRender = true;
			_length = length;
			_outputFile = outFile;

			var ext = Path.GetExtension(outFile);
			if (string.IsNullOrWhiteSpace(ext))
			{
				ext = _settings.GetExtensionFromOutputType();
				_outputFile += ext;
			}

			var outputFilename = Path.GetFileNameWithoutExtension(outFile);
			var outputDir = Path.GetDirectoryName(outFile) ?? "";

			_videoTempFile = Path.Combine(outputDir, outputFilename + "-temp" + ext);
			_audioTempFile = Path.Combine(outputDir, outputFilename + "-temp.wav");

			_currentExporter?.LogMessage($"Writing temp audio file to {_audioTempFile}");
			audio.WriteToFile(_audioTempFile);

			var codec = _settings.GetCodec();

			var videoSettings = new VideoEncoderSettings(
				width: width,
				height: height,
				framerate: frameRate,
				codec: codec);

			if (codec == VideoCodec.H264)
			{
				// higher quality value = lower CRF
				var crf = (int)((1.0f - _settings.Quality) * (H264_CRF_MAX - H264_CRF_MIN) + H264_CRF_MIN);
				videoSettings.CRF = crf;
				videoSettings.EncoderPreset = EncoderPreset.Fast;
			}
			else if (codec == VideoCodec.VP9)
			{
				var crf = (int)((1.0f - _settings.Quality) * (VP9_CRF_MAX - VP9_CRF_MIN) + VP9_CRF_MIN);
				videoSettings.CRF = crf;
				videoSettings.Bitrate = 0;
			}

			_currentExporter?.LogMessage($"Writing temp video file to {_videoTempFile}");
			var builder = MediaBuilder
				.CreateContainer(_videoTempFile)
				.WithVideo(videoSettings);

			_output = builder.Create();
		}

		public void RenderFrame(VideoTimecode timecode, SKBitmap frameBitmap)
		{
			if (!_hasStartedRender || _output == null)
			{
				throw new InvalidOperationException("Can't render a frame if a render hasn't been started!");
			}

			var pixels = frameBitmap.GetPixelSpan();
			if (pixels.Length > _buffer.Length)
			{
				_buffer = new byte[pixels.Length];
			}

			pixels.CopyTo(_buffer);

			var imageData = new ImageData(
				_buffer,
				ImagePixelFormat.Rgba32,
				new System.Drawing.Size(frameBitmap.Width, frameBitmap.Height));
			_output.Video.AddFrame(imageData);
		}

		public void EndRender(bool success)
		{
			if (!_hasStartedRender || _output == null || _videoTempFile == null || _audioTempFile == null)
			{
				throw new InvalidOperationException("Can't end a render when one hasn't been started!");
			}

			_output.Dispose();

			if (success)
			{
				_currentExporter?.LogMessage($"Combining temp video and audio files into {_outputFile}");
				FFMpegUtil.MuxVideoAudio(_videoTempFile, _audioTempFile, 0, _length, _outputFile);
			}

			_currentExporter?.LogMessage($"Deleting temp files");
			File.Delete(_videoTempFile);
			File.Delete(_audioTempFile);

			_hasStartedRender = false;
		}

		public (string Extension, string Title)[] GetOutputExtensions()
		{
			return new (string, string)[] { (_settings.GetExtensionFromOutputType(), _settings.OutputType.ToString() + " file") };
		}

		public void SetExporterObject(VideoExporter exporter)
		{
			_currentExporter = exporter;
		}
	}

	public class FfmpegVideoEncoderSettings : EditableConfig<FfmpegVideoEncoderSettings>
	{
		public FfmpegOutputType OutputType;
		[ConfigRange(0.0f, 1.0f)]
		public float Quality = 0.5f;

		public string GetExtensionFromOutputType()
		{
			switch (OutputType)
			{
				case FfmpegOutputType.Mp4:
					return ".mp4";
				case FfmpegOutputType.WebM:
					return ".webm";
				case FfmpegOutputType.Mov:
					return ".mov";
				default:
					throw new NotImplementedException($"Unknown FfmpegOutputType {OutputType}");
			}
		}

		public VideoCodec GetCodec()
		{
			switch (OutputType)
			{
				case FfmpegOutputType.Mp4:
				case FfmpegOutputType.Mov:
					return VideoCodec.H264;
				case FfmpegOutputType.WebM:
					return VideoCodec.VP9;
				default:
					throw new NotImplementedException($"Unknown FfmpegOutputType {OutputType}");
			}
		}
	}

	public enum FfmpegOutputType
	{
		Mp4,
		WebM,
		Mov
	}
}
