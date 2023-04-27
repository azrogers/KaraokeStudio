using KaraokeLib.Lyrics;
using KaraokeLib.Video.Elements;
using KaraokeLib.Video.Plan;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	internal class VideoRenderer
	{
		private VideoContext _context;
		private VideoSection[] _sections;
		private IVideoElement[] _elements;

		public VideoRenderer(VideoContext context, VideoSection[] sections)
		{
			_context = context;
			_sections = sections;
			_elements = VideoElementGenerator.Generate(context, sections);
		}

		public void RenderFrame(VideoPlan videoPlan, VideoTimecode videoTimecode, SKCanvas canvas)
		{
			canvas.DrawRect(
				new SKRect(0, 0, _context.Size.Width, _context.Size.Height),
				new SKPaint()
				{
					Color = new SKColor(230, 230, 230),
					Style = SKPaintStyle.Fill
				});

			var posSeconds = videoTimecode.ToSeconds();
			var bounds = (posSeconds - _context.Config.LyricTrailTime, posSeconds + _context.Config.LyricLeadTime);

			var elements = videoPlan.GetElementsForFrame(videoTimecode);
			foreach (var ev in elements)
			{
				var minTime = Math.Max(ev.StartTime.ToSeconds(), bounds.Item1);
				var maxTime = Math.Min(ev.EndTime.ToSeconds(), bounds.Item2);
				ev.Element.Render(_context, canvas, posSeconds, (minTime, maxTime));
			}
		}
	}
}
