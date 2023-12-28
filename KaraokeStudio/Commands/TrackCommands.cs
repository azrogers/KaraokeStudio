using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;

namespace KaraokeStudio.Commands
{
	internal abstract class AddTrackCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public abstract string Description { get; }

		public bool CanUndo => true;

		private Action<KaraokeTrack> _onCreate;
		private KaraokeTrackType _type;
		private int? _createdTrackId = null;

		public AddTrackCommand(KaraokeTrackType type, Action<KaraokeTrack> onCreate)
		{
			_type = type;
			_onCreate = onCreate;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			if(context.Project == null)
			{
				Logger.Warn($"Attempted to add a track to non-existent project, giving up");
				yield break;
			}

			var track = context.Project.AddTrack(_type);
			_onCreate(track);
			_createdTrackId = track.Id;

			yield return new TracksUpdate(track.Id);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			if (context.Project == null)
			{
				Logger.Warn($"Attempted to add a track to non-existent project, giving up");
				yield break;
			}

			if(_createdTrackId == null)
			{
				Logger.Warn($"Tried to undo a create track action without a created track ID, giving up");
				yield break;
			}

			context.Project.RemoveTrack(_createdTrackId.Value);

			yield return new TracksUpdate(_createdTrackId.Value);
		}
	}

	internal class AddAudioTrackCommand : AddTrackCommand
	{
		public override string Description => "Add audio track";

		public AddAudioTrackCommand(AudioClipSettings settings, IEventTimecode start, IEventTimecode end)
			: base(KaraokeTrackType.Audio, track => track.AddAudioClipEvent(settings, start, end))
		{

		}
	}

	internal class SetEventTimingsCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		private int _trackId;
		private Dictionary<int, KaraokeEvent> _newEvents = new Dictionary<int, KaraokeEvent>();
		private Dictionary<int, KaraokeEvent> _oldEvents = new Dictionary<int, KaraokeEvent>();

		public string Description => "Set event timings";
		public bool CanUndo => true;

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
