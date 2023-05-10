using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkiaSharp;

namespace KaraokeLib.Video
{
	public class VideoContext
	{
		public VideoStyle Style { get; private set; }

		public SKSize Size { get; private set; }

		public int NumLines { get; private set; }

		public KaraokeConfig Config { get; private set; }

		public VideoTimecode LastFrameTimecode { get; private set; }

		public VideoContext(VideoStyle style, KaraokeConfig config, VideoTimecode lastFrameTimecode)
		{
			Style = style;
			Config = config;
			Size = Config.VideoSize;
			LastFrameTimecode = lastFrameTimecode;
			NumLines = CalculateNumLines();

			if(NumLines <= 0)
			{
				throw new InvalidDataException("NumLines <= 0");
			}
		}

		private int CalculateNumLines()
		{
			var lineHeight = Style.LineHeight;
			var safeRect = Style.GetSafeArea(Size);

			var numLines = safeRect.Height / lineHeight;
			return Math.Max((int)Math.Floor(numLines), 1);
		}
	}
}
