using CSCore;
using CSCore.Codecs;
using KaraokeLib.Audio;
using KaraokeLib.Config;
using KaraokeLib.Video;
using Newtonsoft.Json;

namespace KaraokeLib.Events
{
	public class AudioClipKaraokeEvent : KaraokeEvent
	{
		private double _sourceLength;

		public AudioClipSettings? Settings
		{
			get => JsonConvert.DeserializeObject<AudioClipSettings>(_value ?? "");
			set => _value = JsonConvert.SerializeObject(value);
		}

		public AudioClipKaraokeEvent(AudioClipSettings settings, int id, IEventTimecode start, IEventTimecode end, int linkedId = -1)
			: base(KaraokeEventType.AudioClip, id, start, end, linkedId)
		{
			_value = JsonConvert.SerializeObject(settings);
			_sourceLength = GetSourceLength();
		}

		public AudioClipKaraokeEvent(AudioClipSettings settings, KaraokeEvent ev)
			: this(ev)
		{
			Settings = settings;
			_sourceLength = GetSourceLength();
		}

		public AudioClipKaraokeEvent(KaraokeEvent ev)
			: base(ev.Type, ev.Id, ev.StartTime, ev.EndTime, ev.LinkedId)
		{
			_value = ev.RawValue;
			_sourceLength = GetSourceLength();
		}

		public override string GetText(VideoLayoutState? layoutState)
		{
			return Path.GetFileName(Settings?.AudioFile ?? "");
		}

		public override void SetTiming(IEventTimecode start, IEventTimecode end)
		{
			var newLen = end.GetTimeSeconds() - start.GetTimeSeconds();
			// length is the same - it's just a move, nothing to do
			if(Math.Abs(newLen - LengthSeconds) <= 0.0001)
			{
				base.SetTiming(start, end);
				return;
			}

			// adjust offset to compensate for changed start time
			var settings = Settings ?? new AudioClipSettings("");
			var timeDiff = start.GetTimeSeconds() - StartTimeSeconds;
			var newOffset = settings.Offset + timeDiff;
			settings.Offset = Math.Max(newOffset, 0);
			// limit the start to the length of the clip
			start = new TimeSpanTimecode(start.GetTimeSeconds() - Math.Min(newOffset, 0));
			// limit end to the length of the clip
			end = new TimeSpanTimecode(Math.Min(end.GetTimeSeconds(), start.GetTimeSeconds() + (_sourceLength - settings.Offset)));
			base.SetTiming(start, end);
			Settings = settings;
		}

		private double GetSourceLength()
		{
			if(Settings?.AudioFile == null)
			{
				return 0;
			}

			var info = AudioUtil.GetFileInfo(Settings.AudioFile);
			return info?.LengthSeconds ?? 0;
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

		public IWaveSource LoadAudioFile()
		{
			return CodecFactory.Instance.GetCodec(AudioFile);
		}
	}
}
