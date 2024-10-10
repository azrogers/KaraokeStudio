using KaraokeLib.Audio;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Util;
using NLog;

namespace KaraokeStudio.Commands
{
	internal class SetAudioSettingsCommand : ICommand
	{
		public Logger Logger => LogManager.GetCurrentClassLogger();

		public string Description => "Change audio settings";

		public bool CanUndo => true;

		private AudioSettings? _oldSettings = null;
		private AudioSettings _newSettings;

		public SetAudioSettingsCommand(AudioSettings settings)
		{
			_newSettings = settings;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			_oldSettings = AppSettings.Instance.AudioSettings.CopyTyped();
			AppSettings.Instance.SetAudioSettings(_newSettings);
			yield return new AudioSettingsUpdate(_newSettings);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			if (_oldSettings == null)
			{
				Logger.Warn("Attempted to undo audio settings change but _oldSettings was null, giving up");
				yield break;
			}

			AppSettings.Instance.SetAudioSettings(_oldSettings);
			yield return new AudioSettingsUpdate(_oldSettings);
		}
	}
}
