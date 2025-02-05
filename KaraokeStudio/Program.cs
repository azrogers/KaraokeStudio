using KaraokeLib.Video;
using KaraokeStudio.Managers;
using NLog;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace KaraokeStudio
{
	internal static class Program
	{
		/// <summary>
		///  The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			FFMpegUtil.SetupFfmpegPath();

			// To customize application configuration such as set high DPI settings or default font,
			// see https://aka.ms/applicationconfiguration.
			ApplicationConfiguration.Initialize();

			var fontCollection = CreateFontCollection(Properties.Resources.OpenSans);
			var family = new FontFamily("Open Sans", fontCollection);
			Application.SetDefaultFont(new Font(family, 9f));

			var config = new NLog.Config.LoggingConfiguration();
			var logConsole = new NLog.Targets.ConsoleTarget("Console");
			config.AddRule(LogLevel.Info, LogLevel.Fatal, logConsole);

			var logMethodCall = new NLog.Targets.MethodCallTarget("MethodCall", ConsoleForm.LogEvent);
			config.AddRule(LogLevel.Info, LogLevel.Fatal, logMethodCall);

			NLog.LogManager.Configuration = config;

			AudioManager.Instance.Initialize();

			var form = new MainForm();
			if (args.Length > 0)
			{
				form.LoadProject(args[0]);
			}

			KeyboardManager.Instance.Initialize(form);

			Application.Run(form);

			KeyboardManager.Instance.Cleanup();
		}

		private static PrivateFontCollection CreateFontCollection(byte[] file)
		{
			var collection = new PrivateFontCollection();
			var handle = GCHandle.Alloc(file, GCHandleType.Pinned);
			var ptr = handle.AddrOfPinnedObject();
			try
			{
				collection.AddMemoryFont(ptr, file.Length);
			}
			finally
			{
				handle.Free();
			}

			return collection;
		}
	}
}