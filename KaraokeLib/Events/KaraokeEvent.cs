using KaraokeLib.Files;
using KaraokeLib.Files.Ksf;
using KaraokeLib.Tracks;
using KaraokeLib.Video;

namespace KaraokeLib.Events
{
	[KsfSerializable(KsfObjectType.Event)]
	public class KaraokeEvent : IKsfBinaryObject
	{
		private KaraokeEventType _type;
		private int _id;
		private int _linkedId;
		private IEventTimecode _startTimecode;
		private IEventTimecode _endTimecode;

		protected string? _value;

		public double StartTimeSeconds => _startTimecode.GetTimeSeconds();

		public ulong StartTimeMilliseconds => _startTimecode.GetTimeMilliseconds();

		public IEventTimecode StartTime => _startTimecode;

		public double EndTimeSeconds => _endTimecode.GetTimeSeconds();

		public ulong EndTimeMilliseconds => _endTimecode.GetTimeMilliseconds();

		public IEventTimecode EndTime => _endTimecode;

		public double LengthSeconds => EndTimeSeconds - StartTimeSeconds;

		public KaraokeEventType Type => _type;

		public string? RawValue
		{
			get => _value;
			set => _value = value;
		}

		public int Id
		{
			get => _id;
			internal set => _id = value;
		}

		/// <summary>
		/// The ID of a previous event that this one is linked to.
		/// </summary>
		/// <remarks>
		/// This is used to connect syllables of words. The first syllable will have a LinkedID of -1 and the rest will reference the previous syllable.
		/// "Ab-so-lute-ly" => [
		///		"Ab" { Id = 0, LinkedId = -1 }, 
		///		"so" { Id = 1, LinkedId = 0 }, 
		///		"lute" { Id = 2, LinkedId = 1 }, 
		///		"ly" { Id = 3, LinkedId = 2 }
		///	]
		/// </remarks>
		public int LinkedId { get => _linkedId; set => _linkedId = value; }

		public KaraokeEvent(KaraokeEvent ev) : this(ev.Type, ev.Id, ev.StartTime, ev.EndTime, ev.LinkedId)
		{
			_value = ev._value;
		}

		public KaraokeEvent(
			KaraokeEventType type,
			int id,
			IEventTimecode startTimecode,
			IEventTimecode endTimecode,
			int linkedId = -1)
		{
			_type = type;
			_id = id;
			_linkedId = linkedId;
			_startTimecode = startTimecode;
			_endTimecode = endTimecode;
			_value = null;
		}

		public KaraokeEvent(
			KaraokeEventType type,
			FileIdTracker idTracker,
			IEventTimecode startTimecode,
			IEventTimecode endTimecode,
			int linkedId = -1)
		{
			_type = type;
			_id = idTracker.AddNewEvent(this);
			_startTimecode = startTimecode;
			_endTimecode = endTimecode;
			_value = null;
		}

		// used for serialization
		private KaraokeEvent() : this(KaraokeEventType.Lyric, 0, new TimeSpanTimecode(0), new TimeSpanTimecode(0))
		{

		}

		/// <summary>
		/// Returns the position of the cursor within this event, normalized between [0, 1]
		/// </summary>
		public double GetNormalizedPosition(double songPosition)
		{
			return Math.Clamp((songPosition - StartTimeSeconds) / LengthSeconds, 0, 1);
		}

		/// <summary>
		/// Is the given time contained within the bounds of this event?
		/// </summary>
		public bool ContainsTime(double time)
		{
			return time >= StartTimeSeconds && time <= EndTimeSeconds;
		}

		/// <summary>
		/// Gets the text of the lyric that should be displayed.
		/// </summary>
		/// <param name="layoutState">The layout state that informs how this event should be displayed.</param>
		public virtual string GetText(VideoLayoutState? layoutState)
		{
			return (_value ?? "") + (layoutState?.IsHyphenated(this) ?? false ? "-" : "");
		}

		/// <summary>
		/// Sets the start and end times of this event.
		/// </summary>
		public virtual void SetTiming(IEventTimecode start, IEventTimecode end)
		{
			_startTimecode = start;
			_endTimecode = end;
		}

		public void Write(BinaryWriter writer)
		{
			byte infoByte = 0;
			infoByte |= (byte)(LinkedId != -1 ? 1 : 0);
			infoByte |= (byte)((RawValue != null ? 1 : 0) << 1);

			writer.Write(infoByte);

			// write event
			writer.Write((byte)Type);
			writer.Write(Id);
			writer.Write((uint)StartTimeMilliseconds);
			writer.Write((uint)EndTimeMilliseconds);
			if (LinkedId != -1)
			{
				writer.Write(LinkedId);
			}
			if (RawValue != null)
			{
				writer.WriteNullTerminatedString(RawValue);
			}
		}

		public object Read(BinaryReader reader, byte version)
		{
			var infoByte = reader.ReadByte();
			var hasLinkedId = ((infoByte >> 0) & 1) == 1;
			var hasRawValue = ((infoByte >> 1) & 1) == 1;

			_type = (KaraokeEventType)reader.ReadByte();
			_id = reader.ReadInt32();
			_startTimecode = new TimeSpanTimecode(reader.ReadUInt32());
			_endTimecode = new TimeSpanTimecode(reader.ReadUInt32());

			_linkedId = -1;
			if (hasLinkedId)
			{
				_linkedId = reader.ReadInt32();
			}

			_value = null;
			if (hasRawValue)
			{
				_value = reader.ReadNullTerminatedString();
			}

			if (_type == KaraokeEventType.AudioClip)
			{
				return new AudioClipKaraokeEvent(this);
			}

			return this;
		}

		public override string ToString()
		{
			return $"{_type} <{StartTimeSeconds} - {EndTimeSeconds}> {_value}";
		}
	}

	/// <summary>
	/// Identifies a type of event in a KaraokeTrack.
	/// </summary>
	/// <remarks>
	/// When updating, also update <see cref="KaraokeTrack.ValidEvents"/>.
	/// </remarks>
	public enum KaraokeEventType : byte
	{
		// lyrics
		Lyric = 0,
		ParagraphBreak = 1,
		LineBreak = 2,

		// audio
		AudioClip = 15,

		// graphics
		Image = 30
	}
}
