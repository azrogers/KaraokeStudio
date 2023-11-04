using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio
{
	public static class UndoHandler
	{
		private static Stack<UndoFrame> _undoActions = new Stack<UndoFrame>();

		public static UndoFrame? CurrentItem => _undoActions.Any() ? _undoActions.Peek() : null;

		public static event Action? OnUndoItemsChanged;

		public static void Clear()
		{
			_undoActions.Clear();
			OnUndoItemsChanged?.Invoke();
		}

		public static void Push(string action, Action onUndo)
		{
			_undoActions.Push(new UndoFrame() { Action = action, OnUndo = onUndo });
			OnUndoItemsChanged?.Invoke();
		}

		public static void Undo()
		{
			_undoActions.Pop().OnUndo();
			OnUndoItemsChanged?.Invoke();
		}

		public struct UndoFrame
		{
			public string Action;
			public Action OnUndo;
		}
	}
}
