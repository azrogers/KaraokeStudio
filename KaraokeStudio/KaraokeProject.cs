using KaraokeLib;
using KaraokeLib.Lyrics;
using NAudio.Vorbis;
using NAudio.Wave;
using Newtonsoft.Json;

namespace KaraokeStudio
{
	internal class ProjectConfig : KaraokeConfig
	{
		public static ProjectConfig Copy(ProjectConfig otherConfig)
		{
			return JsonConvert.DeserializeObject<ProjectConfig>(JsonConvert.SerializeObject(otherConfig)) ?? new ProjectConfig();
		}

		public static ProjectConfig Load(string filename)
		{
			return LoadString(File.ReadAllText(filename));
		}

		public static ProjectConfig LoadString(string str)
		{
			var newConfig = new ProjectConfig();
			JsonConvert.PopulateObject(str, newConfig);
			return newConfig;
		}

		public void Save(string filename)
		{
			File.WriteAllText(filename, JsonConvert.SerializeObject(this));
		}
	}

	internal class KaraokeProject
	{
		public bool IsValid => _waveStream != null;

		public string AudioFile { get; private set; } = "";

		public TimeSpan Length { get; private set; }

		public ProjectConfig Config { get; set; }

		public IEnumerable<LyricsTrack> Tracks => _file.GetTracks();

		private WaveStream? _waveStream;
		private KsfLyricsFile _file;

		public KaraokeProject(string audioFile, KsfLyricsFile lyricsFile)
		{
			SetAudioFile(audioFile);
			if (_waveStream != null)
			{
				Length = _waveStream.TotalTime;
			}

			_file = lyricsFile;
			Config = new ProjectConfig();
		}

		public void Save(string outFile)
		{
			_file.SetMetadata("AudioFile", AudioFile);
			_file.SetMetadata("Length", Length.TotalSeconds.ToString());
			_file.SetMetadata("ProjectConfig", JsonConvert.SerializeObject(Config));

			using (var stream = File.OpenWrite(outFile))
			{
				_file.Save(stream);
			}
		}

		private void LoadAudioFile()
		{
			var ext = Path.GetExtension(AudioFile);
			if (ext.Equals(".wav", StringComparison.CurrentCultureIgnoreCase))
			{
				_waveStream = new WaveFileReader(AudioFile);
			}
			else if (ext.Equals(".ogg", StringComparison.CurrentCultureIgnoreCase))
			{
				_waveStream = new VorbisWaveReader(AudioFile);
			}
			else
			{
				Logger.ShowError(new UserException($"Unknown file extension {ext}"));
			}
		}

		private void SetAudioFile(string audioFile)
		{
			AudioFile = audioFile;
			LoadAudioFile();
		}

		public static KaraokeProject? Create(string audioPath)
		{
			var project = new KaraokeProject(audioPath, new KsfLyricsFile());

			return project.IsValid ? project : null;
		}

		public static KaraokeProject? FromMidi(string midiPath, string audioPath)
		{
			var midiFile = new MidiLyricsFile(midiPath);
			if (!midiFile.GetTracks().Any())
			{
				Logger.ShowError(new UserException("MIDI file contains no lyric tracks"));
				return null;
			}

			var project = new KaraokeProject(audioPath, new KsfLyricsFile(midiFile));
			return project.IsValid ? project : null;
		}

		public static KaraokeProject? Load(string projectPath)
		{
			KsfLyricsFile file;
			try
			{
				file = new KsfLyricsFile(projectPath);
			}
			catch (Exception e)
			{
				Logger.ShowError(new UserException(e));
				return null;
			}

			var audioFile = file.GetMetadata("AudioFile");
			if (audioFile == null)
			{
				Logger.ShowError(new UserException("Can't find AudioFile in KSF metadata"));
				return null;
			}

			var project = new KaraokeProject(audioFile, file);
			var length = file.GetMetadata("Length");
			if (!double.TryParse(length, out var lengthDouble))
			{
				Logger.ShowError(new UserException("Can't parse Length from KSF metadata"));
				return null;
			}

			project.Length = TimeSpan.FromSeconds(lengthDouble);

			var config = file.GetMetadata("ProjectConfig");
			if (config == null)
			{
				Logger.ShowError(new UserException("Can't find ProjectConfig in KSF metadata"));
				return null;
			}

			project.Config = ProjectConfig.LoadString(config);
			return project.IsValid ? project : null;
		}
	}
}
