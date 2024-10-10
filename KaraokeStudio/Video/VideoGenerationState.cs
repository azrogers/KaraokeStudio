using KaraokeLib.Config;
using KaraokeLib.Tracks;
using KaraokeLib.Video;
using SkiaSharp;

namespace KaraokeStudio.Video
{
	/// <summary>
	/// Contains the contexts required to generate video frames from a given set of events.
	/// Handles regenerating video data when lyrics change.
	/// </summary>
	internal class VideoGenerationState
	{
		private VideoContext? _context;
		private VideoStyle? _style;
		private VideoRenderer? _renderer;

		private bool _isRendererStale = false;

		/// <summary>
		/// Renders a frame of video.
		/// </summary>
		/// <param name="tracks">The tracks to build the video from.</param>
		/// <param name="position">The position in the video of the frame to render.</param>
		/// <param name="surface">The Skia surface to render to.</param>
		public void Render(IEnumerable<KaraokeTrack> tracks, VideoTimecode position, SKSurface surface)
		{
			if (_context == null)
			{
				return;
			}

			if (_isRendererStale || _renderer == null)
			{
				_renderer = new VideoRenderer(_context, tracks);
				_isRendererStale = false;
			}

			_renderer.RenderFrame(position, surface.Canvas);
		}

		/// <summary>
		/// Invalidates the cached layout information, forcing it to be recalculated next <see cref="Render"/>.
		/// </summary>
		public void InvalidatePlan()
		{
			_isRendererStale = true;
		}

		/// <summary>
		/// Updates the internal contexts based on the given information.
		/// </summary>
		/// <param name="duration">The total length in seconds of the video to generate.</param>
		/// <param name="pConfig">The project config to use to generate the video.</param>
		/// <param name="size">The size in (Width, Height) of the output video.</param>
		public void UpdateVideoContext(double duration, KaraokeConfig pConfig, (int Width, int Height) size)
		{
			var config = FromProjectConfig(pConfig, size);
			_style = new VideoStyle(config);

			_context = new VideoContext(_style, config, new VideoTimecode(duration, pConfig.FrameRate));
			_isRendererStale = true;
		}

		private KaraokeConfig FromProjectConfig(KaraokeConfig config, (int Width, int Height) outputSize)
		{
			var scaleFactor = outputSize.Width / (double)config.VideoSize.Width;

			var kConfig = config.CopyTyped();
			kConfig.VideoSize.Width = outputSize.Width;
			kConfig.VideoSize.Height = outputSize.Height;
			kConfig.Font.Size = (float)(config.Font.Size * scaleFactor);
			kConfig.StrokeWidth = (float)(config.StrokeWidth * scaleFactor);

			return kConfig;
		}
	}
}
