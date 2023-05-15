using KaraokeLib.Lyrics;
using KaraokeLib.Video.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Elements
{
	/// <summary>
	/// Represents a bit of timed text on the screen, usually lyrics.
	/// Can represent one or more LyricsEvents.
	/// </summary>
	internal class VideoTextElement : IVideoElement
	{
		/// <inheritdoc />
		public VideoElementType Type { get; private set; }

		/// <inheritdoc />
		public (float X, float Y) Position { get; set; }

		/// <inheritdoc />
		public (float Width, float Height) Size => (_width, _context.Style.LineHeight);

		/// <inheritdoc />
		public IEventTimecode StartTimecode { get; set; }

		/// <inheritdoc />
		public IEventTimecode EndTimecode { get; set; }

		/// <inheritdoc />
		public TransitionConfig StartTransition { get; set; }

		/// <inheritdoc />
		public TransitionConfig EndTransition { get; set; }

		private (double, double)? _cachedVisibleBounds;
		private bool _cachedVisibleResult;
		private IEventTimecode _earliestEventTimecode;
		private IEventTimecode _latestEventTimecode;
		private string _text;
		private VideoContext _context;
		private float _width;
		private LyricsEvent[] _events;
		private float[] _elementWidths;

		/// <summary>
		/// Creates a VideoTextElement from the given text and start and end times.
		/// <see cref="Position"/> must be set manually.
		/// </summary>
		public VideoTextElement(
			VideoContext context,
			string text,
			IEventTimecode startTimecode,
			IEventTimecode endTimecode)
			: this(context, new LyricsEvent[]
			{
				new LyricsEvent(LyricsEventType.Lyric, -1, startTimecode, endTimecode) { Text = text }
			}, 0) { }

		/// <summary>
		/// Creates a VideoTextElement from at least one event on a single line.
		/// </summary>
		public VideoTextElement(
			VideoContext context,
			IEnumerable<LyricsEvent> events,
			float yPos)
		{
			_context = context;
			_events = events.ToArray();
			StartTimecode = _earliestEventTimecode = events.Select(e => e.StartTime).Min() ?? new TimeSpanTimecode(TimeSpan.MinValue);
			EndTimecode = _latestEventTimecode = events.Select(e => e.EndTime).Max() ?? new TimeSpanTimecode(TimeSpan.MaxValue);

			CreateTransitions();

			StartTransition ??= new TransitionConfig();
			EndTransition ??= new TransitionConfig();

			// create line from events
			var safeArea = _context.Style.GetSafeArea(_context.Size);
			var totalWidth = 0f;
			_elementWidths = new float[_events.Length];

			var builder = new StringBuilder();
			for (var i = 0; i < _events.Length; i++)
			{
				// if we're not the first and this isn't a middle syllable, add a space
				var text = (i != 0 && _events[i].LinkedId == -1 ? " " : "") + _events[i].Text;
				_elementWidths[i] = _context.Style.GetTextWidth(text);
				totalWidth += _elementWidths[i];
				builder.Append(text);
			}

			_text = builder.ToString();
			_width = totalWidth;

			var textXPos = safeArea.Left + safeArea.Width / 2 - totalWidth / 2;
			Position = (textXPos, yPos);
		}

		/// <inheritdoc />
		public void SetTiming(IEventTimecode newStart, IEventTimecode newEnd)
		{
			StartTimecode = newStart;
			EndTimecode = newEnd;

			CreateTransitions();
		}

		/// <inheritdoc/>
		public bool IsVisible((double, double) bounds)
		{
			if (_cachedVisibleBounds == bounds)
			{
				return _cachedVisibleResult;
			}

			var earliest = bounds.Item1;
			var latest = bounds.Item2;
			var startSeconds = StartTimecode.GetTimeSeconds();
			var endSeconds = EndTimecode.GetTimeSeconds();

			_cachedVisibleResult =
				(startSeconds >= earliest && startSeconds < latest) ||
				(endSeconds >= earliest && endSeconds < latest);
			_cachedVisibleBounds = bounds;
			return _cachedVisibleResult;
		}

		/// <inheritdoc/>
		public VideoElementPriority GetPriority(double position, (double, double) bounds)
		{
			var startSeconds = _earliestEventTimecode.GetTimeSeconds();
			var endSeconds = _latestEventTimecode.GetTimeSeconds();

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
			if (endSeconds <= position && endSeconds >= bounds.Item1)
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
			if (endSeconds <= bounds.Item1)
			{
				return VideoElementPriority.BeforeOutOfRange;
			}

			throw new InvalidOperationException("Corner case not considered?");
		}

		/// <inheritdoc/>
		public (float, float) GetRenderedBounds(double position, (double, double) bounds)
		{
			return IsVisible(bounds) ? (Position.X, Position.X) : (Position.X, Position.X + _width);
			// below code is for a swipe transition, which is not implemented yet (TODO)
			/*
			var earliest = bounds.Item1;
			var latest = bounds.Item2;
			var startSeconds = _startTimecode.GetTimeSeconds();
			var endSeconds = _endTimecode.GetTimeSeconds();

			var normalizedStartPos = Math.Clamp((earliest - startSeconds) / (endSeconds - startSeconds), 0, 1);
			var normalizedEndPos = Math.Clamp((latest - startSeconds) / (endSeconds - startSeconds), 0, 1);
			return (Position.Item1 + (float)normalizedStartPos * _width, Position.Item1 + (float)normalizedEndPos * _width);
			*/
		}

		/// <inheritdoc/>
		public void Render(VideoContext context, SKCanvas canvas, double videoPos)
		{
			/*if (!IsVisible(bounds))
			{
				// we don't need to render anything
				return;
			}*/

			// FOR NOW, the only clipping we need to worry about is for the highlighted position
			// no transitions implemented yet

			//var earliest = bounds.Item1;
			//var latest = bounds.Item2;
			var startSeconds = _earliestEventTimecode.GetTimeSeconds();
			//var endSeconds = _endTimecode.GetTimeSeconds();
			var lineHeight = context.Style.LineHeight;

			var drawHighlighted = videoPos >= startSeconds;
			var drawNormal = true;//videoPos < endSeconds;

			var highlightPos = GetNormalizedPosition(videoPos);

			//var normalizedStartPos = Math.Clamp((earliest - startSeconds) / (endSeconds - startSeconds), 0, 1);
			//var normalizedEndPos = 1.0;

			if(drawNormal)
			{
				canvas.Save();

				/*normalizedEndPos = Math.Clamp((latest - startSeconds) / (endSeconds - startSeconds), 0, 1);
				var relativeXRange = ((float)normalizedStartPos * _width, (float)normalizedEndPos * _width);
				var rect = new SKRect(
					Position.Item1 + relativeXRange.Item1,
					Position.Item2,
					Position.Item1 + relativeXRange.Item2,
					context.Size.Height
				);*/
				var rect = new SKRect(
					Position.X + (float)(highlightPos * _width),
					Position.Y,
					Position.X + _width,
					context.Size.Height);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.NormalPaint);

				canvas.Restore();
			}

			if(drawHighlighted)
			{
				canvas.Save();

				/*var highlightedEndPos = Math.Clamp((videoPos - startSeconds) / (endSeconds - startSeconds), 0, 1);
				var relativeXRange = ((float)normalizedStartPos * _width, (float)highlightedEndPos * _width);
				var rect = new SKRect(
					Position.Item1 + relativeXRange.Item1,
					Position.Item2,
					Position.Item1 + relativeXRange.Item2,
					context.Size.Height
				);*/
				var rect = new SKRect(
					Position.X,
					Position.Y,
					Position.X + (float)(highlightPos * _width),
					context.Size.Height);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.HighlightedPaint);

				canvas.Restore();
			}

			if(drawNormal || drawHighlighted)
			{
				canvas.Save();

				//var relativeXRange = ((float)normalizedStartPos * _width, (float)normalizedEndPos * _width);
				var rect = new SKRect(
					Position.Item1,// + relativeXRange.Item1,
					Position.Item2,
					Position.Item1 + _width,// + relativeXRange.Item2,
					context.Size.Height);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.StrokePaint);

				canvas.Restore();
			}
		}

		/// <summary>
		/// Returns the position of the cursor at the given time within the line, normalized to [0, 1].
		/// </summary>
		/// <remarks>
		/// This calculation is based on each individual event contained in the element.
		/// </remarks>
		private double GetNormalizedPosition(double time)
		{
			if(time <= _earliestEventTimecode.GetTimeSeconds())
			{
				return 0;
			}

			if(time >= _latestEventTimecode.GetTimeSeconds())
			{
				return 1;
			}

			var accumWidth = 0f;
			for(var i = 0; i < _events.Length; i++)
			{
				if (_events[i].ContainsTime(time))
				{
					// use element width to get exact position in the line
					var normWidth = _events[i].GetNormalizedPosition(time) * _elementWidths[i];
					return (accumWidth + normWidth) / _width;
				}
				else if (_events[i].StartTimeSeconds > time)
				{
					// we're in-between events, so return the last position
					return accumWidth / _width;
				}

				accumWidth += _elementWidths[i];
			}

			return 0;
		}

		private void CreateTransitions()
		{
			var min = Math.Min(_context.Config.MinTransitionLength, _context.Config.MaxTransitionLength);
			var max = Math.Max(_context.Config.MinTransitionLength, _context.Config.MaxTransitionLength);

			var startDuration = Math.Clamp(_earliestEventTimecode.GetTimeSeconds() - StartTimecode.GetTimeSeconds(), min, max);
			StartTransition = new TransitionConfig()
			{
				Type = _context.Config.TransitionIn,
				EasingCurve = _context.Config.TransitionInCurve,
				Duration = startDuration
			};

			var endDuration = Math.Clamp(EndTimecode.GetTimeSeconds() - _latestEventTimecode.GetTimeSeconds(), min, max);
			EndTransition = new TransitionConfig()
			{
				Type = _context.Config.TransitionOut,
				EasingCurve = _context.Config.TransitionOutCurve,
				Duration = endDuration
			};
		}
	}
}
