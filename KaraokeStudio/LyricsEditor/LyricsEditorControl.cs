﻿using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Project;
using ScintillaNET;
using System.Data;

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

		private UpdateDispatcher.Handle _projectHandle;
		private UpdateDispatcher.Handle _eventsUpdateHandle;
		private UpdateDispatcher.Handle _tracksUpdateHandle;

		public LyricsEditorControl()
		{
			InitializeComponent();

			Disposed += OnDispose;

			BackColor = Color.Black;

			_scintilla = new Scintilla();
			_scintilla.Dock = DockStyle.Fill;
			_scintilla.Lexer = Lexer.Container;
			_scintilla.StyleNeeded += _scintilla_StyleNeeded;
			_scintilla.Margins[0].Width = 1;

			_scintilla.Styles[Style.Default].BackColor = VisualStyle.NeutralDarkColor;
			_scintilla.Styles[Style.Default].ForeColor = Color.White;

			_scintilla.CaretForeColor = Color.White;

			_scintilla.Styles[LYRIC_STYLE].Font = "Open Sans";
			_scintilla.Styles[LYRIC_STYLE].Size = 30;
			_scintilla.Styles[LYRIC_STYLE].ForeColor = Color.White;
			_scintilla.Styles[LYRIC_STYLE].BackColor = VisualStyle.NeutralDarkColor;

			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].Font = "Open Sans";
			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].Size = 30;
			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].ForeColor = VisualStyle.HighlightColor;
			_scintilla.Styles[LYRIC_HIGHLIGHT_STYLE].BackColor = VisualStyle.NeutralDarkColor;

			_scintilla.WrapMode = WrapMode.Word;

			Controls.Add(_scintilla);

			_eventsUpdateHandle = UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				UpdateTextBox();
			});

			_tracksUpdateHandle = UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				UpdateTextBox();
			});

			_projectHandle = UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				if (_project != null)
				{
					_project.PlaybackState.OnPositionChanged -= OnPositionChanged;
				}

				if (update.Project != null)
				{
					update.Project.PlaybackState.OnPositionChanged += OnPositionChanged;
				}

				_project = update.Project;
				UpdateTextBox();
			});
		}

		private void OnDispose(object? sender, EventArgs e)
		{
			if (_project != null)
			{
				_project.PlaybackState.OnPositionChanged -= OnPositionChanged;
			}

			_eventsUpdateHandle.Release();
			_tracksUpdateHandle.Release();
			_projectHandle.Release();
		}

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			UpdateTextBox();
		}

		private void OnPositionChanged(double newPosition)
		{
			var charIndex = _textResult?.PositionToCharIndex(_textElements, newPosition) ?? 0;
			RestyleArea(charIndex, _previousHighlightIndex, charIndex);

			_previousHighlightIndex = charIndex;
		}

		private void UpdateTextBox()
		{
			var offset = _scintilla.CurrentPosition;
			var elem = CharIndexToElement(offset);
			var previousPosition = 0.0;
			if (elem != null && _textResult != null)
			{
				previousPosition = elem.CharIndexToPosition(offset - _textResult.EventOffsets[elem.Id]);
			}

			UpdateTextElements();
			_textResult = LyricsEditorText.CreateString(_textElements);
			_scintilla.Text = _textResult.Text;

			_scintilla.AnchorPosition = _scintilla.CurrentPosition = _textResult?.PositionToCharIndex(_textElements, previousPosition) ?? 0;
			_scintilla.ScrollCaret();
		}

		private void UpdateTextElements()
		{
			var track = _project?.Tracks.Where(t => t.Type == KaraokeTrackType.Lyrics).FirstOrDefault();
			if (track == null)
			{
				_textElements = new LyricsEditorTextElement[0];
				return;
			}

			_textElements = LyricsEditorText.CreateElements(track.Events);
		}

		private LyricsEditorTextElement? CharIndexToElement(int index)
		{
			if (_textElements.Length == 0 || _textResult == null)
			{
				return null;
			}

			var lastElemId = -1;

			foreach (var (elemId, offset) in _textResult.EventOffsets)
			{
				if (offset > index)
				{
					return lastElemId == -1 ? _textElements[0] : _textElements.Where(e => e.Id == elemId).First();
				}

				lastElemId = elemId;
			}

			return _textElements.Last();
		}

		private void RestyleArea(int highlightIndex, int startIndex, int endIndex)
		{
			_scintilla.StartStyling(startIndex);
			if (highlightIndex > startIndex)
			{
				var end = Math.Min(highlightIndex, endIndex);
				_scintilla.SetStyling(end - startIndex, LYRIC_HIGHLIGHT_STYLE);
			}

			if (highlightIndex < endIndex)
			{
				var start = Math.Max(highlightIndex, startIndex);
				_scintilla.SetStyling(endIndex - start, LYRIC_STYLE);
			}
		}

		private void UpdateLyrics(LyricsEditorTextElement[] prevElements)
		{
			var track = _project?.Tracks.FirstOrDefault(t => t.Type == KaraokeTrackType.Lyrics);
			if (track == null)
			{
				return;
			}

			var newEvents = LyricsEditorText.GetEventsFromString(_scintilla.Text, prevElements, _project?.Length.TotalSeconds ?? 0).ToArray();
			CommandDispatcher.Dispatch(new SetTrackEventsCommand(track, newEvents, "Update lyrics"));
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

		private void updateLyricsButton_ButtonClick(object sender, EventArgs e)
		{
			UpdateLyrics(_textElements);
		}

		private void overwriteLyricsToolStripMenuItem_Click(object sender, EventArgs e)
		{
			UpdateLyrics(new LyricsEditorTextElement[0]);
		}
	}
}
