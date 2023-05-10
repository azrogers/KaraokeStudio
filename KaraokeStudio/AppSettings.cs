using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal class AppSettings
	{
		private const int MAX_RECENT_FILES = 5;

		public static readonly AppSettings Instance =
			File.Exists(Filename) ?
				JsonConvert.DeserializeObject<AppSettings>(File.ReadAllText(Filename)) ?? new AppSettings() :
				new AppSettings();

		private static string Filename => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KaraokeStudio", "settings.json");

		public IEnumerable<string> RecentFiles => _recentFiles;

		private HashSet<string> _recentFiles = new HashSet<string>();

		public void AddRecentFile(string file)
		{
			_recentFiles.Add(file);
			if(_recentFiles.Count > MAX_RECENT_FILES)
			{
				_recentFiles.Remove(_recentFiles.First());
			}

			Save();
		}

		public void Save()
		{
			var dir = Path.GetDirectoryName(Filename);
			if(!Directory.Exists(dir))
			{
				Directory.CreateDirectory(dir);
			}
			File.WriteAllText(Filename, JsonConvert.SerializeObject(this));
		}
	}
}
