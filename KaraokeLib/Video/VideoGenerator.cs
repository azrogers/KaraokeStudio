using FFMediaToolkit;
using FFMediaToolkit.Audio;
using FFMediaToolkit.Encoding;
using FFMediaToolkit.Graphics;
using KaraokeLib.Audio;
using KaraokeLib.Lyrics;
using KaraokeLib.Video.Plan;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	public class VideoGenerator
	{
		private LyricsTrack[] _tracks;
		private VideoSection[] _sections;
		private VideoContext _context;
		private VideoTimecode _endTimecode;
		private AudioFile _audioFile;

		public VideoGenerator(ILyricsFile lyricsFile, string audioFilePath)
		{
			FFmpegLoader.FFmpegPath = Path.Join(Environment.CurrentDirectory, "ffmpeg", "x86_64");
			var audioFile = new AudioFile(audioFilePath);

			_tracks = lyricsFile.GetTracks().ToArray();
			_endTimecode = new VideoTimecode(audioFile.GetLengthSeconds(), KaraokeConfig.Default.FrameRate);
			_context = new VideoContext(new VideoStyle(KaraokeConfig.Default), KaraokeConfig.Default);
			_sections = _tracks.Any() ? VideoSection.FromTrack(_context, _tracks[0]) : new VideoSection[0];
			_audioFile = audioFile;
		}

		public void RenderVideo(string outputFile)
		{
			RenderVideo(new VideoTimecode(0, _context.Config.FrameRate), _endTimecode, outputFile);
		}

		public void RenderVideo(double startPosition, double videoLength, string outputFile)
		{
			RenderVideo(
				new VideoTimecode(startPosition, _context.Config.FrameRate), 
				new VideoTimecode(startPosition + videoLength, _context.Config.FrameRate), 
				outputFile);
		}

		public void RenderVideo(VideoTimecode startTimecode, VideoTimecode endTimecode, string outputFile) =>
			RenderVideo(startTimecode, endTimecode, outputFile, VideoPlanGenerator.CreateVideoPlan(_context, _sections, _endTimecode));

		public void RenderVideo(VideoTimecode startTimecode, VideoTimecode endTimecode, string outputFile, VideoPlan plan)
		{
			var outputPath = Path.GetFullPath(outputFile);
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

			var renderer = new VideoRenderer(_context, _sections);

			using (var bitmap = new SKBitmap(_context.Config.VideoSize.Width, _context.Config.VideoSize.Height, SKColorType.Rgba8888, SKAlphaType.Unpremul))
			using (var canvas = new SKCanvas(bitmap))
			using (var file = builder.Create())
			{
				var position = startTimecode;
				while (position <= endTimecode)
				{
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

			FFMpegUtil.MuxVideoAudio(
				tempOutputPath,
				_audioFile.FileName,
				startTimecode.ToSeconds(),
				endTimecode.ToSeconds() - startTimecode.ToSeconds(),
				outputPath);

			File.Delete(tempOutputPath);
		}

		public void RenderFrameToFile(VideoTimecode videoPosition, string outputFile) =>
			RenderFrameToFile(videoPosition, outputFile, VideoPlanGenerator.CreateVideoPlan(_context, _sections, _endTimecode));

		public void RenderFrameToFile(VideoTimecode videoPosition, string outputFile, VideoPlan plan)
		{
			if(videoPosition < 0 || videoPosition > _endTimecode)
			{
				throw new ArgumentOutOfRangeException("Video position out of range");
			}

			var renderer = new VideoRenderer(_context, _sections);

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
