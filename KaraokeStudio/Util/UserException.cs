namespace KaraokeStudio.Util
{
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
