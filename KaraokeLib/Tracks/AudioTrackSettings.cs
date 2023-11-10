using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Tracks
{
	public class AudioTrackSettings : EditableConfig<AudioTrackSettings>
	{
		public bool Muted = false;

		/// <summary>
		/// The volume of the audio track.
		/// </summary>
		/// <remarks>
		/// Currently just a linear multiplier of signal value.
		/// </remarks>
		[ConfigRange(0, 2.0)]
		public float Volume = 1.0f;
	}
}
