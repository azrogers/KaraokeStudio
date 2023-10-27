using KaraokeLib.Lyrics;
using KaraokeStudio.LyricsEditor;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaraokeStudio
{
	public partial class SynchronizedTextBox : TextBox
	{
		private double _currentPosition = 0;
		private LyricsEditorTextElement[] _elements;
		private LyricsEditorTextResult _textResult;

		public SynchronizedTextBox()
		{
			InitializeComponent();

			_elements = new LyricsEditorTextElement[0];
			_textResult = new LyricsEditorTextResult("", new Dictionary<int, int>());
		}

		public void Initialize(IEnumerable<LyricsEvent> events)
		{
			_elements = LyricsEditorText.CreateElements(events).OrderBy(e => e.StartTime).ToArray();
			_textResult = LyricsEditorText.CreateString(_elements);
			_currentPosition = 0;
		}

		public void UpdatePosition(double position)
		{
			_currentPosition = position;
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			base.OnPaint(e);
		}

		private int GetCurrentCharOffset()
		{
			return 0;
		}

		private int GetCurrentElementIndex()
		{
			for (var i = 0; i < _elements.Length; i++)
			{
				if (_currentPosition < _elements[i].StartTime)
				{
					return i > 0 ? i - 1 : i;
				}

				if(_currentPosition < _elements[i].EndTime)
				{
					return i;
				}
			}

			return _elements.Length - 1;
		}
	}
}
