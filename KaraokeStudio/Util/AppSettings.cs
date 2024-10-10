using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using Newtonsoft.Json;

namespace KaraokeStudio.Util
{
	internal class AppSettings : EditableConfig<AppSettings>
	{
		private const int MAX_RECENT_FILES = 5;

		public static readonly AppSettings Instance =
			File.Exists(Filename) ?
				JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Filename)) ?? new AppSettings() :
				new AppSettings();

		private static string Filename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KaraokeStudio", "settings.json");

		/// <summary>
		/// Called when AppSettings.Volume is changed.
		/// </summary>
		public event Action<float>? OnVolumeChanged;

		[JsonProperty]
		[ConfigHide]
		public HashSet<string> RecentFiles { get; private set; } = new HashSet<string>();

		[JsonProperty]
		[ConfigHide]
		public float Volume { get; private set; } = 1.0f;

		[JsonProperty]
		[ConfigHide]
		public float PlaybackRate { get; private set; } = 1.0f;

		[JsonProperty]
		[ConfigHide]
		public AudioSettings AudioSettings { get; private set; } = new AudioSettings();

		public void AddRecentFile(string file)
		{
			RecentFiles.Add(file);
			if (RecentFiles.Count > MAX_RECENT_FILES)
			{
				RecentFiles.Remove(RecentFiles.First());
			}

			Save();
		}

		public void SetVolume(float vol)
		{
			Volume = vol;
			Save();
			OnVolumeChanged?.Invoke(vol);
		}

		public void SetPlaybackRate(float rate)
		{
			PlaybackRate = rate;
			Save();
		}

		public void SetAudioSettings(AudioSettings settings)
		{
			AudioSettings = settings;
			Save();
		}

		public void Save()
		{
			var dir = Path.GetDirectoryName(Filename);
			if (dir != null && !Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			File.WriteAllText(Filename, JsonConvert.SerializeObject(this));
		}
	}
}
