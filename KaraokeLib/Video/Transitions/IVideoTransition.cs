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
	public interface IVideoTransition
	{
		/// <summary>
		/// Blits the <paramref name="source"/> surface to the <paramref name="destination"/> canvas 
		/// with a transition based on the normalized value <paramref name="t"/>.
		/// </summary>
		/// <param name="t">The position between the beginning and the end of the transition as a normalized value between [0, 1].</param>
		void Blit(SKSurface source, SKCanvas destination, float t);
	}

	public enum VideoTransitionType
	{
		None = 0,
		Fade = 1,
		Slide = 2
	}

	public class TransitionConfig
	{
		public VideoTransitionType Type { get; set; }

		public double Duration { get; set; }

		
	}
}
