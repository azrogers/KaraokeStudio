﻿using Melanchall.DryWetMidi.Interaction;
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

		public IEventTimecode EndTime => _endTimecode;

		public double LengthSeconds => EndTimeSeconds - StartTimeSeconds;

		public LyricsEventType Type => _type;

		public string Text
		{
			get
			{
				return _text + (IsHyphenated ? "-" : "") ?? "";
			}
			set
			{
				_text = value;
			}
		}

		public string RawText => _text ?? "";

		public int Id => _id;

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
	}

	public interface IEventTimecode : IComparable<IEventTimecode>
	{
		double GetTimeSeconds();
		ulong GetTimeMilliseconds();
	}

	/// <summary>
	/// Basic implementation of IEventTimecode.
	/// </summary>
	public class TimeSpanTimecode : IEventTimecode
	{
		private TimeSpan _span;

		public TimeSpanTimecode(TimeSpan span)
		{
			_span = span;
		}

		public TimeSpanTimecode(double seconds) => _span = TimeSpan.FromSeconds(seconds);

		public TimeSpanTimecode(uint milliseconds)
		{
			_span = TimeSpan.FromMilliseconds(milliseconds);
		}

		public double GetTimeSeconds() => _span.TotalSeconds;

		public ulong GetTimeMilliseconds() => (ulong)(_span.TotalMicroseconds / 1000L);

		public int CompareTo(IEventTimecode? other)
		{
			if(other == null)
			{
				return 1;
			}

			return GetTimeMilliseconds().CompareTo(other.GetTimeMilliseconds());
		}
	}

	public enum LyricsEventType : byte
	{
		Lyric = 0,
		ParagraphBreak = 1,
		LineBreak = 2
	}
}
