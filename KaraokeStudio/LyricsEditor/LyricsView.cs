using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using KaraokeLib;
using KaraokeLib.Video;
using KaraokeLib.Video.Elements;
using SkiaSharp;

namespace KaraokeStudio.LyricsEditor
{
	internal class LyricsView
	{
		private KaraokeProject? _project = null;
		private VideoStyle _style;
		private KaraokeConfig _config;
		private Dictionary<int, ElementRenderingInfo> _elementRenderingInfo;
		private int[] _elementsToDraw;
		private (int ElementId, int CharOffset)? _cursorPos = null;
		private float _lineHeight = 0;

		private SKPictureRecorder _pictureRecorder;
		private SKPicture? _picture = null;
		private SKPaint _cursorPaint;

		public float Height { get; private set; }

		public LyricsView()
		{
			_pictureRecorder = new SKPictureRecorder();
			_elementsToDraw = new int[0];
			_elementRenderingInfo = new Dictionary<int, ElementRenderingInfo>();

			_config = new KaraokeConfig()
			{
				Font = new KaraokeLib.Util.KFont()
				{
					Family = "Arial",
					Size = 50,
					Slant = SKFontStyleSlant.Upright,
					Weight = SKFontStyleWeight.Normal,
					Width = SKFontStyleWidth.Normal
				},
				BackgroundColor = new KaraokeLib.Util.KColor(0, 0, 0),
				NormalColor = new KaraokeLib.Util.KColor(255, 255, 255),
				StrokeWidth = 0,
				HorizontalAlign = KaraokeLib.HorizontalAlignment.Left,
				VerticalAlign = VerticalAlignment.Top,
				Padding = new KaraokeLib.Util.KPadding(5.0f, 15.0f, 5.0f, 15.0f)
			};

			_cursorPaint = new SKPaint()
			{
				Color = new SKColor(255, 255, 255),
				StrokeWidth = 2.0f
			};

			_style = new VideoStyle(_config);
		}

		public void UpdateView(KaraokeProject? project, RectangleF viewRect)
		{
			_project = project;

			var lyricsTrack = _project?.Tracks.Where(t => t.Type == KaraokeLib.Lyrics.LyricsTrackType.Lyrics).FirstOrDefault();
			if (lyricsTrack == null)
			{
				_elementsToDraw = new int[0];
				return;
			}

			_config.VideoSize = new KaraokeLib.Util.KSize((int)viewRect.Width, (int)viewRect.Height * 2);
			var context = new VideoContext(_style, _config, new VideoTimecode(lyricsTrack.Events.Max(m => m.EndTimeSeconds), _config.FrameRate));

			var layoutState = new VideoLayoutState();
			var paragraphs = VideoParagraph.CreateParagraphs(context, lyricsTrack.Events.ToArray(), layoutState, -1);
			var section = new VideoSection(context, VideoSectionType.Lyrics, paragraphs);
			var elements = VideoElementGenerator.Generate(context, layoutState, new VideoSection[] { section });
			UpdateElementPositions(context, elements);

			var canvas = _pictureRecorder.BeginRecording(new SKRect(0, 0, viewRect.Width, Height));

			foreach (var elementId in _elementsToDraw)
			{
				var renderingInfo = _elementRenderingInfo[elementId];

				var savePoint = canvas.Save();
				canvas.Translate(0, renderingInfo.YOffset);
				renderingInfo.Element.Render(context, canvas, 0);
				canvas.RestoreToCount(savePoint);
			}

			_picture = _pictureRecorder.EndRecording();
		}

		public void Render(SKCanvas destination, SKMatrix matrix, SKRect visibleRect)
		{
			destination.Clear();

			if (_picture != null)
			{
				destination.DrawPicture(_picture, ref matrix);
			}

			if(_cursorPos != null)
			{
				var (elementId, offsetIndex) = _cursorPos.Value;
				var renderingInfo = _elementRenderingInfo[elementId];
				var element = renderingInfo.Element;

				var yPos = element.Position.Y + renderingInfo.YOffset;
				var xPos = element.Position.X;
				if(element is VideoTextElement text)
				{
					xPos += text.GetOffsetWidth(offsetIndex);
				}

				destination.DrawLine(
					matrix.MapPoint(new SKPoint(xPos, yPos)), 
					matrix.MapPoint(new SKPoint(xPos, yPos + _lineHeight)), 
					_cursorPaint);
			}
		}

		public void UpdateCursorPosition(SKPoint point)
		{
			if(_elementsToDraw.Length == 0)
			{
				_cursorPos = null;
				return;
			}

			var elementId = FindElementAtPosition(point.X, point.Y);
			if(elementId == -1)
			{
				_cursorPos = (_elementsToDraw[0], 0);
				return;
			}

			var renderingInfo = _elementRenderingInfo[elementId];

			if(renderingInfo.Element is VideoTextElement text)
			{
				_cursorPos = (elementId, text.GetCharOffset(point.X - text.Position.X, point.Y - text.Position.Y - renderingInfo.YOffset));
			}
			else
			{
				_cursorPos = (elementId, 0);
			}
		}

		private int FindElementAtPosition(float x, float y)
		{
			int lastElem = -1;
			foreach(var elementId in _elementsToDraw)
			{
				var elem = _elementRenderingInfo[elementId].Element;
				var yOffset = _elementRenderingInfo[elementId].YOffset;
				var yPos = elem.Position.Y + yOffset;
				var height = elem.Size.Height;
				if(y >= yPos && y < yPos + height)
				{
					// we're in this element
					return elementId;
				}

				if(y < yPos)
				{
					// we're before this element, but closer to it than any other element
					return lastElem == -1 ? _elementsToDraw.First() : lastElem;
				}

				lastElem = elementId;
			}

			return _elementsToDraw.LastOrDefault();
		}

		private void UpdateElementPositions(VideoContext context, IVideoElement[] elements)
		{
			var nextId = elements.Max(e => e.Id) + 1;
			var yOffset = 0.0f;
			var paraHeight = 0.0f;
			var lastParagraphId = -1;
			var lineCount = 0;
			_elementRenderingInfo.Clear();
			var elemsToDraw = new List<int>();
			foreach (var elem in elements)
			{
				// add offset between paragraphs
				if (elem.ParagraphId != lastParagraphId)
				{
					// add an extra line break
					yOffset += paraHeight + context.Style.LineHeight;
					lineCount++;
					
					lastParagraphId = elem.ParagraphId;
					paraHeight = 0.0f;
				}

				elemsToDraw.Add(elem.Id);
				_elementRenderingInfo.Add(elem.Id, new ElementRenderingInfo(elem, yOffset));

				paraHeight += elem.Size.Height;
				_lineHeight = Math.Max(_lineHeight, elem.Size.Height);
				lineCount++;
			}

			// TODO: why do we need to add padding? what's going on?
			// it seems like the offset between lines changes at different sizes?
			Height = lineCount * (context.Style.LineHeight + 2.0f);
			_elementsToDraw = elemsToDraw.OrderBy(i => _elementRenderingInfo[i].YOffset + _elementRenderingInfo[i].Element.Position.Y).ToArray();
		}

		private class ElementRenderingInfo
		{
			public float YOffset;
			public IVideoElement Element;

			public ElementRenderingInfo(IVideoElement element, float yOffset)
			{
				YOffset = yOffset;
				Element = element;
			}
		}
	}
}
