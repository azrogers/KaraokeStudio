using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;

namespace KaraokeStudio
{
	internal static class WindowManager
	{
		public static SyncForm Sync
		{
			get
			{
				if (_syncForm == null || _syncForm.IsDisposed)
				{
					_syncForm = new SyncForm();
				}

				return _syncForm;
			}
		}

		private static SyncForm? _syncForm = null;

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

	internal class OpenSyncFormCommand : UndolessCommand
	{
		private KaraokeProject _project;
		private KaraokeTrack _track;

		public OpenSyncFormCommand(KaraokeProject project, KaraokeTrack track)
		{
			_project = project;
			_track = track;
		}

		public override void DoExecute(CommandContext context)
		{
			if (WindowManager.Sync.Visible)
			{
				WindowManager.Sync.Focus();
			}
			else
			{
				WindowManager.Sync.Open(_project, _track);
			}
		}
	}
}
