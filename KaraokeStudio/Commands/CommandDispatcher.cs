using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;

namespace KaraokeStudio.Commands
{
	internal static class CommandDispatcher
	{
		public static CommandContext CurrentContext = new CommandContext();

		public static void Dispatch(ICommand command)
		{
			foreach(var update in command.Execute(CurrentContext))
			{
				UpdateDispatcher.Dispatch(update);
			}

			UndoHandler.Push(command.Description, () =>
			{
				foreach(var update in command.Undo(CurrentContext))
				{
					UpdateDispatcher.Dispatch(update);
				}
			});
		}
	}
}
