using SkiaSharp;

namespace KaraokeLib.Util
{
	/// <summary>
	/// Represents one of nine anchor points of an element. 
	/// </summary>
	public enum KAnchor : byte
	{
		TopLeft = 0,
		MiddleLeft = 1,
		BottomLeft = 2,
		TopCenter = 3,
		MiddleCenter = 4,
		BottomCenter = 5,
		TopRight = 6,
		MiddleRight = 7,
		BottomRight = 8
	}

	public static class KAnchorExtensions
	{
		public static SKPoint GetAnchorPosition(this KAnchor anchor, SKSize size)
		{
			var xPos = 0.0f;
			if (anchor == KAnchor.TopCenter || anchor == KAnchor.MiddleCenter || anchor == KAnchor.BottomCenter)
			{
				xPos = size.Width / 2;
			}
			else if (anchor == KAnchor.TopRight || anchor == KAnchor.MiddleRight || anchor == KAnchor.BottomRight)
			{
				xPos = size.Width;
			}

			var yPos = 0.0f;
			if (anchor == KAnchor.MiddleLeft || anchor == KAnchor.MiddleCenter || anchor == KAnchor.MiddleRight)
			{
				yPos = size.Height / 2;
			}
			else if (anchor == KAnchor.BottomLeft || anchor == KAnchor.BottomCenter || anchor == KAnchor.BottomRight)
			{
				yPos = size.Height;
			}

			return new SKPoint(xPos, yPos);
		}
	}
}
