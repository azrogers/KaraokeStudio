using KaraokeLib.Audio;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Config;
using KaraokeStudio.Project;
using KaraokeStudio.Util;

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

		public static StyleForm Style
		{
			get
			{
				if (_styleForm == null || _styleForm.IsDisposed)
				{
					_styleForm = new StyleForm();
				}

				return _styleForm;
			}
		}

		public static ConsoleForm Console
		{
			get
			{
				if (_consoleForm == null || _consoleForm.IsDisposed)
				{
					_consoleForm = new ConsoleForm();
				}

				return _consoleForm;
			}
		}

		public static ExportVideoForm Export
		{
			get
			{
				if (_exportForm == null || _exportForm.IsDisposed)
				{
					_exportForm = new ExportVideoForm();
				}

				return _exportForm;
			}
		}

		public static GenericConfigEditorForm AudioSettings
		{
			get
			{
				if (_audioSettingsForm == null || _audioSettingsForm.IsDisposed)
				{
					_audioSettingsForm = new GenericConfigEditorForm();
				}

				return _audioSettingsForm;
			}
		}

		private static SyncForm? _syncForm = null;
		private static ConsoleForm? _consoleForm = null;
		private static StyleForm? _styleForm = null;
		private static ExportVideoForm? _exportForm = null;
		private static GenericConfigEditorForm? _audioSettingsForm = null;

		private static Dictionary<int, GenericConfigEditorForm> _trackSettingsEditors = new Dictionary<int, GenericConfigEditorForm>();

		static WindowManager()
		{
			// when track settings change, replace the settings in any active properties windows unless they're dirty
			UpdateDispatcher.RegisterHandler<TrackConfigUpdate>(update =>
			{
				foreach (var id in update.TrackIds)
				{
					if (_trackSettingsEditors.ContainsKey(id) && !_trackSettingsEditors[id].IsDisposed)
					{
						_trackSettingsEditors[id].ReplaceConfigIfNotChanged(update.NewConfig);
					}
				}
			});
		}

		internal static void OpenTrackSettingsEditor(KaraokeTrack track)
		{
			if (!_trackSettingsEditors.ContainsKey(track.Id) || _trackSettingsEditors[track.Id].IsDisposed)
			{
				_trackSettingsEditors[track.Id] = new GenericConfigEditorForm();
			}

			if (_trackSettingsEditors[track.Id].Visible)
			{
				_trackSettingsEditors[track.Id].Focus();
			}
			else
			{
				_trackSettingsEditors[track.Id].Open(track.GetTrackConfig(), newConfig =>
				{
					CommandDispatcher.Dispatch(new SetTrackSettingsCommand(track, newConfig));
				});
			}
		}

		internal static bool OnProjectWillChange()
		{
			return Sync.OnProjectWillChange();
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

	internal class OpenTrackSettingsCommand : UndolessCommand
	{
		private KaraokeTrack _track;

		public OpenTrackSettingsCommand(KaraokeTrack track)
		{
			_track = track;
		}

		public override void DoExecute(CommandContext context)
		{
			WindowManager.OpenTrackSettingsEditor(_track);
		}
	}

	internal class OpenConsoleCommand : UndolessCommand
	{
		public override void DoExecute(CommandContext context)
		{
			if (WindowManager.Console.Visible)
			{
				WindowManager.Console.Focus();
			}
			else
			{
				WindowManager.Console.Show();
			}
		}
	}

	internal class OpenStyleCommand : UndolessCommand
	{
		public override void DoExecute(CommandContext context)
		{
			if (WindowManager.Style.Visible)
			{
				WindowManager.Style.Focus();
			}
			else
			{
				WindowManager.Style.Open(context.Project);
			}
		}
	}

	internal class OpenExportVideoCommand : UndolessCommand
	{
		public override void DoExecute(CommandContext context)
		{
			if (WindowManager.Export.Visible)
			{
				WindowManager.Export.Focus();
			}
			else
			{
				WindowManager.Export.Open(context.Project);
			}
		}
	}

	internal class OpenAudioSettingsCommand : UndolessCommand
	{
		public override void DoExecute(CommandContext context)
		{
			if (WindowManager.AudioSettings.Visible)
			{
				WindowManager.AudioSettings.Focus();
			}
			else
			{
				WindowManager.AudioSettings.Open(AppSettings.Instance.AudioSettings, settings =>
				{
					CommandDispatcher.Dispatch(new SetAudioSettingsCommand((AudioSettings)settings));
				});
			}
		}
	}
}
