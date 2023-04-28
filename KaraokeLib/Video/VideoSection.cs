using KaraokeLib.Lyrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

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

		public void SetEvents(IEnumerable<LyricsEvent> events)
		{
			var newParagraphs = new List<VideoParagraph>();
			var eventsArr = events.ToArray();
			var i = 0;

			while(i < eventsArr.Length)
			{
				var para = new VideoParagraph(_context, _context.NumLines);
				var numConsumed = para.FillParagraph(eventsArr, i);

				if(numConsumed <= 0)
				{
					throw new InvalidDataException("Paragraph took zero events?");
				}

				i += numConsumed;
				newParagraphs.Add(para);
			}

			_paragraphs = newParagraphs.ToArray();
		}

		public static VideoSection[] FromTrack(VideoContext context, LyricsTrack track)
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
					newSection.SetEvents(eventsAccumulated);
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
				newSection.SetEvents(eventsAccumulated);
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
