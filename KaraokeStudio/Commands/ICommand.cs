using KaraokeStudio.Commands.Updates;

namespace KaraokeStudio.Commands
{
    internal interface ICommand
	{
		IEnumerable<IUpdate> Execute(CommandContext context);
		IEnumerable<IUpdate> Undo(CommandContext context);
		string Description { get; }
		bool CanUndo { get; }
	}

	internal abstract class UndolessCommand : ICommand
	{
		public string Description => "";

		public bool CanUndo => false;

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			DoExecute(context);
			yield break;
		}

		public abstract void DoExecute(CommandContext context);

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			throw new NotImplementedException();
		}
	}
}
