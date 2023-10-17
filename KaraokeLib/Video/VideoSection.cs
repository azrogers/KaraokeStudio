﻿using KaraokeLib.Lyrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	public class VideoSection
	{
		private VideoContext _context;
		private VideoParagraph[] _paragraphs;
		
		public VideoSectionType Type { get; private set; }

		public double Start { get; private set; }

		public double SectionLength { get; private set; }

		internal IEnumerable<VideoParagraph> Paragraphs => _paragraphs;

		public VideoSection(
			VideoContext context, 
			VideoSectionType type, 
			double sectionStart, 
			double sectionLength)
		{
			_context = context;
			_paragraphs = new VideoParagraph[0];
			Type = type;
			Start = sectionStart;
			SectionLength = sectionLength;
		}

		public VideoSection(
			VideoContext context,
			VideoSectionType type,
			VideoParagraph[] paragraphs)
		{
			_context = context;
			_paragraphs = paragraphs;
			Type = type;
			Start = paragraphs.Min(p => p.StartTimeSeconds);
			SectionLength = paragraphs.Max(p => p.EndTimeSeconds) - Start;
		}

		public void SetEvents(IEnumerable<LyricsEvent> events, VideoLayoutState layoutState)
		{
			_paragraphs = VideoParagraph.CreateParagraphs(_context, events.ToArray(), layoutState, _context.NumLines);
		}

		public static VideoSection[] SectionsFromTrack(VideoContext context, LyricsTrack track, VideoLayoutState layoutState)
		{
			var sections = new List<VideoSection>();

			var lastEventEndTime = 0.0;
			var firstEventStartTime = track.Events.FirstOrDefault()?.StartTimeSeconds ?? 0.0;
			var eventsAccumulated = new List<LyricsEvent>();

			foreach (var ev in track.Events)
			{
				var timeBetweenEvents = ev.StartTimeSeconds - lastEventEndTime;
				if (eventsAccumulated.Any() && timeBetweenEvents >= context.Config.MinTimeBetweenSections)
				{
					// break in the video, add a new section for the break and add all accumulated events
					var newSection = new VideoSection(
						context,
						VideoSectionType.Lyrics,
						firstEventStartTime,
						lastEventEndTime - firstEventStartTime);
					newSection.SetEvents(eventsAccumulated, layoutState);
					sections.Add(newSection);
					sections.Add(new VideoSection(
						context,
						VideoSectionType.Break,
						lastEventEndTime,
						timeBetweenEvents));

					eventsAccumulated.Clear();
					firstEventStartTime = ev.StartTimeSeconds;
				}

				lastEventEndTime = ev.EndTimeSeconds;
				eventsAccumulated.Add(ev);
			}

			// add last section
			if (eventsAccumulated.Any())
			{
				var newSection = new VideoSection(
					context,
					VideoSectionType.Lyrics,
					firstEventStartTime,
					lastEventEndTime - firstEventStartTime);
				newSection.SetEvents(eventsAccumulated, layoutState);
				sections.Add(newSection);
			}

			return sections.ToArray();
		}
	}

	public enum VideoSectionType
	{
		Lyrics,
		Break
	}
}
