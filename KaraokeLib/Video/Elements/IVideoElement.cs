using KaraokeLib.Events;
using KaraokeLib.Video.Transitions;
using SkiaSharp;

namespace KaraokeLib.Video.Elements
{
	// a single element in the video (a renderable bit of text, an intermission, etc)
	public interface IVideoElement : IDisposable
	{
		/// <summary>
		/// The type of element this class implements.
		/// </summary>
		VideoElementType Type { get; }

		/// <summary>
		/// The position of this element on the screen.
		/// </summary>
		(float X, float Y) Position { get; }

		/// <summary>
		/// The (Width, Height) of the element in pixels.
		/// </summary>
		(float Width, float Height) Size { get; }

		/// <summary>
		/// The timecode of the start of the event.
		/// </summary>
		IEventTimecode StartTimecode { get; }

		/// <summary>
		/// The timecode of the end of the event.
		/// </summary>
		IEventTimecode EndTimecode { get; }

		/// <summary>
		/// The transition going from not visible to visible.
		/// </summary>
		TransitionConfig StartTransition { get; }

		/// <summary>
		/// The transition going from visible to not visible.
		/// </summary>
		TransitionConfig EndTransition { get; }

		/// <summary>
		/// Used to keep track of which paragraph this video element belongs to.
		/// </summary>
		int ParagraphId { get; }

		/// <summary>
		/// The unique ID of this element.
		/// </summary>
		int Id { get; }

		/// <summary>
		/// Change the timing of this element.
		/// </summary>
		void SetTiming(IEventTimecode newStartTimecode, IEventTimecode newEndTimecode);

		/// <summary>
		/// Checks whether this element overlaps with the timeframe specified in <paramref name="bounds"/>.
		/// </summary>
		/// <param name="bounds">The beginning and end of the timeframe to check, in seconds.</param>
		bool IsVisible((double, double) bounds);

		/// <summary>
		/// Renders this element at the given time.
		/// </summary>
		/// <param name="position">The current point in the video.</param>
		void Render(VideoContext context, SKCanvas canvas, double position);

		/// <summary>
		/// Gets the (xmin, xmax) of this element when rendered at the current video position.
		/// </summary>
		(float, float) GetRenderedBounds(double position, (double, double) bounds);
	}

	public enum VideoElementType
	{
		Text,
		Image
	}
}
