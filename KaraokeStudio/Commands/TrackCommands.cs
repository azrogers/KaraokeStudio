﻿using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands.Updates;
using NLog;

namespace KaraokeStudio.Commands
{
	internal class AddTrackCommand : ICommand
	{
		private static Logger Logger = LogManager.GetCurrentClassLogger();

		public string Description => _description;

		public bool CanUndo => true;

		private Action<KaraokeTrack>? _onCreate;
		private KaraokeTrackType _type;
		private int? _createdTrackId = null;
		private string _description;

		public AddTrackCommand(KaraokeTrackType type, string description, Action<KaraokeTrack>? onCreate)
		{
			_type = type;
			_onCreate = onCreate;
			_description = description;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			if (context.Project == null)
			{
				Logger.Warn($"Attempted to add a track to non-existent project, giving up");
				yield break;
			}

			var track = context.Project.AddTrack(_type);
			_onCreate?.Invoke(track);
			_createdTrackId = track.Id;

			yield return new TracksUpdate(track.Id, TracksUpdate.UpdateType.Added);
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

			yield return new TracksUpdate(_createdTrackId.Value, TracksUpdate.UpdateType.Removed);
		}
	}

	internal class AddAudioTrackCommand : AddTrackCommand
	{
		public AddAudioTrackCommand(AudioClipSettings settings, IEventTimecode start, IEventTimecode end)
			: base(KaraokeTrackType.Audio, "Add audio track", track => track.AddAudioClipEvent(settings, start, end))
		{

		}
	}

	internal class RemoveTracksCommand : ICommand
	{
		private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();

		public string Description => _trackIds.Length > 1 ? "Remove tracks" : "Remove track";

		public bool CanUndo => true;

		private int[] _trackIds;
		private Dictionary<int, KaraokeTrack> _oldTracks = new Dictionary<int, KaraokeTrack>();

		public RemoveTracksCommand(KaraokeTrack[] tracks)
		{
			_trackIds = tracks.Select(t => t.Id).ToArray();
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			foreach (var id in _trackIds)
			{
				var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == id);
				if (track == null)
				{
					Logger.Warn($"Can't find track ID {id}, giving up");
					yield break;
				}

				_oldTracks[id] = track;
			}

			foreach (var id in _trackIds)
			{
				context.Project?.RemoveTrack(id);
			}

			yield return new TracksUpdate(_trackIds, TracksUpdate.UpdateType.Removed);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			foreach (var id in _trackIds)
			{
				context.Project?.AddTrack(_oldTracks[id]);
			}

			yield return new TracksUpdate(_trackIds, TracksUpdate.UpdateType.Added);
		}
	}

	internal class SetTrackEventsCommand : ICommand
	{
		private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();

		public string Description => _actionString;

		public bool CanUndo => true;

		private string _actionString;
		private int _trackId;
		private KaraokeEvent[] _newEvents;
		private KaraokeEvent[]? _oldEvents = null;

		public SetTrackEventsCommand(KaraokeTrack track, KaraokeEvent[] newEvents, string actionString = "Set track events")
		{
			_trackId = track.Id;
			_actionString = actionString;

			_newEvents = newEvents;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == _trackId);
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			_oldEvents = track.Events.ToArray();
			track.ReplaceEvents(_newEvents);
			yield return new EventsUpdate(_newEvents.Select(ev => ev.Id).ToArray(), EventsUpdate.UpdateType.Replace);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			var track = context.Project?.Tracks.FirstOrDefault(t => t.Id == _trackId);
			if (track == null)
			{
				Logger.Warn($"Can't find track ID {_trackId}, giving up");
				yield break;
			}

			if (_oldEvents == null)
			{
				Logger.Warn("Tried to undo a set track events command but _oldEvents is null?");
				yield break;
			}

			track.ReplaceEvents(_oldEvents);
			yield return new EventsUpdate(_oldEvents.Select(ev => ev.Id).ToArray(), EventsUpdate.UpdateType.Replace);
		}
	}

	internal class SetTrackSettingsCommand : ICommand
	{
		private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();

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
			yield return new TrackConfigUpdate(track.Id, _newConfig);
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
			yield return new TrackConfigUpdate(track.Id, _oldConfig);
		}
	}

	internal class RepositionTracksCommand : ICommand
	{
		private static NLog.Logger Logger = LogManager.GetCurrentClassLogger();

		public string Description => "Reposition tracks";

		public bool CanUndo => true;

		private HashSet<int> _trackIds;
		private int _newPosition;
		private Dictionary<int, int> _oldPositions = [];

		public RepositionTracksCommand(KaraokeTrack[] tracks, int newPosition)
		{
			_trackIds = new(tracks.Select(t => t.Id));
			_newPosition = newPosition;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			if (context.Project == null)
			{
				yield break;
			}

			_oldPositions = new(context.Project.Tracks.Select(t => new KeyValuePair<int, int>(t.Id, t.Order)));
			var newOrder = context.Project.Tracks.Where(t => !_trackIds.Contains(t.Id)).ToList();
			if (_newPosition > newOrder.Count)
			{
				newOrder.AddRange(context.Project.Tracks.Where(t => _trackIds.Contains(t.Id)));
			}
			else
			{
				newOrder.InsertRange(_newPosition, context.Project.Tracks.Where(t => _trackIds.Contains(t.Id)));
			}
			for (var i = 0; i < newOrder.Count; i++)
			{
				newOrder[i].Order = i;
			}

			yield return new TracksUpdate(context.Project.Tracks.Select(t => t.Id).ToArray(), TracksUpdate.UpdateType.Reordered);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			foreach (var track in context.Project?.Tracks ?? [])
			{
				if (!_oldPositions.ContainsKey(track.Id))
				{
					Logger.Warn($"Attempted to undo a track repositioning but no previous order for track {track.Id} found.");
					yield break;
				}

				track.Order = _oldPositions[track.Id];
			}

			yield return new TracksUpdate(context.Project?.Tracks.Select(t => t.Id).ToArray() ?? [], TracksUpdate.UpdateType.Reordered);
		}
	}
}
