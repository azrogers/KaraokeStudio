using KaraokeLib.Lyrics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Elements
{
	internal class VideoTextElement : IVideoElement
	{
		public VideoElementType Type { get; private set; }

		public (float, float) Position { get; set; }

		public float Width => _width;

		public double StartPos => _startTimecode.GetTimeSeconds();

		private (double, double)? _cachedVisibleBounds;
		private bool _cachedVisibleResult;
		private IEventTimecode _startTimecode;
		private IEventTimecode _endTimecode;
		private string _text;
		private VideoContext _context;
		private float _width;

		public VideoTextElement(
			VideoContext context, 
			string text,
			IEventTimecode startTimecode, 
			IEventTimecode endTimecode)
		{
			_context = context;
			_text = text;
			_startTimecode = startTimecode;
			_endTimecode = endTimecode;
			_width = context.Style.GetTextWidth(_text);
		}

		public bool IsVisible((double, double) bounds)
		{
			if (_cachedVisibleBounds == bounds)
			{
				return _cachedVisibleResult;
			}

			var earliest = bounds.Item1;
			var latest = bounds.Item2;
			var startSeconds = _startTimecode.GetTimeSeconds();
			var endSeconds = _endTimecode.GetTimeSeconds();

			_cachedVisibleResult =
				(startSeconds >= earliest && startSeconds < latest) ||
				(endSeconds >= earliest && endSeconds < latest);
			_cachedVisibleBounds = bounds;
			return _cachedVisibleResult;
		}

		public VideoElementPriority GetPriority(double position, (double, double) bounds)
		{
			var startSeconds = _startTimecode.GetTimeSeconds();
			var endSeconds = _endTimecode.GetTimeSeconds();

			//                   v
			// -------{-------[event]-------}-------
			if (position >= startSeconds && position < endSeconds)
			{
				return VideoElementPriority.Current;
			}

			//                   v
			// -------{------------[event]--}-------
			if (startSeconds >= position && startSeconds < bounds.Item2)
			{
				return VideoElementPriority.AfterCurrent;
			}

			//                   v
			// -------{--[event]------------}-------
			if (endSeconds < position && endSeconds >= bounds.Item1)
			{
				return VideoElementPriority.BeforeCurrent;
			}

			//                   v
			// -------{---------------------}[event]
			if(startSeconds >= bounds.Item2)
			{
				return VideoElementPriority.AfterOutOfRange;
			}

			//                   v
			// [event]{---------------------}-------
			if (endSeconds < bounds.Item1)
			{
				return VideoElementPriority.BeforeOutOfRange;
			}

			throw new InvalidOperationException("Corner case not considered?");
		}

		public (float, float) GetRenderedBounds(double position, (double, double) bounds)
		{
			var earliest = bounds.Item1;
			var latest = bounds.Item2;
			var startSeconds = _startTimecode.GetTimeSeconds();
			var endSeconds = _endTimecode.GetTimeSeconds();

			var normalizedStartPos = Math.Clamp((earliest - startSeconds) / (endSeconds - startSeconds), 0, 1);
			var normalizedEndPos = Math.Clamp((latest - startSeconds) / (endSeconds - startSeconds), 0, 1);
			return (Position.Item1 + (float)normalizedStartPos * _width, Position.Item1 + (float)normalizedEndPos * _width);
		}

		public void Render(VideoContext context, SKCanvas canvas, double videoPos, (double, double) bounds)
		{
			/*if (!IsVisible(bounds))
			{
				// we don't need to render anything
				return;
			}*/

			var earliest = bounds.Item1;
			var latest = bounds.Item2;
			var startSeconds = _startTimecode.GetTimeSeconds();
			var endSeconds = _endTimecode.GetTimeSeconds();
			var lineHeight = context.Style.LineHeight;

			var drawHighlighted = videoPos >= startSeconds;
			var drawNormal = videoPos < endSeconds;

			var normalizedStartPos = Math.Clamp((earliest - startSeconds) / (endSeconds - startSeconds), 0, 1);
			var normalizedEndPos = 1.0;

			if(drawNormal)
			{
				canvas.Save();

				normalizedEndPos = Math.Clamp((latest - startSeconds) / (endSeconds - startSeconds), 0, 1);
				var relativeXRange = ((float)normalizedStartPos * _width, (float)normalizedEndPos * _width);
				var rect = new SKRect(
					Position.Item1 + relativeXRange.Item1,
					Position.Item2,
					Position.Item1 + relativeXRange.Item2,
					context.Size.Height
				);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.NormalPaint);

				canvas.Restore();
			}

			if(drawHighlighted)
			{
				canvas.Save();

				var highlightedEndPos = Math.Clamp((videoPos - startSeconds) / (endSeconds - startSeconds), 0, 1);
				var relativeXRange = ((float)normalizedStartPos * _width, (float)highlightedEndPos * _width);
				var rect = new SKRect(
					Position.Item1 + relativeXRange.Item1,
					Position.Item2,
					Position.Item1 + relativeXRange.Item2,
					context.Size.Height
				);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.HighlightedPaint);

				canvas.Restore();
			}

			if(drawNormal || drawHighlighted)
			{
				canvas.Save();

				var relativeXRange = ((float)normalizedStartPos * _width, (float)normalizedEndPos * _width);
				var rect = new SKRect(
					Position.Item1 + relativeXRange.Item1,
					Position.Item2,
					Position.Item1 + relativeXRange.Item2,
					context.Size.Height);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.StrokePaint);

				canvas.Restore();
			}
		}
	}
}
