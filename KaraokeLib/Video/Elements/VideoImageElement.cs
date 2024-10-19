using KaraokeLib.Events;
using KaraokeLib.Util;
using KaraokeLib.Video.Transitions;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Elements
{
	internal class VideoImageElement : IVideoElement
	{
		public VideoElementType Type => VideoElementType.Image;

		public (float X, float Y) Position { get; private set; }

		public (float Width, float Height) Size { get; private set; }

		/// <inheritdoc />
		public IEventTimecode StartTimecode { get; set; }

		/// <inheritdoc />
		public IEventTimecode EndTimecode { get; set; }

		public TransitionConfig StartTransition { get; private set; }

		public TransitionConfig EndTransition { get; private set; }

		public int ParagraphId => _event.Id;

		public int Id => _event.Id;

		private ImageKaraokeEvent _event;
		private VideoContext _context;
		private (double, double)? _cachedVisibleBounds;
		private bool _cachedVisibleResult;
		private SKPaint _imagePaint;
		private SKBitmap? _imageBitmap;

		public VideoImageElement(VideoContext context, ImageKaraokeEvent ev)
		{
			_event = ev;
			_context = context;

			Size = (ev.Settings?.Size.Width ?? 0, ev.Settings?.Size.Height ?? 0);
			var origin = ev.Settings?.Origin.GetAnchorPosition(new SKSize(Size.Width, Size.Height)) ?? SKPoint.Empty;
			var position = ev.Settings?.Alignment.GetAnchorPosition(context.Size) ?? SKPoint.Empty;
			position -= origin;
			position += ev.Settings?.Offset ?? SKPoint.Empty;
			Position = (position.X, position.Y);

			StartTimecode = ev.StartTime;
			EndTimecode = ev.EndTime;

			_imagePaint = new SKPaint()
			{
				ColorF = new SKColorF(1.0f, 1.0f, 1.0f, ev.Settings?.Opacity ?? 1.0f)
			};

			_imageBitmap = SKBitmap.Decode(ev.Settings?.File);

			CreateTransitions();
		}

		public (float, float) GetRenderedBounds(double position, (double, double) bounds)
		{
			return (Position.X, Position.X + Size.Width);
		}

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

		public void Render(VideoContext context, SKCanvas canvas, double position)
		{
			var saveIndex = canvas.Save();

			if (_imageBitmap != null)
			{
				var rect = new SKRect(Position.X, Position.Y, Position.X + Size.Width, Position.Y + Size.Height);
				canvas.DrawBitmap(_imageBitmap, rect, _imagePaint);
			}

			canvas.RestoreToCount(saveIndex);
		}

		public void SetTiming(IEventTimecode newStartTimecode, IEventTimecode newEndTimecode)
		{
			StartTimecode = newStartTimecode;
			EndTimecode = newEndTimecode;
		}

		private void CreateTransitions()
		{
			var min = Math.Min(_context.Config.MinTransitionLength, _context.Config.MaxTransitionLength);
			var max = Math.Max(_context.Config.MinTransitionLength, _context.Config.MaxTransitionLength);

			StartTransition = new TransitionConfig()
			{
				Type = _context.Config.TransitionIn,
				EasingCurve = _context.Config.TransitionInCurve,
				Duration = (max - min) / 2 + min
			};

			EndTransition = new TransitionConfig()
			{
				Type = _context.Config.TransitionOut,
				EasingCurve = _context.Config.TransitionOutCurve,
				Duration = (max - min) / 2 + min
			};
		}

		public void Dispose()
		{
			_imageBitmap?.Dispose();
			_imagePaint.Dispose();
		}
	}
}
