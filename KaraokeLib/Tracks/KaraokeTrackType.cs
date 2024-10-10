using KaraokeLib.Events;

namespace KaraokeLib.Tracks
{
	public enum KaraokeTrackType : byte
	{
		Lyrics = 0x00,
		Graphics = 0x01,
		Audio = 0x02
	}

	public static class KaraokeTrackTypeMapping
	{
		private static readonly Dictionary<KaraokeTrackType, HashSet<KaraokeEventType>> _validEvents = new Dictionary<KaraokeTrackType, HashSet<KaraokeEventType>>()
		{
			{ KaraokeTrackType.Lyrics, new HashSet<KaraokeEventType>() { KaraokeEventType.Lyric, KaraokeEventType.ParagraphBreak, KaraokeEventType.LineBreak } },
			{ KaraokeTrackType.Audio, new HashSet<KaraokeEventType>() { KaraokeEventType.AudioClip } },
			{ KaraokeTrackType.Graphics, new HashSet<KaraokeEventType>() { KaraokeEventType.Image } }
		};

		private static readonly Dictionary<KaraokeTrackType, Type> _trackSettingsTypes = new Dictionary<KaraokeTrackType, Type>()
		{
			{KaraokeTrackType.Lyrics, typeof(LyricsTrackSettings) },
			{ KaraokeTrackType.Audio, typeof(AudioTrackSettings) },
			{ KaraokeTrackType.Graphics, typeof(GraphicsTrackSettings) }
		};

		/// <summary>
		/// Is the given event type valid for this track type?
		/// </summary>
		public static bool IsEventValid(KaraokeTrackType trackType, KaraokeEventType eventType)
		{
			if (!_validEvents.ContainsKey(trackType))
			{
				throw new NotImplementedException($"Missing _validEvents entry for track of type {trackType}");
			}

			return _validEvents[trackType].Contains(eventType);
		}

		public static Type GetTrackSettingsType(KaraokeTrackType trackType)
		{
			return _trackSettingsTypes[trackType];
		}

		public static bool TrackHasVideoContent(KaraokeTrackType trackType)
		{
			switch(trackType)
			{
				case KaraokeTrackType.Graphics:
				case KaraokeTrackType.Lyrics:
					return true;
				default:
					return false;
			}
		}
	}
}
