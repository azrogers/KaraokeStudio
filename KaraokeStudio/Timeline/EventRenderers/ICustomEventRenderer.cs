using KaraokeLib.Events;
using SkiaSharp;

namespace KaraokeStudio.Timeline.EventRenderers
{
	internal interface ICustomEventRenderer
	{
		void RecreateContext();
		void Render(SKCanvas canvas, SKRect rect, KaraokeEvent ev);
	}
}
