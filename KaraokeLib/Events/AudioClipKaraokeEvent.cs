using KaraokeLib.Config;
using KaraokeLib.Video;
using Newtonsoft.Json;

namespace KaraokeLib.Events
{
	public class AudioClipKaraokeEvent : KaraokeEvent
	{
		public AudioClipSettings? Settings
		{
			get => JsonConvert.DeserializeObject<AudioClipSettings>(_value ?? "");
			set => _value = JsonConvert.SerializeObject(value);
		}

		public AudioClipKaraokeEvent(AudioClipSettings settings, int id, IEventTimecode start, IEventTimecode end, int linkedId = -1)
			: base(KaraokeEventType.AudioClip, id, start, end, linkedId)
		{
			_value = JsonConvert.SerializeObject(settings);
		}

		public AudioClipKaraokeEvent(AudioClipSettings settings, KaraokeEvent ev)
			: this(ev)
		{
			Settings = settings;
		}

		public AudioClipKaraokeEvent(KaraokeEvent ev)
			: base(ev.Type, ev.Id, ev.StartTime, ev.EndTime, ev.LinkedId)
		{
			_value = ev.RawValue;
		}

		public override string GetText(VideoLayoutState? layoutState)
		{
			return Path.GetFileName(Settings?.AudioFile ?? "");
		}
	}

	public class AudioClipSettings : EditableConfig<AudioClipSettings>
	{
		/// <summary>
		/// The offset in seconds from the start of the audio file that this clip starts at.
		/// </summary>
		public double Offset;

		/// <summary>
		/// The audio file this clip refers to.
		/// </summary>
		public string AudioFile;

		public AudioClipSettings(string audioFile)
		{
			AudioFile = audioFile;
		}
	}
}
