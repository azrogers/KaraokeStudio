namespace KaraokeLib.Events
{
	public interface IEventTimecode : IComparable<IEventTimecode>
	{
		double GetTimeSeconds();
		ulong GetTimeMilliseconds();
	}

	/// <summary>
	/// Basic implementation of IEventTimecode.
	/// </summary>
	public struct TimeSpanTimecode : IEventTimecode
	{
		private TimeSpan _span;

		public TimeSpanTimecode(TimeSpan span)
		{
			_span = span;
		}

		public TimeSpanTimecode(double seconds) => _span = TimeSpan.FromSeconds(seconds);

		public TimeSpanTimecode(uint milliseconds)
		{
			_span = TimeSpan.FromMilliseconds(milliseconds);
		}

		public double GetTimeSeconds() => _span.TotalSeconds;

		public ulong GetTimeMilliseconds() => (ulong)(_span.TotalMicroseconds / 1000L);

		public int CompareTo(IEventTimecode? other)
		{
			if (other == null)
			{
				return 1;
			}

			return GetTimeMilliseconds().CompareTo(other.GetTimeMilliseconds());
		}
	}
}
