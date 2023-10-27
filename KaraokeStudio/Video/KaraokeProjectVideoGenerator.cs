using KaraokeLib.Video;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.Video
{
	internal class KaraokeProjectVideoGenerator : IVideoGenerator
	{
		private VideoGenerationState _generationState = new VideoGenerationState();

		public void Invalidate() => _generationState.InvalidatePlan();

		public void Render(KaraokeProject project, VideoTimecode timecode, SKSurface surface)
		{
			_generationState.Render(project.Tracks, timecode, surface);
		}

		public void UpdateContext(KaraokeProject project, Size videoSize)
		{
			_generationState.UpdateVideoContext(
				project.Length.TotalSeconds,
				project.Config,
				(videoSize.Width, videoSize.Height));
		}
	}
}
