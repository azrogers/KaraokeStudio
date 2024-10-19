using KaraokeLib.Config;
using KaraokeLib.Config.Attributes;
using KaraokeLib.Util;
using KaraokeLib.Video;
using Newtonsoft.Json;
using SkiaSharp;

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

		public override IEditableConfig? GetEventConfig()
		{
			return Settings;
		}

		public override void SetEventConfig(IEditableConfig? config)
		{
			Settings = config as ImageSettings;
		}

		public override string GetText(VideoLayoutState? layoutState)
		{
			return Path.GetFileName(Settings?.File ?? "");
		}
	}

	public class ImageSettings : EditableConfig<ImageSettings>
	{
		/// <summary>
		/// The image this ImageKaraokeEvent represents.
		/// </summary>
		[ConfigFile(AllowedExtensions = ["jpg", "jpeg", "png", "gif", "bmp", "ico", "webp", "astc", "avif", "dng", "heif", "ktx", "pkm", "wbmp"])]
		public string File;

		/// <summary>
		/// The opacity of this image, between 0 and 1.
		/// </summary>
		[ConfigRange(0.0, 1.0)]
		public float Opacity = 1.0f;

		/// <summary>
		/// The size of the image in the frame.
		/// </summary>
		public KSize Size;

		/// <summary>
		/// The position of this element's (0, 0) point relative to itself.
		/// </summary>
		public KAnchor Origin = KAnchor.TopLeft;

		/// <summary>
		/// The position of this element's (0, 0) point relative to the frame.
		/// </summary>
		public KAnchor Alignment = KAnchor.TopLeft;

		/// <summary>
		/// The offset of this element's (0, 0) point relative to its origin and alignment.
		/// </summary>
		public KSize Offset = new KSize(0, 0);

		public ImageSettings()
		{
			File = string.Empty;
		}

		public ImageSettings(string imageFile)
		{
			File = imageFile;
		}

		public ImageSettings(string imageFile, KSize size)
		{
			File = imageFile;
			Size = size;
		}
	}

	public class ImageUtil
	{
		public static KSize? GetImageSize(string imageFile)
		{
			using (var bitmap = SKBitmap.Decode(imageFile))
			{
				return bitmap != null ? new KSize(bitmap.Width, bitmap.Height) : null;
			}
		}
	}
}
