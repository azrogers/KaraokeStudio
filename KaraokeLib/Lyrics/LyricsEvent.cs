using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Lyrics
{
	public class LyricsEvent
	{
		private LyricsEventType _type;
		private int _id;
		private int _linkedId;
		private IEventTimecode _startTimecode;
		private IEventTimecode _endTimecode;

		private string? _text;

		public bool IsHyphenated { get; set; } = false;

		public double StartTimeSeconds => _startTimecode.GetTimeSeconds();

		public ulong StartTimeMilliseconds => _startTimecode.GetTimeMilliseconds();

		public IEventTimecode StartTime => _startTimecode;

		public double EndTimeSeconds => _endTimecode.GetTimeSeconds();

		public ulong EndTimeMilliseconds => _endTimecode.GetTimeMilliseconds();

		public double LengthSeconds => EndTimeSeconds - StartTimeSeconds;

		public IEventTimecode EndTime => _endTimecode;

		public LyricsEventType Type => _type;

		public string Text => _text + (IsHyphenated ? "-" : "") ?? "";

		public int Id => _id;
		public int LinkedId => _linkedId;

		public LyricsEvent(
			LyricsEventType type, 
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
			_text = null;
		}

		public void SetText(string text)
		{
			_text = text;
		}

		public double GetNormalizedPosition(double songPosition)
		{
			return Math.Clamp((songPosition - StartTimeSeconds) / LengthSeconds, 0, 1);
		}
	}

	public interface IEventTimecode
	{
		double GetTimeSeconds();
		ulong GetTimeMilliseconds();
	}

	public enum LyricsEventType : byte
	{
		Lyric = 0,
		ParagraphBreak = 1,
		LineBreak = 2
	}
}
