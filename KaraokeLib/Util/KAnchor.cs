using SkiaSharp;

namespace KaraokeLib.Util
{
	/// <summary>
	/// Represents one of nine anchor points of an element. 
	/// </summary>
	public enum KAnchor : byte
	{
		None = 0,
		TopLeft = 1,
		MiddleLeft = 2,
		BottomLeft = 3,
		TopCenter = 4,
		MiddleCenter = 5,
		BottomCenter = 6,
		TopRight = 7,
		MiddleRight = 8,
		BottomRight = 9
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
