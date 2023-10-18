using SkiaSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ScintillaNET;
using KaraokeLib.Util;
using KaraokeLib.Video.Elements;

namespace KaraokeStudio.LyricsEditor
{
	public partial class LyricsEditorControl : UserControl
	{
		private const int LYRIC_STYLE = 11;
		private const int LYRIC_HIGHLIGHT_STYLE = 12;

		private KaraokeProject? _project;
		private LyricsEditorTextElement[] _textElements = new LyricsEditorTextElement[0];
		private LyricsEditorTextResult? _textResult;
		private Scintilla _scintilla;
		private int _previousHighlightIndex = 0;

		public LyricsEditorControl()
		{
			InitializeComponent();

			BackColor = Color.Black;

			_scintilla = new Scintilla();
			_scintilla.Dock = DockStyle.Fill;
			_scintilla.Lexer = Lexer.Container;
			_scintilla.StyleNeeded += _scintilla_StyleNeeded;
			_scintilla.Margins[0].Width = 1;

			_scintilla.Styles[Style.Default].BackColor = Color.Black;
			_scintilla.Styles[Style.Default].ForeColor = Color.White;

			_scintilla.CaretForeColor = Color.White;

			_scintilla.Styles[LYRIC_STYLE].Font = "Open Sans";
			_scintilla.Styles[LYRIC_STYLE].Size = 30;
			_scintilla.Styles[LYRIC_STYLE].ForeColor = Color.White;
			_scintilla.Styles[LYRIC_STYLE].BackColor = Color.Black;

			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].Font = "Open Sans";
			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].Size = 30;
			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].ForeColor = VisualStyle.HighlightColor;
			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].BackColor = Color.Black;

			_scintilla.WrapMode = WrapMode.Word;

			Controls.Add(_scintilla);
		}

		internal void OnProjectChanged(KaraokeProject? project)
		{
			_project = project;
			UpdateTextBox();
		}

		internal void OnPositionChanged(double newPosition)
		{
			var charIndex = PositionToCharIndex(newPosition);
			RestyleArea(charIndex, _previousHighlightIndex, charIndex);

			_previousHighlightIndex = charIndex;
		}

		private void UpdateTextBox()
		{
			UpdateTextElements();
			_textResult = LyricsEditorText.CreateString(_textElements);
			_scintilla.Text = _textResult.Text;
		}

		private void UpdateTextElements()
		{
			var track = _project?.Tracks.Where(t => t.Type == KaraokeLib.Lyrics.LyricsTrackType.Lyrics).FirstOrDefault();
			if (track == null)
			{
				_textElements = new LyricsEditorTextElement[0];
				return;
			}

			_textElements = LyricsEditorText.CreateElements(track.Events);
		}

		private LyricsEditorTextElement? CharIndexToElement(int index)
		{
			if(_textElements.Length == 0 || _textResult == null)
			{
				return null;
			}

			var lastElemId = -1;

			foreach(var (elemId, offset) in _textResult.EventOffsets)
			{
				if (offset > index)
				{
					return lastElemId == -1 ? _textElements[0] : _textElements.Where(e => e.Id == elemId).First();
				}

				lastElemId = elemId;
			}

			return _textElements.Last();
		}

		private int PositionToCharIndex(double position)
		{
			if(_textResult == null)
			{
				return 0;
			}

			foreach(var elem in _textElements.OrderBy(e => e.StartTime))
			{
				if(position < elem.StartTime)
				{
					return _textResult.EventOffsets[elem.Id];
				}
				else if(position >= elem.StartTime && position < elem.EndTime)
				{
					var start = _textResult.EventOffsets[elem.Id];
					var end = start + elem.ToString().Length;
					var normalizedPos = elem.GetNormalizedPosition(position);
					return (int)((end - start) * normalizedPos);
				}
			}

			return _scintilla.Text.Length - 1;
		}

		private void RestyleArea(int highlightIndex, int startIndex, int endIndex)
		{
			if(highlightIndex < endIndex)
			{
				var start = Math.Max(highlightIndex, startIndex);
				_scintilla.StartStyling(start);
				_scintilla.SetStyling(endIndex - start, LYRIC_STYLE);
			}

			if(highlightIndex > startIndex)
			{
				var end = Math.Min(highlightIndex, endIndex);
				_scintilla.StartStyling(startIndex);
				_scintilla.SetStyling(end - startIndex, LYRIC_HIGHLIGHT_STYLE);
			}
		}

		private void _scintilla_StyleNeeded(object? sender, StyleNeededEventArgs e)
		{
			var start = _scintilla.GetEndStyled();
			var end = e.Position;

			RestyleArea(_previousHighlightIndex, start, end);
		}

		private void skiaControl_MouseDown(object sender, MouseEventArgs e)
		{
		}

		private void LyricsEditorControl_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == '\r')
			{

			}
		}
	}
}
