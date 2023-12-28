using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Project;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaraokeStudio
{
	internal static class WindowManager
	{
		private static SyncForm Sync
		{
			get
			{
				if(_syncForm == null || _syncForm.IsDisposed)
				{
					_syncForm = new SyncForm();
				}

				return _syncForm;
			}
		}

		private static SyncForm? _syncForm = null;

		internal static void OpenSyncForm(KaraokeProject project, KaraokeTrack track)
		{
			if(Sync.Visible)
			{
				Sync.Focus();
			}
			else
			{
				Sync.Open(project, track);
			}
		}

		internal static bool OnProjectWillChange()
		{
			return Sync.OnProjectWillChange();
		}

		internal static void OnProjectChanged(KaraokeProject? project)
		{
			Sync.Hide();
		}

		internal static void OnLyricsEventsChanged(KaraokeProject project, (KaraokeTrack Track, IEnumerable<KaraokeEvent> NewEvents) obj)
		{
			Sync.OnProjectEventsChanged();
		}

		internal static void OnTrackEventsChanged(KaraokeProject project, KaraokeTrack obj)
		{
			Sync.OnProjectEventsChanged();
		}
	}
}
