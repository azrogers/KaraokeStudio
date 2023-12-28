using KaraokeLib.Events;
using KaraokeLib.Tracks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Files
{
    /// <summary>
    /// Keeps track of IDs for events and tracks to make sure they don't overlap.
    /// This is necessary as event IDs are per-file unique, not per-track unique.
    /// </summary>
    public class FileIdTracker
	{
		private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private Dictionary<int, KaraokeTrack> _tracks = new Dictionary<int, KaraokeTrack>();
		private Dictionary<int, KaraokeEvent> _events = new Dictionary<int, KaraokeEvent>();
		private Dictionary<int, HashSet<int>> _trackIdToEventIdMap = new Dictionary<int, HashSet<int>>();
		private int _nextTrackId = 0;
		private int _nextEventId = 0;

		/// <summary>
		/// Adds the given track to the tracker and returns the new ID it should have.
		/// </summary>
		internal int AddNewTrack(KaraokeTrack track)
		{
			_tracks[_nextTrackId] = track;
			return _nextTrackId++;
		}

		internal int AddNewEvent(KaraokeEvent ev)
		{
			_events[_nextEventId] = ev;
			return _nextEventId++;
		}
		
		/// <summary>
		/// Adds the given events to the tracker, reassigning IDs if necessary.
		/// </summary>
		internal void AddEvents(int trackId, IEnumerable<KaraokeEvent> events)
		{
			var eventsNeedNewId = new List<KaraokeEvent>();
			foreach (var ev in events)
			{
				if (_events.ContainsKey(ev.Id))
				{
					eventsNeedNewId.Add(ev);
				}
				else
				{
					_events[ev.Id] = ev;
				}
			}

			foreach (var ev in eventsNeedNewId)
			{
				Logger.Debug($"Event with ID {ev.Id} collides with existing event ID - reassigning to ID {_nextEventId}");
				// remap linked ids
				foreach(var e in events)
				{
					if(e.LinkedId == ev.Id)
					{
						e.LinkedId = _nextEventId;
					}
				}
				ev.Id = _nextEventId++;
				_events[ev.Id] = ev;
			}

			var evIds = new HashSet<int>();
			foreach (var ev in events)
			{
				evIds.Add(ev.Id);
			}

			_trackIdToEventIdMap[trackId] = evIds;
		}

		/// <summary>
		/// Replaces a track and all of its events with a new track and set of events.
		/// </summary>
		internal void ReplaceTrack(int trackId, KaraokeTrack newTrack)
		{
			if(_trackIdToEventIdMap.ContainsKey(trackId))
			{
				foreach(var eventId in _trackIdToEventIdMap[trackId])
				{
					_events.Remove(eventId);
				}

				_trackIdToEventIdMap.Remove(trackId);
			}

			_tracks[trackId] = newTrack;
			AddEvents(trackId, newTrack.Events);
		}

		/// <summary>
		/// Builds the internal maps of tracks and events for future ID assignments and lookups.
		/// </summary>
		internal void BuildMaps(IEnumerable<KaraokeTrack> tracks)
		{
			_trackIdToEventIdMap.Clear();
			_tracks.Clear();
			_events.Clear();

			var tracksNeedNewId = new List<KaraokeTrack>();
			var eventsNeedNewId = new List<KaraokeEvent>();

			foreach(var track in tracks)
			{
				if(_tracks.ContainsKey(track.Id))
				{
					tracksNeedNewId.Add(track);
				}
				else
				{
					_tracks[track.Id] = track;
				}

				var events = new HashSet<int>();

				foreach(var ev in track.Events)
				{
					if(_events.ContainsKey(ev.Id))
					{
						eventsNeedNewId.Add(ev);
					}
					else
					{
						_events[ev.Id] = ev;
					}
				}
			}

			_nextTrackId = _tracks.Any() ? _tracks.Keys.Max() + 1 : 0;
			_nextEventId = _events.Any() ? _events.Keys.Max() + 1 : 0;

			foreach(var track in tracksNeedNewId)
			{
				Logger.Warn($"Track with ID {track.Id} collides with existing track ID - reassigning to ID {_nextTrackId}");
				track.Id = _nextTrackId++;
				_tracks[track.Id] = track;
			}

			foreach(var ev in eventsNeedNewId)
			{
				Logger.Warn($"Event with ID {ev.Id} collides with existing event ID - reassigning to ID {_nextEventId}");
				ev.Id = _nextEventId++;
				_events[ev.Id] = ev;
			}

			// build map for looking up event IDs from track IDs
			foreach(var track in _tracks.Values)
			{
				var events = new HashSet<int>();

				foreach(var ev in track.Events)
				{
					events.Add(ev.Id);
				}

				_trackIdToEventIdMap[track.Id] = events;
			}
		}
	}
}
