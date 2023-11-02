using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Files;
using KaraokeLib.Tracks;
using KaraokeStudio.Util;

namespace KaraokeStudio
{
    internal class KaraokeProject
	{
		public TimeSpan Length
		{
			get => TimeSpan.FromSeconds(_file.LengthSeconds);
			set => _file.LengthSeconds = value.TotalSeconds;
		}

		public KaraokeConfig Config
		{
			get => _file.Config;
			set => _file.Config = value;
		}

		public IEnumerable<KaraokeTrack> Tracks => _file.GetTracks();

		public ProjectPlaybackState PlaybackState { get; private set; }

		private KsfKaraokeFile _file;

		public KaraokeProject(KsfKaraokeFile lyricsFile)
		{
			_file = lyricsFile;
			PlaybackState = new ProjectPlaybackState(this, lyricsFile.GetTracks());
		}

		public KaraokeTrack AddTrack(KaraokeTrackType type)
		{
			var track = _file.AddTrack(type);
			return track;
		}

		public void UpdateMixer() => PlaybackState.UpdateMixer(_file.GetTracks());

		public void Save(string outFile)
		{
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

			var project = new KaraokeProject(file);
			project.Length = TimeSpan.FromSeconds(fileInfo.LengthSeconds);

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
			var project = new KaraokeProject(file);
			return project;
		}
	}
}
