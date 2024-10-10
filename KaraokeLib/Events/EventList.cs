using System.Collections;

namespace KaraokeLib.Events
{
	/// <summary>
	/// A list of events sorted by their start times.
	/// </summary>
	public class EventList : IList<KaraokeEvent>
	{
		private SortedList<ulong, KaraokeEvent> _events = new SortedList<ulong, KaraokeEvent>();

		/// <inheritdoc/>
		public KaraokeEvent this[int index] { get => _events.GetValueAtIndex(index); set => _events.SetValueAtIndex(index, value); }

		/// <inheritdoc/>
		public int Count => _events.Count;

		/// <inheritdoc/>
		public bool IsReadOnly => false;

		/// <inheritdoc/>
		public void Add(KaraokeEvent item)
		{
			_events.Add(item.StartTimeMilliseconds, item);
		}

		/// <summary>
		/// Adds items from an IEnumerable to this EventList.
		/// </summary>
		public void AddRange(IEnumerable<KaraokeEvent> items)
		{
			foreach (var item in items)
			{
				_events.Add(item.StartTimeMilliseconds, item);
			}
		}

		/// <inheritdoc/>
		public void Clear()
		{
			_events.Clear();
		}

		/// <inheritdoc/>
		public bool Contains(KaraokeEvent item)
		{
			return _events.ContainsValue(item);
		}

		/// <inheritdoc/>
		public void CopyTo(KaraokeEvent[] array, int arrayIndex)
		{
			_events.Values.CopyTo(array, arrayIndex);
		}

		/// <inheritdoc/>
		public IEnumerator<KaraokeEvent> GetEnumerator()
		{
			return _events.Select(kv => kv.Value).GetEnumerator();
		}

		/// <inheritdoc/>
		public int IndexOf(KaraokeEvent item)
		{
			return _events.IndexOfValue(item);
		}

		/// <inheritdoc/>
		public void Insert(int index, KaraokeEvent item)
		{
			_events.Add(item.StartTimeMilliseconds, item);
		}

		/// <inheritdoc/>
		public bool Remove(KaraokeEvent item)
		{
			var index = _events.IndexOfValue(item);
			if (index != -1)
			{
				_events.RemoveAt(index);
			}

			return index != -1;
		}

		/// <inheritdoc/>
		public void RemoveAt(int index)
		{
			_events.RemoveAt(index);
		}

		/// <inheritdoc/>
		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}
	}
}
