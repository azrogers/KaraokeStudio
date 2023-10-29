using FFMediaToolkit;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Files;
using KaraokeLib.Video.Plan;
using SkiaSharp;

namespace KaraokeLib.Video
{
    /// <summary>
    /// Responsible for rendering a lyrics file into a video.
    /// This is the entry-level class for using KaraokeLib.
    /// </summary>
    public class VideoGenerator
	{
		private KaraokeTrack[] _tracks;
		private VideoSection[] _sections;
		private VideoContext _context;
		private VideoLayoutState _layoutState;
		private VideoTimecode _endTimecode;
		private AudioFile _audioFile;

		/// <summary>
		/// Creates a new VideoGenerator.
		/// </summary>
		/// <param name="lyricsFile">The lyrics file that the video will be generated from.</param>
		/// <param name="audioFilePath">The path of the audio file that will be paired with the generated video.</param>
		/// <param name="videoLength">The length of the generated video in seconds.</param>
		public VideoGenerator(IKaraokeFile lyricsFile, string audioFilePath, double videoLength = -1)
		{
			FFMpegUtil.SetupFfmpegPath();
			var audioFile = new AudioFile(audioFilePath);

			_tracks = lyricsFile.GetTracks().ToArray();
			_endTimecode = new VideoTimecode(videoLength < 0 ? audioFile.GetLengthSeconds() : videoLength, KaraokeConfig.Default.FrameRate);
			_context = new VideoContext(new VideoStyle(KaraokeConfig.Default), KaraokeConfig.Default, _endTimecode);
			_layoutState = new VideoLayoutState();
			_sections = _tracks.Any() ? VideoSection.SectionsFromTrack(_context, _tracks[0], _layoutState) : new VideoSection[0];
			_audioFile = audioFile;
		}

		/// <summary>
		/// Renders the full video from start to finish to the given output file.
		/// </summary>
		public void RenderVideo(string outputFile)
		{
			RenderVideo(new VideoTimecode(0, _context.Config.FrameRate), _endTimecode, outputFile);
		}

		/// <summary>
		/// Renders a video of a given length at the given start position to the given output file.
		/// </summary>
		public void RenderVideo(double startPosition, double videoLength, string outputFile)
		{
			RenderVideo(
				new VideoTimecode(startPosition, _context.Config.FrameRate),
				new VideoTimecode(startPosition + videoLength, _context.Config.FrameRate),
				outputFile);
		}

		/// <summary>
		/// Renders a video of the given length at the given start position to the given output file.
		/// </summary>
		public void RenderVideo(VideoTimecode startTimecode, VideoTimecode endTimecode, string outputFile) =>
			RenderVideo(startTimecode, endTimecode, outputFile, VideoPlanGenerator.CreateVideoPlan(_context, _layoutState, _sections));

		public void RenderVideo(VideoTimecode startTimecode, VideoTimecode endTimecode, string outputFile, VideoPlan plan)
		{
			var outputPath = Path.GetFullPath(outputFile);
			// originally render to a temp file before muxing
			var tempFileName =
				Path.GetFileNameWithoutExtension(outputFile) +
				"-temp" +
				Path.GetExtension(outputFile);
			var tempOutputPath = Path.Combine(Path.GetDirectoryName(outputPath) ?? "", tempFileName);

			var videoSettings = new VideoEncoderSettings(
				width: _context.Config.VideoSize.Width,
				height: _context.Config.VideoSize.Height,
				framerate: _context.Config.FrameRate,
				codec: VideoCodec.H264);
			videoSettings.EncoderPreset = EncoderPreset.Fast;
			videoSettings.CRF = 17;
			var builder = MediaBuilder
				.CreateContainer(tempOutputPath)
				.WithVideo(videoSettings);

			var renderer = new VideoRenderer(_context, _layoutState, _sections);

			using (var bitmap = new SKBitmap(_context.Config.VideoSize.Width, _context.Config.VideoSize.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul))
			using (var canvas = new SKCanvas(bitmap))
			using (var file = builder.Create())
			{
				var position = startTimecode;
				while (position <= endTimecode)
				{
					// render the current frame
					renderer.RenderFrame(plan, position, canvas);

					var imageData = new ImageData(
						bitmap.Bytes,
						ImagePixelFormat.Rgba32,
						new System.Drawing.Size(_context.Config.VideoSize.Width, _context.Config.VideoSize.Height));
					file.Video.AddFrame(imageData);

					canvas.Clear();
					position++;
				}
			}

			// FFMediaToolkit's muxing sucks - just call FFMpeg directly to combine audio and video
			FFMpegUtil.MuxVideoAudio(
				tempOutputPath,
				_audioFile.FileName,
				startTimecode.ToSeconds(),
				endTimecode.ToSeconds() - startTimecode.ToSeconds(),
				outputPath);

			File.Delete(tempOutputPath);
		}

		/// <summary>
		/// Renders the frame at the given position to the output file.
		/// </summary>
		public void RenderFrameToFile(VideoTimecode videoPosition, string outputFile) =>
			RenderFrameToFile(videoPosition, outputFile, VideoPlanGenerator.CreateVideoPlan(_context, _layoutState, _sections));

		public void RenderFrameToFile(VideoTimecode videoPosition, string outputFile, VideoPlan plan)
		{
			if (videoPosition < 0 || videoPosition > _endTimecode)
			{
				throw new ArgumentOutOfRangeException("Video position out of range");
			}

			var renderer = new VideoRenderer(_context, _layoutState, _sections);

			using (var bitmap = new SKBitmap(1920, 1080))
			{
				using (var canvas = new SKCanvas(bitmap))
				{
					renderer.RenderFrame(plan, videoPosition, canvas);
				}

				var data = bitmap.Encode(SKEncodedImageFormat.Png, 100);
				using (var outputStream = File.OpenWrite(outputFile))
				{
					data.SaveTo(outputStream);
				}
			}
		}
	}
}
