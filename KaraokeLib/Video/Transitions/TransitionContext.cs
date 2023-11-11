using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Transitions
{
	internal struct TransitionContext
	{
		public VideoContext VideoContext;
		public SKSurface Surface;
		public SKCanvas Destination;
		public float TransitionPosition;
		public bool IsStartTransition;
		public double VideoPosition;
	}
}
