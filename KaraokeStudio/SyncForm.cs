using KaraokeLib.Config;
using KaraokeLib.Lyrics;
using KaraokeLib.Util;
using KaraokeLib.Video;
using KaraokeLib.Video.Elements;
using KaraokeStudio.LyricsEditor;
using KaraokeStudio.Video;
using SkiaSharp;
using System.Data;
using System.Windows.Controls;

namespace KaraokeStudio
{
	public partial class SyncForm : Form
	{
		private KaraokeProject? _currentProject;
		private LyricsTrack? _lyricsTrack;

		public bool IsDirty { get; private set; } = false;

		public event Action<LyricsTrack>? OnSyncDataApplied;

		public SyncForm()
		{
			InitializeComponent();
		}

		internal void Open(KaraokeProject project, LyricsTrack track)
		{
			_currentProject = project;
			_lyricsTrack = track;
			video.SetVideoGenerator(new SyncFormVideoGenerator(project, track));
			video.OnProjectChanged(project);

			IsDirty = false;

			UpdateButtons();
			UpdateText();

			Show();
		}

		internal void OnProjectEventsChanged(KaraokeProject? project)
		{
			if (_lyricsTrack == null || project == null || !Visible)
			{
				return;
			}

			if (project != _currentProject)
			{
				throw new InvalidOperationException("SyncForm shouldn't be receiving OnProjectEventsChanged from a different project than it was opened for");
			}

			var trackMatch = project.Tracks.Where(t => t.Id == _lyricsTrack.Id).FirstOrDefault();
			// track must've been deleted
			if (trackMatch == null)
			{
				Hide();
				return;
			}

			_lyricsTrack = trackMatch;
			UpdateText();
			UpdateButtons();
		}

		internal bool OnProjectWillChange()
		{
			if (!IsDirty)
			{
				return true;
			}

			var result = MessageBox.Show("You have unsaved sync data! Do you want to apply before closing?", "Warning", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Warning);
			if (result == DialogResult.Yes)
			{
				ApplyChanges();
				return true;
			}
			else if (result == DialogResult.No)
			{
				return true;
			}

			return false;
		}

		private void ApplyChanges()
		{
			IsDirty = false;
			if (_lyricsTrack == null)
			{
				return;
			}
			OnSyncDataApplied?.Invoke(_lyricsTrack);
		}

		private void UpdateButtons()
		{
			undoLineButton.Enabled = false;
			undoWordButton.Enabled = false;
			syncButton.Enabled = _lyricsTrack != null && _lyricsTrack.Events.Any();
			breakButton.Enabled = false;
		}

		private void UpdateText()
		{
			if (_lyricsTrack == null)
			{
				lyricsBox.Text = "";
				return;
			}

			lyricsBox.Text = LyricsEditorText.CreateString(LyricsEditorText.CreateElements(_lyricsTrack.Events)).Text.Replace("\n", Environment.NewLine);
		}

		private void HandleSync()
		{

		}

		private void HandleBreak()
		{

		}

		private void HandleRewindWord()
		{

		}

		private void HandleRewindLine()
		{

		}

		private void SyncForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !OnProjectWillChange();
		}

