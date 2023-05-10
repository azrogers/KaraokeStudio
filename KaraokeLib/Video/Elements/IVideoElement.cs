using KaraokeLib.Lyrics;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Elements
{
	// a single element in the video (a renderable bit of text, an intermission, etc)
	internal interface IVideoElement
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
		/// The timecode of the start of the event.
		/// </summary>
		IEventTimecode StartTimecode { get; set; }

		/// <summary>
		/// The timecode of the end of the event.
		/// </summary>
		IEventTimecode EndTimecode { get; set; }

		/// <summary>
		/// Checks whether this element overlaps with the timeframe specified in <paramref name="bounds"/>.
		/// </summary>
		/// <param name="bounds">The beginning and end of the timeframe to check, in seconds.</param>
		bool IsVisible((double, double) bounds);

		/// <summary>
		/// Renders this element at the given time.
		/// </summary>
		/// <param name="position">The current point in the video.</param>
		/// <param name="bounds">The (earliest, latest) time that elements should be visible.</param>
		void Render(VideoContext context, SKCanvas canvas, double position, (double, double) bounds);

		/// <summary>
		/// Gets the element's priority.
		/// </summary>
		/// <param name="position">The current video position.</param>
		/// <param name="bounds">The bounds of the grace period.</param>
		VideoElementPriority GetPriority(double position, (double, double) bounds);

		/// <summary>
		/// Gets the (xmin, xmax) of this element when rendered at the current video position.
		/// </summary>
		(float, float) GetRenderedBounds(double position, (double, double) bounds);
	}

	internal enum VideoElementType
	{
		Text,
	}

	/// <summary>
	/// The order in which to render elements. 
	/// Lower = higher priority.
	/// </summary>
	internal enum VideoElementPriority
	{
		/// <summary>
		/// The current element at the current video position
		/// </summary>
		Current = 0,
		/// <summary>
		/// After the current video position but within the grace period.
		/// </summary>
		AfterCurrent = 1,
		/// <summary>
		/// Before the current video position but within the grace period.
		/// </summary>
		BeforeCurrent = 2,
		/// <summary>
		/// After the current video position and outside of the grace period.
		/// </summary>
		AfterOutOfRange = 3,
		/// <summary>
		/// Before the current video position and outside of the grace period.
		/// </summary>
		BeforeOutOfRange = 4
	}
}
