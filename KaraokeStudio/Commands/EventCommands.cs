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
}
