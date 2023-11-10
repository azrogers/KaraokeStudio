using KaraokeLib.Events;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.Timeline.EventRenderers
{
    internal interface ICustomEventRenderer
    {
        void RecreateContext();
        void Render(SKCanvas canvas, SKRect rect, KaraokeEvent ev);
    }
}
