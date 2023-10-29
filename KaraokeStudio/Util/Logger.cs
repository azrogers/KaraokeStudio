using System.Diagnostics;

namespace KaraokeStudio.Util
{
	internal static class Logger
	{
		public static Form? ParentForm { get; set; } = null;

		public static void ShowError(UserException ex)
		{
#if DEBUG
			if (Debugger.IsAttached)
			{
				Debugger.Break();
			}
#endif

			Console.WriteLine(ex.Message);
			MessageBox.Show(ParentForm, ex.FriendlyMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}
}
