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
			_context = new VideoContext(new VideoStyle(), KaraokeConfig.Default);
			_sections = new VideoSection[0];
			_audioFile = audioFile;

			CreateSections();
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

		public void RenderVideo(VideoTimecode startTimecode, VideoTimecode endTimecode, string outputFile)
		{
			var outputPath = Path.GetFullPath(outputFile);
			var tempFileName = 
				Path.GetFileNameWithoutExtension(outputFile) +
				"-temp" +
				Path.GetExtension(outputFile);
			var tempOutputPath = Path.Combine(Path.GetDirectoryName(outputPath) ?? "", tempFileName);

			var videoSettings = new VideoEncoderSettings(
				width: _context.Config.VideoWidth, 
				height: _context.Config.VideoHeight, 
				framerate: _context.Config.FrameRate, 
				codec: VideoCodec.H264);
			videoSettings.EncoderPreset = EncoderPreset.Fast;
			videoSettings.CRF = 17;
			var builder = MediaBuilder
				.CreateContainer(tempOutputPath)
				.WithVideo(videoSettings);

			var plan = VideoPlanGenerator.CreateVideoPlan(_context, _sections, _endTimecode);
			var renderer = new VideoRenderer(_context, _sections);

			using (var bitmap = new SKBitmap(_context.Config.VideoWidth, _context.Config.VideoHeight, SKColorType.Rgba8888, SKAlphaType.Unpremul))
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
						new System.Drawing.Size(_context.Config.VideoWidth, _context.Config.VideoHeight));
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

		public void RenderFrameToFile(VideoTimecode videoPosition, string outputFile)
		{
			if(videoPosition < 0 || videoPosition > _endTimecode)
			{
				throw new ArgumentOutOfRangeException("Video position out of range");
			}

			var plan = VideoPlanGenerator.CreateVideoPlan(_context, _sections, _endTimecode);
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

		private void CreateSections()
		{
			var sections = new List<VideoSection>();
			
			if(_tracks.Length == 0)
			{
				_sections = new VideoSection[0];
				return;
			}

			// TODO: support multiple tracks
			var track = _tracks[0];
			var lastEventEndTime = 0.0;
			var firstEventStartTime = track.Events.FirstOrDefault()?.StartTimeSeconds ?? 0.0;
			var eventsAccumulated = new List<LyricsEvent>();

			foreach(var ev in track.Events)
			{
				var timeBetweenEvents = ev.StartTimeSeconds - lastEventEndTime;
				if(eventsAccumulated.Any() && timeBetweenEvents >= _context.Config.MinTimeBetweenSections)
				{
					// break in the video, add a new section for the break and add all accumulated events
					var newSection = new VideoSection(
						_context,
						VideoSectionType.Lyrics,
						firstEventStartTime,
						lastEventEndTime - firstEventStartTime);
					newSection.SetEvents(eventsAccumulated);
					sections.Add(newSection);
					sections.Add(new VideoSection(
						_context,
						VideoSectionType.Break, 
						lastEventEndTime,
						timeBetweenEvents));

					eventsAccumulated.Clear();
					firstEventStartTime = ev.StartTimeSeconds;
				}

				lastEventEndTime = ev.EndTimeSeconds;
				eventsAccumulated.Add(ev);
			}

			// add last section
			if(eventsAccumulated.Any())
			{
				var newSection = new VideoSection(
					_context, 
					VideoSectionType.Lyrics, 
					firstEventStartTime,
					lastEventEndTime - firstEventStartTime);
				newSection.SetEvents(eventsAccumulated);
				sections.Add(newSection);
			}

			_sections = sections.ToArray();
		}
	}
}
