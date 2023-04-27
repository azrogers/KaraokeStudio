using KaraokeLib.Lyrics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KaraokeLib.Video
{
	internal class VideoSection
	{
		private VideoContext _context;
		private VideoParagraph[] _paragraphs;
		
		public VideoSectionType Type { get; private set; }

		public double Start { get; private set; }

		public double SectionLength { get; private set; }

		public IEnumerable<VideoParagraph> Paragraphs => _paragraphs;

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
	}

	internal enum VideoSectionType
	{
		Lyrics,
		Break
	}
}
