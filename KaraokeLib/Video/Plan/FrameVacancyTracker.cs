using KaraokeLib.Video.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Plan
{

	/// <summary>
	/// Tracks which parts of the frame are vacant (can fit a new event)
	/// </summary>
	internal class FrameVacancyTracker
	{
		private float _lineHeight = 0;
		// line height indexing lists of (xmin, xmax) tuples specifying which parts of the line are occupied
		private Dictionary<float, List<(float, float)>> _lineOccupants = new Dictionary<float, List<(float, float)>>();
		private double _videoPosition;
		private (double, double) _bounds;

		public FrameVacancyTracker(VideoContext context, double videoPosition, (double, double) bounds)
		{
			_lineHeight = context.Style.LineHeight;
			_videoPosition = videoPosition;
			_bounds = bounds;
		}

		public bool AddOccupant(IVideoElement element)
		{
			var yPos = (float)Math.Round(element.Position.Item2, 2);
			var thisRange = element.GetRenderedBounds(_videoPosition, _bounds);
			if (!_lineOccupants.ContainsKey(yPos))
			{
				_lineOccupants[yPos] = new List<(float, float)>() { thisRange };
				return true;
			}

			var list = _lineOccupants[yPos];
			foreach (var item in list)
			{
				if (
					//   [item]
					// [this]
					(item.Item1 >= thisRange.Item1 && item.Item1 < thisRange.Item2) ||
					// [item]
					//   [this]
					(item.Item2 > thisRange.Item1 && item.Item2 < thisRange.Item2) ||
					// [--item--]
					//   [this]
					(thisRange.Item1 >= item.Item1 && thisRange.Item2 < item.Item2))
				{
					return false;
				}
			}

			list.Add(thisRange);
			return true;
		}
	}
}
