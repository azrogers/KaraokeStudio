namespace KaraokeLib.Events
{
	/// <summary>
	/// A binary tree of events sorted by start time.
	/// Allows events to be found by position efficiently.
	/// </summary>
	internal class EventTree
	{
		(ulong, ulong) Range;
		int EventId;

		EventTree? Larger;
		EventTree? Smaller;

		uint Height;

		/// <summary>
		/// Create a new event tree from the given events.
		/// There must be at least one event in the array.
		/// </summary>
		public EventTree(KaraokeEvent[] events) : this(events.OrderBy(ev => ev.StartTimeMilliseconds).ToArray(), 0)
		{

		}

		public EventTree(EventTree other)
		{
			Range = other.Range;
			EventId = other.EventId;
			Smaller = other.Smaller;
			Larger = other.Larger;
		}

		private EventTree(KaraokeEvent[] events, uint height)
		{
			Height = height;

			Range = (
				events.Length > 0 ? events[0].StartTimeMilliseconds : 0,
				events.Length > 0 ? events[events.Length - 1].EndTimeMilliseconds : 0);

			EventId = -1;

			if (events.Length < 2)
			{
				EventId = events[0].Id;
				return;
			}

			var len = events.Length;
			var half = (int)(len / 2);

			var smallerEvs = new KaraokeEvent[half];
			Array.Copy(events, 0, smallerEvs, 0, half);

			var largerEvs = new KaraokeEvent[(len - half)];
			Array.Copy(events, half, largerEvs, 0, len - half);

			Smaller = new EventTree(smallerEvs, height + 1);
			Larger = new EventTree(largerEvs, height + 1);
		}

		/// <summary>
		/// Finds an event ID at the given position, if any.
		/// </summary>
		public int? Find(double time)
		{
			if (Smaller == null && Larger == null)
			{
				return EventId;
			}

			if (Smaller != null && time >= Smaller.Range.Item1 && time < Smaller.Range.Item2)
			{
				return Smaller.Find(time);
			}

			if (Larger != null && time >= Larger.Range.Item1 && time < Larger.Range.Item2)
			{
				return Larger.Find(time);
			}

			return null;
		}

		/// <summary>
		/// Adds the given event to the tree.
		/// </summary>
		/// <returns>The new range of the event after conforming.</returns>
		public (ulong, ulong) Add(KaraokeEvent e)
		{
			var (start, end) = (e.StartTimeMilliseconds, e.EndTimeMilliseconds);
			if (start < Range.Item1 && end >= Range.Item2)
			{
				// this range entirely encasulates the old range
				// this one should be in charge
				Smaller = new EventTree(this);
				Larger = null;
				EventId = e.Id;
				Range = (e.StartTimeMilliseconds, e.EndTimeMilliseconds);
				return Range;
			}

			// TODO
			throw new NotImplementedException();
		}
	}
}
