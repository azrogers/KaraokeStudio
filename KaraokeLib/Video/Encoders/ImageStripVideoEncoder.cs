using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using NAudio.Wave;
using SkiaSharp;

namespace KaraokeLib.Video.Encoders
{
	public class ImageStripVideoEncoder : IVideoEncoder
	{
		public VideoEncoderType EncoderType => VideoEncoderType.ImageStrip;

		public bool DoesEncodeMultipleFiles => true;

		private ImageStripVideoEncoderSettings _settings = new ImageStripVideoEncoderSettings();
		private bool _hasStartedRender = false;
		private string? _currentOutDir = null;
		private VideoExporter? _currentExporter = null;

		public IEditableConfig GetConfigObject() => _settings;

		public void SetConfigObject(IEditableConfig configObject) => _settings = (ImageStripVideoEncoderSettings)configObject;

		public string[] ValidateConfigObject(IEditableConfig config) => new string[0];

		public void StartRender(string outFile, WaveStream audio, int width, int height, int frameRate, double length)
		{
			if (_hasStartedRender)
			{
				throw new InvalidOperationException("Can't start a render before the previous one has finished!");
			}

			_hasStartedRender = true;
			_currentOutDir = outFile;

			WaveFileWriter.CreateWaveFile(Path.Combine(_currentOutDir, "output.wav"), audio);
		}

		public void RenderFrame(VideoTimecode timecode, SKBitmap frameBitmap)
		{
			if (!_hasStartedRender || _currentOutDir == null)
			{
				throw new InvalidOperationException("Can't render a frame if a render hasn't been started!");
			}

			var ext = _settings.ImageFormat.ToString().ToLower();

			var path = Path.Combine(_currentOutDir, $"{_settings.FramePrefix}{timecode.FrameNumber:D6}.{ext}");

			using (var data = frameBitmap.Encode(_settings.GetSkiaFormat(), _settings.ImageQuality))
			using (var output = File.OpenWrite(path))
			{
				data.SaveTo(output);
			}
		}

		public void EndRender(bool success)
		{
			if (!_hasStartedRender)
			{
				throw new InvalidOperationException("Can't end a render when one hasn't been started!");
			}

			_hasStartedRender = false;
			_currentOutDir = null;
		}

		public (string Extension, string Title)[] GetOutputExtensions()
		{
			return new (string, string)[] { ("." + _settings.ImageFormat.ToString().ToLower(), _settings.ImageFormat.ToString() + " file") };
		}

		public void SetExporterObject(VideoExporter exporter)
		{
			_currentExporter = exporter;
		}
	}

	public class ImageStripVideoEncoderSettings : EditableConfig<ImageStripVideoEncoderSettings>
	{

		public ImageType ImageFormat;

		[ConfigRange(0, 100)]
		public int ImageQuality = 95;

		public string FramePrefix = "frame";

		public enum ImageType
		{
			Png,
			Jpg,
			Gif,
			WebP,
			Bmp
		}

		public SKEncodedImageFormat GetSkiaFormat()
		{
			switch (ImageFormat)
			{
				case ImageType.Gif:
					return SKEncodedImageFormat.Gif;
				case ImageType.Png:
					return SKEncodedImageFormat.Png;
				case ImageType.Jpg:
					return SKEncodedImageFormat.Jpeg;
				case ImageType.WebP:
					return SKEncodedImageFormat.Webp;
				case ImageType.Bmp:
					return SKEncodedImageFormat.Bmp;
				default:
					throw new NotImplementedException($"Unknown image format {ImageFormat}");
			}
		}
	}
}