		private void SyncForm_FormClosed(object sender, FormClosedEventArgs e)
		{
			video.Pause();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(Focused && keyData == Keys.Space)
			{
				HandleSync();
				return true;
			}
			else if(Focused && keyData == Keys.Control)
			{
				HandleBreak();
				return true;
			}
			else if(Focused && keyData == Keys.Back)
			{
				HandleRewindWord();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	internal class SyncFormVideoGenerator : IVideoGenerator
	{
		private bool _isElementsStale = true;
		private LyricsTrack _currentTrack;
		private VideoContext? _context;
		private VideoStyle? _style;
		private KaraokeConfig? _config;
		private SKMatrix? _matrix;
		private IVideoElement[] _elements;

		public SyncFormVideoGenerator(KaraokeProject project, LyricsTrack currentTrack)
		{
			_currentTrack = currentTrack;
			_elements = new IVideoElement[0];
		}

		public void Invalidate()
		{
			_isElementsStale = true;
		}

		public void Render(KaraokeProject project, VideoTimecode timecode, SKSurface surface)
		{
			surface.Canvas.Clear();

			if (_currentTrack == null || _context == null || _config == null || _style == null)
			{
				return;
			}

			if (_isElementsStale || _matrix == null)
			{
				_isElementsStale = false;
				var layoutState = new VideoLayoutState();
				var textElements = LyricsEditorText.CreateElements(_currentTrack.Events);
				var lineElements = new List<LyricsEditorTextElement>();
				var line = 0;
				var nextElementId = 0;
				var videoElements = new List<IVideoElement>();
				foreach (var elem in textElements)
				{
					if ((elem.Type == LyricsEventType.LineBreak || elem.Type == LyricsEventType.ParagraphBreak) && lineElements.Any())
					{
						videoElements.Add(new VideoTextElement(_context, layoutState, lineElements.SelectMany(e => e.Events), line * _style.LineHeight, nextElementId++, 0));
						lineElements.Clear();
					}

					switch(elem.Type)
					{
						case LyricsEventType.LineBreak:
							line++;
							break;
						case LyricsEventType.ParagraphBreak:
							line += 2;
							break;
						case LyricsEventType.Lyric:
							lineElements.Add(elem);
							break;
						default:
							throw new NotSupportedException($"Unknown LyricsEventType {elem.Type}");
					}
				}

				if (lineElements.Any())
				{
					videoElements.Add(new VideoTextElement(_context, layoutState, lineElements.SelectMany(e => e.Events), line * _style.LineHeight, nextElementId++, 0));
				}

				var widestLine = videoElements.Select(v => v.Size.Width).Max();
				var scaleFactor = _style.GetSafeArea(_config.VideoSize).Width / widestLine;

				_matrix = SKMatrix.CreateScale(scaleFactor, scaleFactor, 0, 0);
				_elements = videoElements.OrderBy(e => e.StartTimecode.GetTimeSeconds()).ToArray();
			}

			var activeElement = GetActiveElement(timecode.ToSeconds());
			var startYPos = activeElement != null ? activeElement.Position.Y : 0;
			var centerYPos = _config.VideoSize.Height / 2;
			
			var canvas = surface.Canvas;
			var savePoint = canvas.Save();

			canvas.DrawRect(0, 0, _config.VideoSize.Width, _config.VideoSize.Height, _style.BackgroundPaint);

			canvas.SetMatrix(_matrix ?? SKMatrix.Identity);
			canvas.Translate(0, centerYPos - startYPos);

			foreach(var elem in _elements)
			{
				elem.Render(_context, canvas, timecode.ToSeconds());
			}

			canvas.RestoreToCount(savePoint);
		}

		public void UpdateContext(KaraokeProject project, Size videoSize)
		{
			_config = new KaraokeConfig()
			{
				VideoSize = videoSize,
				BackgroundColor = VisualStyle.NeutralDarkColor,
				NormalColor = VisualStyle.NeutralLightColor,
				HighlightColor = VisualStyle.PositiveColor,
				HorizontalAlign = KaraokeLib.Config.HorizontalAlignment.Left,
				VerticalAlign = VerticalAlignment.Top,
				Font = new KFont()
				{
					Family = "Open Sans",
					Size = 36
				},
				StrokeWidth = 0
			};

			_style = new VideoStyle(_config);
			_context = new VideoContext(_style, _config, new VideoTimecode(project.Length.TotalSeconds, project.Config.FrameRate));
			_isElementsStale = true;
		}

		private IVideoElement GetActiveElement(double videoPos)
		{
			for(var i = 0; i < _elements.Length; i++)
			{
				var elem = _elements[i];
				if(videoPos < elem.StartTimecode.GetTimeSeconds())
				{
					return _elements[i > 0 ? i - 1 : i];
				}
				else if(videoPos < elem.EndTimecode.GetTimeSeconds())
				{
					return _elements[i];
				}
			}

			return _elements[_elements.Length - 1];
		}
	}
}
