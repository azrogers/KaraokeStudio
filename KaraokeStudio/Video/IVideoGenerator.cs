using KaraokeLib.Video;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.Video
{
	internal interface IVideoGenerator
	{
		void UpdateContext(KaraokeProject project, Size videoSize);

		void Render(KaraokeProject project, VideoTimecode timecode, SKSurface surface);

		void Invalidate();
	}
}
