using KaraokeLib;
using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files;
using KaraokeStudio.Util;
using Newtonsoft.Json;

namespace KaraokeStudio
{
	internal class KaraokeProject
	{
		public TimeSpan Length { get; private set; }

		public KaraokeConfig Config { get; set; }

		public IEnumerable<KaraokeTrack> Tracks => _file.GetTracks();

		public AudioMixer Mixer { get; private set; }

		private KsfKaraokeFile _file;

		public KaraokeProject(TimeSpan length, KsfKaraokeFile lyricsFile)
		{
			Length = length;
			_file = lyricsFile;
			Config = new KaraokeConfig();
			Mixer = new AudioMixer(lyricsFile.GetTracks());
		}

		public KaraokeTrack AddTrack(KaraokeTrackType type)
		{
			var track = _file.AddTrack(type);
			return track;
		}

		public void UpdateMixer()
		{
			var pos = Mixer.Position;
			Mixer = new AudioMixer(_file.GetTracks());
			Mixer.Position = pos;
		}

		public void Save(string outFile)
		{
			_file.SetMetadata("Length", Length.TotalSeconds.ToString());
			_file.SetMetadata("ProjectConfig", JsonConvert.SerializeObject(Config));

			using (var stream = File.OpenWrite(outFile))
			{
				_file.Save(stream);
			}
		}

		public static KaraokeProject? FromMidi(string midiPath, string audioPath)
		{
			var midiFile = new MidiKaraokeFile(midiPath);
			if (!midiFile.GetTracks().Any())
			{
				ExceptionLogger.ShowError(new UserException("MIDI file contains no lyric tracks"));
				return null;
			}

			return Create(audioPath, new KsfKaraokeFile(midiFile));
		}

		public static KaraokeProject? Create(string audioPath) => Create(audioPath, new KsfKaraokeFile());

		private static KaraokeProject Create(string audioPath, KsfKaraokeFile file)
		{
			var fileInfo = AudioUtil.GetFileInfo(audioPath);
			if (fileInfo == null)
			{
				throw new UserException($"Not a valid audio file: {audioPath}");
			}

			var settings = new AudioClipSettings(audioPath);
			var track = file.AddTrack(KaraokeTrackType.Audio);
			track.AddAudioClipEvent(settings, new TimeSpanTimecode(0), new TimeSpanTimecode(fileInfo.LengthSeconds));

			var project = new KaraokeProject(TimeSpan.FromSeconds(fileInfo.LengthSeconds), file);

			return project;
		}

		public static KaraokeProject? Load(string projectPath)
		{
			KsfKaraokeFile file;
#if !DEBUG
			try
			{
#endif
				file = new KsfKaraokeFile(projectPath);
#if !DEBUG
		}
			catch (Exception e)
			{
				Logger.ShowError(new UserException(e));
				return null;
			}
#endif

			var length = file.GetMetadata("Length");
			if (!double.TryParse(length, out var lengthDouble))
			{
				ExceptionLogger.ShowError(new UserException("Can't parse Length from KSF metadata"));
				return null;
			}

			var endTimespan = TimeSpan.FromSeconds(lengthDouble);

			var audioFile = file.GetMetadata("AudioFile");
			if (audioFile != null)
			{
				// port over to new audio clip
				var audioTrack = file.AddTrack(KaraokeTrackType.Audio);
				var settings = new AudioClipSettings(audioFile);
				audioTrack.AddAudioClipEvent(settings, new TimeSpanTimecode(0), new TimeSpanTimecode(endTimespan));

				file.RemoveMetadata("AudioFile");
			}

			var project = new KaraokeProject(endTimespan, file);

			var config = file.GetMetadata("ProjectConfig");
			if (config == null)
			{
				ExceptionLogger.ShowError(new UserException("Can't find ProjectConfig in KSF metadata"));
				return null;
			}

			project.Config = new KaraokeConfig(config);
			return project;
		}
	}
}
