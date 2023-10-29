using FFMediaToolkit.Decoding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace KaraokeLib.Audio
{
	public class AudioUtil
	{
		public static AudioFileInfo? GetFileInfo(string filename)
		{
			var file = MediaFile.Open(filename);

			if (file.AudioStreams.Length == 0)
			{
				throw new InvalidDataException("Audio file must have at least one stream");
			}

			var streamInfo = file.AudioStreams[0].Info;

			return new AudioFileInfo()
			{
				LengthSeconds = streamInfo.Duration.TotalSeconds,
				SampleRate = streamInfo.SampleRate,
				FormatType = GetFormat(file.Info)
			};
		}

		private static AudioFormatType GetFormat(MediaInfo mediaInfo)
		{
			switch(mediaInfo.ContainerFormat)
			{
				case "mp3":
					return AudioFormatType.Mp3;
				case "ogg":
					return AudioFormatType.Ogg;
				case "wav":
					return AudioFormatType.Wav;
			}

			return AudioFormatType.Invalid;
		}

		public class AudioFileInfo
		{
			/// <summary>
			/// The length of this file in seconds.
			/// </summary>
			public double LengthSeconds;

			/// <summary>
			/// The number of samples per second in this file.
			/// </summary>
			public int SampleRate;

			/// <summary>
			/// 
			/// </summary>
			public AudioFormatType FormatType = AudioFormatType.Invalid;
		}

		public enum AudioFormatType
		{
			Invalid,
			Mp3,
			Ogg,
			Wav,
		}
	}
}
