using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Transitions
{
	internal class NoneVideoTransition : IVideoTransition
	{
		public VideoTransitionType Type => VideoTransitionType.None;

		public void Blit(IVideoElement elem, SKSurface source, SKCanvas dest, float t, bool isStartTransition)
		{
			dest.DrawSurface(source, new SKPoint(0, 0));
		}
	}
}
