using KaraokeLib.Config;
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
			if (context.Project == null)
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

			if (_createdTrackId == null)
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

	internal class SetTrackSettingsCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public string Description => _actionString;

		public bool CanUndo => true;

		private IEditableConfig? _oldConfig = null;
		private IEditableConfig _newConfig;
		private string _actionString;
		private int _trackId;

		public SetTrackSettingsCommand(KaraokeTrack track, IEditableConfig config, string actionString = "Change track properties")
		{
			_trackId = track.Id;
			_newConfig = config;
			_actionString = actionString;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == _trackId);
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			_oldConfig = track.GetTrackConfig().Copy();
			track.SetTrackConfig(_newConfig);
			yield return new TrackSettingsUpdate(track.Id, _newConfig);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == _trackId);
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			if (_oldConfig == null)
			{
				Logger.Warn("Attempted to undo a track settings change but _oldConfig was null, giving up");
				yield break;
			}

			track.SetTrackConfig(_oldConfig);
			yield return new TrackSettingsUpdate(track.Id, _oldConfig);
		}
	}
}
