using KaraokeLib;
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

		private bool _isPlanStale = false;

		public void Render(KaraokeProject project, VideoTimecode position, SKSurface surface)
		{
			if(_context == null)
			{
				return;
			}

			if(_isPlanStale || _renderer == null || _plan == null)
			{
				var tracks = project.Tracks;
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

				_plan = VideoPlanGenerator.CreateVideoPlan(_context, sections, new VideoTimecode(project.Length.TotalSeconds, project.Config.FrameRate));
				_renderer = new VideoRenderer(_context, sections);
				_isPlanStale = false;
			}

			_renderer.RenderFrame(_plan, position, surface.Canvas);
		}

		public void UpdateVideoContext(KaraokeProject project, (int Width, int Height) size)
		{
			var config = FromProjectConfig(project.Config, size);
			_style = new VideoStyle(config);

			_context = new VideoContext(_style, config);
			_isPlanStale = true;
		}

		private KaraokeConfig FromProjectConfig(ProjectConfig config, (int Width, int Height) outputSize)
		{
			var scaleFactor = (double)outputSize.Width / (double)config.VideoWidth;

			var kConfig = ProjectConfig.Copy(config);
			kConfig.VideoWidth = outputSize.Width;
			kConfig.VideoHeight = outputSize.Height;
			kConfig.FontSize = (float)(config.FontSize * scaleFactor);
			kConfig.StrokeWidth = (float)(config.StrokeWidth * scaleFactor);

			return kConfig;
		}
	}
}
