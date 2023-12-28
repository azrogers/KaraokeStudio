using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;

namespace KaraokeStudio.Commands
{
	internal class SetEventTimingsCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private int _trackId;
		private Dictionary<int, KaraokeEvent> _newEvents = new Dictionary<int, KaraokeEvent>();
		private Dictionary<int, KaraokeEvent> _oldEvents = new Dictionary<int, KaraokeEvent>();

		public string Description => "Set event timings";

		public SetEventTimingsCommand(KaraokeTrack track, KaraokeEvent[] events)
		{
			_trackId = track.Id;

			foreach (var ev in track.Events)
			{
				_oldEvents[ev.Id] = ev;
			}

			foreach (var ev in events)
			{
				_newEvents[ev.Id] = ev;
			}
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			var track = context.Project?.Tracks.Where(t => t.Id == _trackId).FirstOrDefault();
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			foreach (var ev in track.Events)
			{
				if (_newEvents.ContainsKey(ev.Id))
				{
					ev.SetTiming(_newEvents[ev.Id].StartTime, _newEvents[ev.Id].EndTime);
				}
			}

			yield return new EventsUpdate(_newEvents.Keys.ToArray());
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			var track = context.Project?.Tracks.Where(t => t.Id == _trackId).FirstOrDefault();
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			foreach (var ev in track.Events)
			{
				if (_newEvents.ContainsKey(ev.Id))
				{
					ev.SetTiming(_oldEvents[ev.Id].StartTime, _oldEvents[ev.Id].EndTime);
				}
			}

			yield return new EventsUpdate(_newEvents.Keys.ToArray());
		}
	}
}
