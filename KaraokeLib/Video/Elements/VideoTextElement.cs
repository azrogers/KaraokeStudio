using KaraokeLib.Config;
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
    public class VideoTextElement : IVideoElement
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

		/// <inheritdoc />
		public int ParagraphId { get; private set; }

		/// <inheritdoc />
		public int Id { get; private set; }

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
			VideoLayoutState layoutState,
			string text,
			IEventTimecode startTimecode,
			IEventTimecode endTimecode,
			int id,
			int paragraphId = -1)
			: this(context, layoutState, new LyricsEvent[]
			{
				new LyricsEvent(LyricsEventType.Lyric, -1, startTimecode, endTimecode) { RawText = text }
			}, 0, id, paragraphId) { }

		/// <summary>
		/// Creates a VideoTextElement from at least one event on a single line.
		/// </summary>
		public VideoTextElement(
			VideoContext context,
			VideoLayoutState layoutState,
			IEnumerable<LyricsEvent> events,
			float yPos,
			int id,
			int paragraphId)
		{
			_context = context;
			_events = events.ToArray();
			StartTimecode = _earliestEventTimecode = events.Select(e => e.StartTime).Min() ?? new TimeSpanTimecode(TimeSpan.MinValue);
			EndTimecode = _latestEventTimecode = events.Select(e => e.EndTime).Max() ?? new TimeSpanTimecode(TimeSpan.MaxValue);

			CreateTransitions();

			StartTransition ??= new TransitionConfig();
			EndTransition ??= new TransitionConfig();
			ParagraphId = paragraphId;
			Id = id;

			// create line from events
			var safeArea = _context.Style.GetSafeArea(_context.Size);
			var totalWidth = 0f;
			_elementWidths = new float[_events.Length];

			var builder = new StringBuilder();
			for (var i = 0; i < _events.Length; i++)
			{
				// if we're not the first and this isn't a middle syllable, add a space
				var text = (i != 0 && _events[i].LinkedId == -1 ? " " : "") + _events[i].GetText(layoutState);
				_elementWidths[i] = _context.Style.GetTextWidth(text);
				totalWidth += _elementWidths[i];
				builder.Append(text);
			}

			_text = builder.ToString();
			_width = totalWidth;

			switch(context.Config.HorizontalAlign)
			{
				case HorizontalAlignment.Left:
					Position = (safeArea.Left, yPos);
					break;
				case HorizontalAlignment.Right:
					Position = (safeArea.Right - totalWidth, yPos);
					break;
				case HorizontalAlignment.Center:
					Position = (safeArea.Left + safeArea.Width / 2 - totalWidth / 2, yPos);
					break;
				default:
					throw new NotImplementedException($"Unknown enum value {context.Config.HorizontalAlign}");
			}
		}

		/// <summary>
		/// Returns the index into this VideoTextElement's text that the given position is located closest to.
		/// </summary>
		/// <param name="x">The X position to test, relative to <see cref="Position"/>.</param>
		/// <param name="y">The Y position to test, relative to <see cref="Position"/>.</param>
		/// <returns>The index into this element's text. Can be equal to the length of the text, which signifies that it's after the last character.</returns>
		public int GetCharOffset(float x, float y)
		{
			if(x < 0)
			{
				return 0;
			}

			if(x >= Size.Width)
			{
				return _text.Length;
			}

			var width = 0.0f;
			for(int i = 0; i < _text.Length; i++)
			{
				var ch = _text[i];
				var chWidth = _context.Style.GetTextWidth(ch.ToString());
				if(x >= width && x < (width + chWidth * 0.5f))
				{
					// first half of the char, select before
					return Math.Max(i -1, 0);
				}
				else if(x >= width && x < (width + chWidth))
				{
					// second half of the char, select after
					return i;
				}

				width += chWidth;
			}

			return _text.Length;
		}

		/// <summary>
		/// Returns the width of the amount of text before the given index.
		/// </summary>
		public float GetOffsetWidth(int offset)
		{
			if(offset == 0)
			{
				return 0;
			}

			if(offset >= _text.Length)
			{
				return Size.Width;
			}

			return _context.Style.GetTextWidth(_text.Substring(0, offset + 1));
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
		public (float, float) GetRenderedBounds(double position, (double, double) bounds)
		{
			return IsVisible(bounds) ? (Position.X, Position.X) : (Position.X, Position.X + _width);
		}

		/// <inheritdoc/>
		public void Render(VideoContext context, SKCanvas canvas, double videoPos)
		{
			var startSeconds = _earliestEventTimecode.GetTimeSeconds();
			var lineHeight = context.Style.LineHeight;

			var drawHighlighted = videoPos >= startSeconds;
			var drawNormal = true;

			var highlightPos = GetNormalizedPosition(videoPos);

			if (drawNormal)
			{
				canvas.Save();

				var rect = new SKRect(
					Position.X + (float)(highlightPos * _width),
					Position.Y,
					Position.X + _width,
					Position.Y + context.Size.Height);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.NormalPaint);

				canvas.Restore();
			}

			if (drawHighlighted)
			{
				canvas.Save();

				var rect = new SKRect(
					Position.X,
					Position.Y,
					Position.X + (float)(highlightPos * _width),
					Position.Y + context.Size.Height);

				canvas.ClipRect(rect, SKClipOperation.Intersect, true);
				canvas.DrawText(_text, new SKPoint(Position.Item1, Position.Item2 + lineHeight), context.Style.HighlightedPaint);

				canvas.Restore();
			}

			if ((drawNormal || drawHighlighted) && context.Config.StrokeWidth > 0)
			{
				canvas.Save();

				var rect = new SKRect(
					Position.Item1,
					Position.Item2,
					Position.Item1 + _width,
					Position.Y + context.Size.Height);

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
			if (time <= _earliestEventTimecode.GetTimeSeconds())
			{
				return 0;
			}

			if (time >= _latestEventTimecode.GetTimeSeconds())
			{
				return 1;
			}

			var accumWidth = 0f;
			for (var i = 0; i < _events.Length; i++)
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
