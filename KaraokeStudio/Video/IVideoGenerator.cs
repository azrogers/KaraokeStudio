using KaraokeLib.Video;
using KaraokeStudio.Project;
using SkiaSharp;

namespace KaraokeStudio.Video
{
	internal interface IVideoGenerator: IDisposable
	{
		void UpdateContext(KaraokeProject project, Size videoSize);

		void Render(KaraokeProject project, VideoTimecode timecode, SKSurface surface);

		void Invalidate();
	}
}
