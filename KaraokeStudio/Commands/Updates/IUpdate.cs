using KaraokeLib.Config;
using KaraokeStudio.Project;

namespace KaraokeStudio.Commands.Updates
{
	internal interface IUpdate
	{
	}

	/// <summary>
	/// An update denoting that which project is currently open has changed.
	/// </summary>
	internal class ProjectUpdate : IUpdate
	{
		public KaraokeProject? Project;

		public ProjectUpdate(KaraokeProject? project)
		{
			Project = project;
		}
	}

	/// <summary>
	/// An update denoting that one or more events have had their timings changed.
	/// </summary>
	internal class EventTimingsUpdate : IUpdate
	{
		public int[] EventIds;

		public EventTimingsUpdate(int[] eventIds)
		{
			EventIds = eventIds;
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
	internal class TrackSettingsUpdate : IUpdate
	{
		public int[] TrackIds;
		public IEditableConfig NewConfig;

		public TrackSettingsUpdate(int trackId, IEditableConfig newConfig)
		{
			TrackIds = new int[] { trackId };
			NewConfig = newConfig;
		}
	}
}
