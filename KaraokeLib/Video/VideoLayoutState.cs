using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KaraokeLib.Events;
using SkiaSharp;

namespace KaraokeLib.Video
{

    public class VideoLayoutState
	{
		private HashSet<int> _hyphenatedIds = new HashSet<int>();

		/// <summary>
		/// The maximum number of lines that can fit on the screen.
		/// </summary>
		public int NumLines { get; private set; } = 0;

		public VideoLayoutState(VideoContext context)
		{
			NumLines = CalculateNumLines(context.Style, context.Size);
		}

		public void Clear() => _hyphenatedIds.Clear();

		public void SetHyphenated(KaraokeEvent ev) => _hyphenatedIds.Add(ev.Id);

		public bool IsHyphenated(KaraokeEvent ev) => _hyphenatedIds.Contains(ev.Id);

		private int CalculateNumLines(VideoStyle style, SKSize size)
		{
			var lineHeight = style.LineHeight;
			var safeRect = style.GetSafeArea(size);

			var numLines = safeRect.Height / lineHeight;
			return Math.Max((int)Math.Floor(numLines), 1);
		}
	}
}
