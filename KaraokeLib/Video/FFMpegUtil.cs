﻿using FFMediaToolkit;
using System.Diagnostics;

namespace KaraokeLib.Video
{
	public class FFMpegUtil
	{
		public static void MuxVideoAudio(
			string videoPath,
			string audioPath,
			double audioPosition,
			double length,
			string outputPath
		)
		{
			RunCommand(
				$"-i {FormatPath(videoPath)}",
				$"-ss {SecondsToFfmpegTimecode(audioPosition)}",
				$"-i {FormatPath(audioPath)}",
				$"-t {length}",
				"-c:v copy",
				"-c:a aac",
				"-y",
				FormatPath(outputPath));
		}

		private static void RunCommand(params string[] arguments)
		{
			var process = new Process();
			var startInfo = new ProcessStartInfo
			{
				WindowStyle = ProcessWindowStyle.Normal,
				FileName = GetFFmpegPath(),
				Arguments = string.Join(" ", arguments)
			};

			process.StartInfo = startInfo;
			process.EnableRaisingEvents = true;
			process.Start();
			process.WaitForExit();
		}

		public static void SetupFfmpegPath()
		{
			FFmpegLoader.FFmpegPath = Path.Join(Environment.CurrentDirectory, "ffmpeg", "x86_64");
		}

		private static string GetFFmpegPath()
		{
			return Path.Join(FFmpegLoader.FFmpegPath, "ffmpeg.exe");
		}

		private static string FormatPath(string path)
		{
			return "\"" + path + "\"";
		}

		private static string SecondsToFfmpegTimecode(double seconds)
		{
			var span = TimeSpan.FromSeconds(seconds);
			return $"{span.Hours:00}:{span.Minutes:00}:{span.Seconds:00}.{span.Milliseconds}";
		}
	}
}
