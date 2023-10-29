using KaraokeLib.Files;
using KaraokeLib.Video;

namespace MidiToKaraoke
{
	internal class Program
	{
		static void Main(string[] args)
		{
			var lyricsFile = new MidiKaraokeFile("notes.mid");
			var video = new VideoGenerator(lyricsFile, "song.ogg");
			video.RenderFrameToFile(new VideoTimecode(20.0, 30), "test.png");
			//video.RenderVideo(0.0, 185.0, "test.mp4");
		}
	}
}