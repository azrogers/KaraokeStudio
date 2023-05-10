using KaraokeLib;
using KaraokeLib.Lyrics;
using KaraokeLib.Video;
using KaraokeLib.Video.Plan;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal class VideoGenerationState
	{
		private VideoContext? _context;
		private VideoStyle? _style;
		private VideoRenderer? _renderer;
		private VideoPlan? _plan;
		private int _frameRate;

		private bool _isPlanStale = false;

		public void Render(IEnumerable<LyricsTrack> tracks, VideoTimecode position, SKSurface surface)
		{
			if(_context == null)
			{
				return;
			}

			if(_isPlanStale || _renderer == null || _plan == null)
			{
				VideoSection[] sections;
				if (tracks.Any())
				{
					// TODO: support multiple tracks?
					sections = VideoSection.FromTrack(_context, tracks.First());
				}
				else
				{
					sections = new VideoSection[0];
				}

				_plan = VideoPlanGenerator.CreateVideoPlan(_context, sections);
				_renderer = new VideoRenderer(_context, sections);
				_isPlanStale = false;
			}

			_renderer.RenderFrame(_plan, position, surface.Canvas);
		}

		public void UpdateVideoContext(double duration, ProjectConfig pConfig, (int Width, int Height) size)
		{
			var config = FromProjectConfig(pConfig, size);
			_style = new VideoStyle(config);

			_context = new VideoContext(_style, config, new VideoTimecode(duration, pConfig.FrameRate));
			_isPlanStale = true;
			_frameRate = pConfig.FrameRate;
		}

		private KaraokeConfig FromProjectConfig(ProjectConfig config, (int Width, int Height) outputSize)
		{
			var scaleFactor = (double)outputSize.Width / (double)config.VideoSize.Width;

			var kConfig = ProjectConfig.Copy(config);
			kConfig.VideoSize.Width = outputSize.Width;
			kConfig.VideoSize.Height = outputSize.Height;
			kConfig.Font.Size = (float)(config.Font.Size * scaleFactor);
			kConfig.StrokeWidth = (float)(config.StrokeWidth * scaleFactor);

			return kConfig;
		}
	}
}
