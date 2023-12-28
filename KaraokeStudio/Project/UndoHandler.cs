namespace KaraokeStudio.Project
{
	public static class UndoHandler
	{
		private static Stack<UndoFrame> _undoActions = new Stack<UndoFrame>();
		private static Stack<UndoFrame> _redoActions = new Stack<UndoFrame>();

		public static UndoFrame? CurrentItem => _undoActions.Any() ? _undoActions.Peek() : null;
		public static UndoFrame? CurrentRedoItem => _redoActions.Any() ? _redoActions.Peek() : null;

		public static event Action? OnUndoItemsChanged;

		public static void Clear()
		{
			_undoActions.Clear();
			_redoActions.Clear();
			OnUndoItemsChanged?.Invoke();
		}

		public static void Push(string action, Action onUndo, Action onRedo)
		{
			_undoActions.Push(new UndoFrame() { Action = action, OnUndo = onUndo, OnRedo = onRedo });
			_redoActions.Clear();
			OnUndoItemsChanged?.Invoke();
		}

		public static void Undo()
		{
			var frame = _undoActions.Pop();
			frame.OnUndo();
			_redoActions.Push(frame);
			OnUndoItemsChanged?.Invoke();
		}

		public static void Redo()
		{
			var frame = _redoActions.Pop();
			frame.OnRedo();
			_undoActions.Push(frame);
			OnUndoItemsChanged?.Invoke();
		}

		public struct UndoFrame
		{
			public string Action;
			public Action OnUndo;
			public Action OnRedo;
		}
	}
}
