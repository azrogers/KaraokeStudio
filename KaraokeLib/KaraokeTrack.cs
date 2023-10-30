using KaraokeLib.Events;
using KaraokeLib.Files;

namespace KaraokeLib
{
	/// <summary>
	/// A single track within a KaraokeFile. 
	/// A track can be more than just text - see <see cref="KaraokeTrackType"/>.
	/// </summary>
	public class KaraokeTrack
	{
		private static readonly Dictionary<KaraokeTrackType, HashSet<KaraokeEventType>> ValidEvents = new Dictionary<KaraokeTrackType, HashSet<KaraokeEventType>>()
		{
			{ KaraokeTrackType.Lyrics, new HashSet<KaraokeEventType>() { KaraokeEventType.Lyric, KaraokeEventType.ParagraphBreak, KaraokeEventType.LineBreak } },
			{ KaraokeTrackType.Audio, new HashSet<KaraokeEventType>() { KaraokeEventType.AudioClip } },
			{ KaraokeTrackType.Graphics, new HashSet<KaraokeEventType>() { } }
		};

		private List<KaraokeEvent> _events;

		/// <summary>
		/// The events on this track.
		/// </summary>
		public List<KaraokeEvent> Events => _events;

		/// <summary>
		/// The type of this track.
		/// </summary>
		public KaraokeTrackType Type { get; private set; }

		/// <summary>
		/// The id used to uniquely identify this track.
		/// </summary>
		public int Id { get; internal set; }

		private IKaraokeFile? _karaokeFile;

		internal KaraokeTrack(int id, KaraokeTrackType type)
		{
			_events = new List<KaraokeEvent>();
			Type = type;
			Id = id;
		}

		internal KaraokeTrack(int id, KaraokeTrackType type, IEnumerable<KaraokeEvent> events)
		{
			_events = events.ToList();
			Type = type;
			Id = id;
		}

		public KaraokeTrack(IKaraokeFile file, KaraokeTrackType type)
		{
			_karaokeFile = file;
			_events = new List<KaraokeEvent>();
			Type = type;
			Id = _karaokeFile.IdTracker.AddNewTrack(this);
		}

		public KaraokeTrack(IKaraokeFile file, KaraokeTrackType type, IEnumerable<KaraokeEvent> events)
		{
			_karaokeFile = file;
			_events = new List<KaraokeEvent>(events);
			Type = type;
			Id = _karaokeFile.IdTracker.AddNewTrack(this);
		}

		/// <summary>
		/// Adds a new event to this track and returns the event.
		/// </summary>
		/// <param name="type"></param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <returns></returns>
		public KaraokeEvent AddEvent(KaraokeEventType type, IEventTimecode start, IEventTimecode end, int linkedId = -1)
		{
			if(_karaokeFile == null)
			{
				throw new InvalidOperationException("KaraokeTrack missing KaraokeFile");
			}

			return new KaraokeEvent(type, _karaokeFile.IdTracker, start, end, linkedId);
		}

		/// <summary>
		/// Adds a new event to this track of type KaraokeEventType.AudioClip.
		/// </summary>
		public AudioClipKaraokeEvent AddAudioClipEvent(AudioClipSettings settings, IEventTimecode start, IEventTimecode end) => 
			new AudioClipKaraokeEvent(AddEvent(KaraokeEventType.AudioClip, start, end));

		/// <summary>
		/// Adds the given events to this track.
		/// </summary>
		/// <remarks>
		/// This will automatically conform events so they don't overlap, and check that their types match the track type they're on.
		/// </remarks>
		public void AddEvents(IEnumerable<KaraokeEvent> events)
		{
			if (_karaokeFile == null)
			{
				throw new InvalidOperationException("KaraokeTrack missing KaraokeFile");
			}

			_events.AddRange(events);
			ValidateEvents();
			ConformEvents();
			_karaokeFile.IdTracker.AddEvents(Id, events);
		}

		/// <summary>
		/// Removes the existing events on this track and adds the given events.
		/// </summary>
		public void ReplaceEvents(IEnumerable<KaraokeEvent> events)
		{
			if(_karaokeFile == null)
			{
				throw new InvalidOperationException("KaraokeTrack missing KaraokeFile");
			}

			_events.Clear();
			_karaokeFile.IdTracker.ReplaceTrack(Id, this);
			AddEvents(events);
		}

		/// <summary>
		/// Returns events within the given bounds.
		/// </summary>
		public IEnumerable<KaraokeEvent> GetRelevantEvents((double Start, double End) bounds)
		{
			foreach (var ev in _events)
			{
				if (
					(ev.StartTimeSeconds >= bounds.Start && ev.StartTimeSeconds < bounds.End) ||
					(ev.EndTimeSeconds >= bounds.Start && ev.EndTimeSeconds < bounds.End) ||
					(bounds.Start >= ev.StartTimeSeconds && bounds.Start < ev.EndTimeSeconds) ||
					(bounds.End >= ev.StartTimeSeconds && bounds.End < ev.EndTimeSeconds))
				{
					yield return ev;
				}
			}
		}

		internal void SetFile(IKaraokeFile file)
		{
			_karaokeFile = file;
		}

		private void ConformEvents()
		{
			_events = _events.OrderBy(ev => ev.StartTimeMilliseconds).ToList();
			if (_events.Count < 2)
			{
				// nothing to do
				return;
			}

			// fix overlapping events
			for (var i = 1; i < _events.Count; i++)
			{
				if (_events[i].StartTimeSeconds < _events[i - 1].EndTimeSeconds)
				{
					_events[i - 1].SetTiming(_events[i - 1].StartTime, new TimeSpanTimecode(_events[i].StartTimeSeconds));
				}
			}
		}

		private void ValidateEvents()
		{
			var validEvents = ValidEvents[Type];
			if (validEvents == null)
			{
				throw new NotSupportedException($"Missing valid events for {Type}");
			}

			foreach (var ev in _events)
			{
				if (!validEvents.Contains(ev.Type))
				{
					throw new InvalidDataException($"Can't have event of type {ev.Type} on track of type {Type}!");
				}
			}
		}
	}

	public enum KaraokeTrackType : byte
	{
		Lyrics = 0x00,
		Graphics = 0x01,
		Audio = 0x02
	}
}
