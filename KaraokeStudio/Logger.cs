using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	internal static class Logger
	{
		public static Form? ParentForm { get; set; } = null;

		public static void ShowError(UserException ex)
		{
#if DEBUG
			if(Debugger.IsAttached)
			{
				Debugger.Break();
			}
#endif

			Console.WriteLine(ex.Message);
			MessageBox.Show(ParentForm, ex.FriendlyMessage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
		}
	}

	public class UserException : Exception
	{
		public string FriendlyMessage { get; private set; }

		public UserException(Exception ex)
			: base(ex.Message, ex)
		{
			FriendlyMessage = ex.Message;
		}

		public UserException(string message)
			: this(message, message)
		{

		}

		public UserException(string message, string friendlyMessage)
			: base(message)
		{
			FriendlyMessage = friendlyMessage;
		}
	}
}
