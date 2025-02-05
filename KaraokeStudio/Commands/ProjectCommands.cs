﻿using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using KaraokeLib.Events;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;
using Ookii.Dialogs.WinForms;
using System.Reflection;

namespace KaraokeStudio.Commands
{
	internal class SetProjectConfigCommand : ICommand
	{
		private static NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

		public string Description => _actionString;

		public bool CanUndo => true;

		private string _actionString;
		private KaraokeConfig? _oldConfig;
		private KaraokeConfig _newConfig;

		public SetProjectConfigCommand(KaraokeConfig newConfig, string actionString = "Set project config")
		{
			_newConfig = newConfig;
			_actionString = actionString;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			if (context.Project == null)
			{
				Logger.Warn("Can't set project config of a null project, giving up");
				yield break;
			}

			_oldConfig = (KaraokeConfig)context.Project.Config.Copy();
			context.Project.Config = _newConfig;
			yield return new ProjectConfigUpdate(context.Project);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			if (context.Project == null)
			{
				Logger.Warn("Can't set project config of a null project, giving up");
				yield break;
			}

			if (_oldConfig == null)
			{
				Logger.Warn("Tried to undo set project config command, but _oldConfig is null?");
				yield break;
			}

			context.Project.Config = _oldConfig;
			yield return new ProjectConfigUpdate(context.Project);
		}
	}

	internal class NewProjectCommand : ICommand
	{
		public string Description => "New project";

		public bool CanUndo => false;

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			var audioFile = ProjectUtil.OpenAudioFile(null);
			if (audioFile == null)
			{
				yield break;
			}

			yield return new ProjectUpdate(KaraokeProject.Create(audioFile), null);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			throw new NotImplementedException();
		}
	}

	internal class OpenProjectCommand : ICommand
	{
		public string Description => "Open project";

		public bool CanUndo => false;

		private string _file;

		public OpenProjectCommand(string file)
		{
			_file = file;
		}

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			if (!File.Exists(_file))
			{
				yield break;
			}

			yield return new ProjectUpdate(KaraokeProject.Load(_file), _file);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			throw new NotImplementedException();
		}
	}

	internal class LoadMidiFileCommand : ICommand
	{
		public string Description => "Import midi file";

		public bool CanUndo => false;

		private string _file;

		public IEnumerable<IUpdate> Execute(CommandContext context)
		{
			var dialog = new VistaOpenFileDialog();
			dialog.CheckFileExists = true;
			dialog.Multiselect = false;
			dialog.Title = "Open MIDI file";
			dialog.Filter = "Guitar Hero/Rock Band MIDI|notes.mid|MIDI files|*.mid|All files|*.*";
			if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName))
			{
				yield break;
			}

			var midiFile = dialog.FileName;
			var audioFile = ProjectUtil.OpenAudioFile(Path.GetDirectoryName(midiFile));
			if (audioFile == null)
			{
				yield break;
			}

			yield return new ProjectUpdate(KaraokeProject.FromMidi(midiFile, audioFile), null);
		}

		public IEnumerable<IUpdate> Undo(CommandContext context)
		{
			throw new NotImplementedException();
		}
	}

	internal class ProjectUtil
	{
		internal static string? OpenAudioFile(string? baseDir)
		{
			return OpenFile<AudioClipSettings>(baseDir, "Open audio file", "Audio files", nameof(AudioClipSettings.AudioFile));
		}

		internal static string? OpenImageFile(string? baseDir)
		{
			return OpenFile<ImageSettings>(baseDir, "Open image file", "Image files", nameof(ImageSettings.File));
		}

		private static string? OpenFile<T>(string? baseDir, string title, string displayName, string fieldName) where T : IEditableConfig
		{
			var dialog = new VistaOpenFileDialog
			{
				CheckFileExists = true,
				Multiselect = false,
				Title = title,
				Filter = GetConfigFilter<T>(displayName, fieldName),
				InitialDirectory = baseDir
			};

			if (dialog.ShowDialog() != DialogResult.OK || !File.Exists(dialog.FileName))
			{
				return null;
			}

			return dialog.FileName;
		}

		private static string GetConfigFilter<T>(string displayName, string fieldName) where T: IEditableConfig
		{
			var fieldInfo = typeof(T).GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
			var extensions = fieldInfo?.GetCustomAttribute<ConfigFileAttribute>()?.AllowedExtensions ?? [];
			if (!extensions.Any())
			{
				return "All files (*.*)|*.*";
			}
			else
			{
				return $"${displayName} ({string.Join(", ", extensions.Select(e => "*." + e))})|{string.Join(";", extensions.Select(e => "*." + e))}|All files (*.*)|*.*";
			}
		}
	}
}
