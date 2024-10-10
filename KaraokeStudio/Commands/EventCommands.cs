using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;

namespace KaraokeStudio.Commands
{
	internal class SetEventTimingsCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private int[] _trackIds;
		private string _eventString;
		private Dictionary<int, (double Start, double End)> _newEventTimings = new Dictionary<int, (double Start, double End)>();
		private Dictionary<int, (double Start, double End)> _oldEventTimings = new Dictionary<int, (double Start, double End)>();

		public string Description => _eventString;
		public bool CanUndo => true;

		public SetEventTimingsCommand(KaraokeTrack[] tracks, KaraokeEvent[] events, string eventString = "Set event timings")
		{
			_trackIds = tracks.Select(t => t.Id).ToArray();
			var eventIdLookup = new HashSet<int>(events.Select(ev => ev.Id));

			foreach (var track in tracks)
			{
				foreach (var ev in track.Events)
				{
					if (eventIdLookup.Contains(ev.Id))
					{
						_oldEventTimings[ev.Id] = (ev.StartTimeSeconds, ev.EndTimeSeconds);
					}
				}
			}

			foreach (var ev in events)
			{
				_newEventTimings[ev.Id] = (ev.StartTimeSeconds, ev.EndTimeSeconds);
			}

			_eventString = eventString;
		}

		public SetEventTimingsCommand(KaraokeTrack[] tracks, Dictionary<int, (double Start, double End)> newEventTimings, string eventString = "Set event timings")
		{
			_trackIds = tracks.Select(t => t.Id).ToArray();

			foreach (var track in tracks)
			{
				foreach (var ev in track.Events)
				{
					if (newEventTimings.ContainsKey(ev.Id))
					{
						_oldEventTimings[ev.Id] = (ev.StartTimeSeconds, ev.EndTimeSeconds);
					}
				}
			}

			_newEventTimings = newEventTimings;
			_eventString = eventString;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			foreach (var trackId in _trackIds)
			{
				var track = context.Project?.Tracks.Where(t => t.Id == trackId).FirstOrDefault();
				if (track == null)
				{
					Logger.Warn($"Can't find track ID {trackId}, giving up");
					yield break;
				}

				foreach (var ev in track.Events)
				{
					if (_newEventTimings.ContainsKey(ev.Id))
					{
						ev.SetTiming(new TimeSpanTimecode(_newEventTimings[ev.Id].Start), new TimeSpanTimecode(_newEventTimings[ev.Id].End));
					}
				}
			}

			yield return new EventsUpdate(_newEventTimings.Keys.ToArray(), EventsUpdate.UpdateType.Timing);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			foreach (var trackId in _trackIds)
			{
				var track = context.Project?.Tracks.Where(t => t.Id == trackId).FirstOrDefault();
				if (track == null)
				{
					Logger.Warn($"Can't find track ID {trackId}, giving up");
					yield break;
				}

				foreach (var ev in track.Events)
				{
					if (_newEventTimings.ContainsKey(ev.Id))
					{
						ev.SetTiming(new TimeSpanTimecode(_oldEventTimings[ev.Id].Start), new TimeSpanTimecode(_oldEventTimings[ev.Id].End));
					}
				}
			}

			yield return new EventsUpdate(_newEventTimings.Keys.ToArray(), EventsUpdate.UpdateType.Timing);
		}
	}

	internal class AddAudioClipEventCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public string Description => "Add audio clip event";

		public bool CanUndo => true;

		private int _trackId;
		private AudioClipSettings _settings;
		private int _createdEventId = -1;
		private IEventTimecode _start;
		private IEventTimecode _end;

		public AddAudioClipEventCommand(KaraokeTrack track, AudioClipSettings clipSettings, IEventTimecode start, IEventTimecode end)
		{
			_trackId = track.Id;
			_settings = clipSettings;
			_start = start;
			_end = end;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == _trackId);
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			var ev = track.AddAudioClipEvent(_settings, _start, _end);
			_createdEventId = ev.Id;
			yield return new EventsUpdate(new int[] { _createdEventId }, EventsUpdate.UpdateType.Add);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == _trackId);
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			if (_createdEventId == -1)
			{
				Logger.Warn("Tried to undo create audio clip event command, but _createdEventId was -1?");
				yield break;
			}

			track.ReplaceEvents(track.Events.Where(e => e.Id != _createdEventId));
			yield return new EventsUpdate(new int[] { _createdEventId }, EventsUpdate.UpdateType.Remove);
		}
	}

	internal class RemoveEventsCommand : ICommand
	{
		public string Description => _eventIdsToRemove.Length == 1 ? "Remove event" : "Remove events";

		public bool CanUndo => true;

		private int[] _eventIdsToRemove;
		private int[] _selectedIds = new int[0];
		private Dictionary<int, KaraokeEvent[]> _removedEvents = new Dictionary<int, KaraokeEvent[]>();

		public RemoveEventsCommand(int[] eventIds)
		{
			_eventIdsToRemove = eventIds;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			if (context.Project == null)
			{
				// nothing to do
				yield break;
			}

			var idsLookup = new HashSet<int>(_eventIdsToRemove);

			_removedEvents.Clear();
			_selectedIds = SelectionManager.SelectedEvents.Select(ev => ev.Id).Where(id => idsLookup.Contains(id)).ToArray();

			foreach (var track in context.Project.Tracks)
			{
				_removedEvents[track.Id] = track.Events.Where(ev => idsLookup.Contains(ev.Id)).ToArray();
				track.ReplaceEvents(track.Events.Where(ev => !idsLookup.Contains(ev.Id)).ToArray());
			}

			yield return new EventsUpdate(_eventIdsToRemove, EventsUpdate.UpdateType.Remove);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			if (context.Project == null)
			{
				yield break;
			}

			foreach (var track in context.Project.Tracks)
			{
				if (!_removedEvents.ContainsKey(track.Id))
				{
					continue;
				}

				track.AddEvents(_removedEvents[track.Id]);
			}

			var selectedIdsLookup = new HashSet<int>(_selectedIds);
			var selectedEvents = _removedEvents.Values.SelectMany(ev => ev).Where(ev => selectedIdsLookup.Contains(ev.Id));
			SelectionManager.Select(selectedEvents, false);

			yield return new EventsUpdate(_eventIdsToRemove, EventsUpdate.UpdateType.Add);
		}
	}
}
