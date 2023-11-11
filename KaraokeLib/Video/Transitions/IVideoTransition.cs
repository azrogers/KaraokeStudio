using KaraokeLib.Util;
using KaraokeLib.Video.Elements;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video.Transitions
{
	/// <summary>
	/// Represents an in or out transition for an event.
	/// </summary>
	internal interface IVideoTransition
	{
		/// <summary>
		/// Returns the <see cref="VideoTransitionType"/> that this transition implements.
		/// </summary>
		VideoTransitionType Type { get; }

		/// <summary>
		/// Blits the <paramref name="source"/> surface to the <paramref name="destination"/> canvas 
		/// with a transition based on the normalized value <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The position between the beginning and the end of the transition as a normalized value between [0, 1].</param>
		/// <param name="isStartTransition">Is this a start transition (going from non-visible to visible) or an end transition (going from visible to non-visible).</param>
		void Blit(IVideoElement elem, TransitionContext context);
	}

	public enum VideoTransitionType
	{
		None = 0,
		Fade = 1,
		Swipe = 2
	}
}
