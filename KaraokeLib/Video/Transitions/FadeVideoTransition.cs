using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Transitions
{
	internal class FadeVideoTransition : IVideoTransition
	{
		public VideoTransitionType Type => VideoTransitionType.Fade;

		public void Blit(IVideoElement elem, SKSurface surface, SKCanvas canvas, float t, bool isStartTransition)
		{
			var paint = new SKPaint()
			{
					ColorF = new SKColorF(1.0f, 1.0f, 1.0f, t)
			};

			canvas.DrawSurface(surface, SKPoint.Empty, paint);
		}
	}
}
