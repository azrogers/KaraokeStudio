using Newtonsoft.Json;

namespace KaraokeStudio.Util
{
	internal static class FileCache
	{
		private const string MANIFEST_FILENAME = "_manifest.json";

		private static string CacheDir => Path.Combine(Path.GetTempPath(), "KaraokeStudio");
		private static string CacheManifestPath => Path.Combine(CacheDir, MANIFEST_FILENAME);
		private static Dictionary<string, DateTime> _manifest;

		public static string Get(ICacheRequest request)
		{
			var localPath = Path.Combine(request.Category, Utility.Sha256Hash(request.Key));
			var categoryDir = Path.Combine(CacheDir, request.Category);
			if (!Directory.Exists(categoryDir))
			{
				Directory.CreateDirectory(categoryDir);
			}

			var path = GetTempFilePath(localPath);
			if (!File.Exists(path))
			{
				using (var output = File.OpenWrite(path))
				{
					request.Create(output);
				}
			}

			return path;
		}

		private static string GetTempFilePath(string filename)
		{
			UpdateFileUsed(filename);
			return Path.Combine(CacheDir, filename);
		}

		private static void UpdateFileUsed(string filename)
		{
			_manifest[filename] = DateTime.Now.AddDays(30);
			File.WriteAllText(CacheManifestPath, JsonConvert.SerializeObject(_manifest));
		}

		static FileCache()
		{
			if (!Directory.Exists(CacheDir))
			{
				Directory.CreateDirectory(CacheDir);
			}

			_manifest = LoadManifest();
			var files = Directory.EnumerateFiles(CacheDir).Where(f => Path.GetFileName(f) != MANIFEST_FILENAME).Select(f => Path.GetFileName(f));
			var toDelete = PruneCache(files);
			foreach (var f in toDelete)
			{
				File.Delete(Path.Combine(CacheDir, f));
				_manifest.Remove(f);
			}
		}

		private static Dictionary<string, DateTime> LoadManifest()
		{
			if (!File.Exists(CacheManifestPath))
			{
				return new Dictionary<string, DateTime>();
			}

			var manifest = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(CacheManifestPath));
			if (manifest == null)
			{
				File.Delete(CacheManifestPath);
				return new Dictionary<string, DateTime>();
			}

			return manifest;
		}

		// returns the files that should be deleted
		private static IEnumerable<string> PruneCache(IEnumerable<string> filesInFolder)
		{
			if (!File.Exists(CacheManifestPath))
			{
				// no manifest, so assume it's all expired
				return filesInFolder;
			}

			var manifest = JsonConvert.DeserializeObject<Dictionary<string, DateTime>>(File.ReadAllText(CacheManifestPath));
			if (manifest == null)
			{
				// manifest is invalid, so delete it and assume it's all expired
				File.Delete(CacheManifestPath);
				return filesInFolder;
			}

			return filesInFolder.Where(f => !manifest.ContainsKey(f) || manifest[f] < DateTime.Now);
		}
	}

	internal interface ICacheRequest
	{
		string Key { get; }
		string Category { get; }

		void Create(Stream output);
	}
}
