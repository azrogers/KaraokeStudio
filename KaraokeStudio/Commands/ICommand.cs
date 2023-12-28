using KaraokeStudio.Commands.Updates;

namespace KaraokeStudio.Commands
{
    internal interface ICommand
	{
		IEnumerable<IUpdate> Execute(CommandContext context);
		IEnumerable<IUpdate> Undo(CommandContext context);
		string Description { get; }
	}
}
