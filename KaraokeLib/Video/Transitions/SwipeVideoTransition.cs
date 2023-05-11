using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Transitions
{
	internal class SwipeVideoTransition : IVideoTransition
	{
		public VideoTransitionType Type => VideoTransitionType.Swipe;

		public void Blit(IVideoElement elem, SKSurface source, SKCanvas dest, float t, bool isStartTransition)
		{
			var rect = new SKRect(
				elem.Position.X + (isStartTransition ? 0 : (1.0f - t) * elem.Size.Width),
				elem.Position.Y,
				elem.Position.X + (isStartTransition ? t * elem.Size.Width : elem.Size.Width),
				elem.Position.Y + elem.Size.Height * 2);

			var savePos = dest.Save();
			dest.ClipRect(rect);
			dest.DrawSurface(source, SKPoint.Empty);
			dest.RestoreToCount(savePos);
		}
	}
}
