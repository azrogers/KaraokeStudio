using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeStudio.Project;

namespace KaraokeStudio.Commands.Updates
{
	internal interface IUpdate
	{
	}

	/// <summary>
	/// An update denoting that the project that's currently open has changed.
	/// </summary>
	internal class ProjectUpdate : IUpdate
	{
		public KaraokeProject? Project;
		public string? ProjectPath;

		public ProjectUpdate(KaraokeProject? project, string? projectPath)
		{
			Project = project;
			ProjectPath = projectPath;
		}
	}

	/// <summary>
	/// An update denoting that the project's config has changed.
	/// </summary>
	internal class ProjectConfigUpdate : IUpdate
	{
		public KaraokeProject Project;

		public ProjectConfigUpdate(KaraokeProject project)
		{
			Project = project;
		}
	}

	/// <summary>
	/// An update denoting that one or more events have changed.
	/// </summary>
	internal class EventsUpdate : IUpdate
	{
		public int[] EventIds;
		public UpdateType Type;

		public EventsUpdate(int[] eventIds, UpdateType type)
		{
			EventIds = eventIds;
			Type = type;
		}

		public enum UpdateType
		{
			Timing,
			Add,
			Remove,
			Replace
		}
	}

	/// <summary>
	/// An update denoting that one or more tracks were added or removed.
	/// </summary>
	internal class TracksUpdate : IUpdate
	{
		public int[] TrackIds;
		public bool Added;

		public TracksUpdate(int[] trackIds, bool added = true)
		{
			TrackIds = trackIds;
			Added = added;
		}

		public TracksUpdate(int trackId, bool added = true)
		{
			TrackIds = new int[] { trackId };
			Added = added;
		}
	}

	/// <summary>
	/// An update denoting that one or more tracks have had their settings changed.
	/// </summary>
	internal class TrackConfigUpdate : IUpdate
	{
		public int[] TrackIds;
		public IEditableConfig NewConfig;

		public TrackConfigUpdate(int trackId, IEditableConfig newConfig)
		{
			TrackIds = new int[] { trackId };
			NewConfig = newConfig;
		}
	}

	/// <summary>
	/// An update denoting that audio settings have changed.
	/// </summary>
	internal class AudioSettingsUpdate : IUpdate
	{
		public AudioSettings Settings;

		public AudioSettingsUpdate(AudioSettings settings)
		{
			Settings = settings;
		}
	}
}
