namespace KaraokeStudio.Commands.Updates
{
	internal class DeleteKeyUpdate : IUpdate { }
	internal class ArrowKeyUpdate : IUpdate
	{
		public bool IsLeft;

		public ArrowKeyUpdate(bool isLeft)
		{
			IsLeft = isLeft;
		}
	}
}
