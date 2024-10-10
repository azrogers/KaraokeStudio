using CSCore;
using CSCore.Codecs;
using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using KaraokeLib.Video;
using Newtonsoft.Json;

namespace KaraokeLib.Events
{
	public class ImageKaraokeEvent : KaraokeEvent
	{
		public ImageSettings? Settings
		{
			get => JsonConvert.DeserializeObject<ImageSettings>(_value ?? "");
			set => _value = JsonConvert.SerializeObject(value);
		}

		public ImageKaraokeEvent(ImageSettings settings, int id, IEventTimecode start, IEventTimecode end, int linkedId = -1)
			: base(KaraokeEventType.AudioClip, id, start, end, linkedId)
		{
			_value = JsonConvert.SerializeObject(settings);
		}

		public ImageKaraokeEvent(ImageSettings settings, KaraokeEvent ev)
			: this(ev)
		{
			Settings = settings;
		}

		public ImageKaraokeEvent(KaraokeEvent ev)
			: base(ev.Type, ev.Id, ev.StartTime, ev.EndTime, ev.LinkedId)
		{
			_value = ev.RawValue;
		}

		public override string GetText(VideoLayoutState? layoutState)
		{
			return Path.GetFileName(Settings?.ImageFile ?? "");
		}
	}

	public class ImageSettings : EditableConfig<ImageSettings>
	{
		/// <summary>
		/// The image this ImageKaraokeEvent represents.
		/// </summary>
		public string ImageFile;

		/// <summary>
		/// The opacity of this image, between 0 and 1.
		/// </summary>
		[ConfigRange(0.0, 1.0)]
		public float Opacity = 1.0f;

		public ImageSettings(string imageFile)
		{
			ImageFile = imageFile;
		}
	}
}
