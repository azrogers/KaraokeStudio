using KaraokeStudio.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeStudio.Commands.Updates
{
    internal interface IUpdate
    {
    }

    internal class ProjectUpdate : IUpdate
    {
        public KaraokeProject? Project;

        public ProjectUpdate(KaraokeProject? project)
        {
            Project = project;
        }
    }

    internal class EventsUpdate : IUpdate
    {
        public int[] EventIds;

        public EventsUpdate(int[] eventIds)
        {
            EventIds = eventIds;
        }
    }

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
}
