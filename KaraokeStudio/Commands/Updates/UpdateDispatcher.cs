namespace KaraokeStudio.Commands.Updates
{
	internal static class UpdateDispatcher
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private static Dictionary<Type, List<Action<IUpdate>>> _updateHandlers = new Dictionary<Type, List<Action<IUpdate>>>();

		public static void Dispatch<T>(T update) where T: IUpdate
		{
			var type = update.GetType();
			if(!_updateHandlers.ContainsKey(type))
			{
				return;
			}

			foreach (var handler in _updateHandlers[type])
			{
				handler(update);
			}
		}

		internal static Handle RegisterHandler<T>(Action<T> onUpdate) where T : IUpdate
		{
			if (!_updateHandlers.ContainsKey(typeof(T)))
			{
				_updateHandlers[typeof(T)] = new List<Action<IUpdate>>();
			}

			var handler = (IUpdate update) => onUpdate((T)update);
			_updateHandlers[typeof(T)].Add(handler);
			return new Handle(typeof(T), handler);
		}

		internal class Handle
		{
			public Type UpdateType;
			public Action<IUpdate> UpdateHandler;

			private bool _released = false;

			internal Handle(Type type, Action<IUpdate> updateHandler)
			{
				UpdateType = type;
				UpdateHandler = updateHandler;
			}

			public void Release()
			{
				if (_released)
				{
					Logger.Warn("Attempted to release already-released handle");
					return;
				}

				if (!_updateHandlers.ContainsKey(UpdateType))
				{
					Logger.Warn($"Attempted to release handle but no update type {UpdateType} handlers have been registered?");
					return;
				}

				_updateHandlers[UpdateType].Remove(UpdateHandler);
				_released = true;
			}
		}
	}
}
