using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
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
		private KaraokeTrack? _lyricsTrack;
		private KaraokeEvent[] _events = new KaraokeEvent[0];
		private int _eventIndex = -1;
		private int _lastEventIndex = -1;
		private int _firstLyricIndex = -1;
		private bool _finishedLastSyllable = true;
		private SyncFormVideoGenerator? _generator;

		public double CurrentTime
		{
			get
			{
				if (_eventIndex == -1)
				{
					return 0;
				}

				if (_eventIndex >= _events.Length)
				{
					return _events[_events.Length - 1].EndTimeSeconds;
				}

				return _events[_eventIndex].StartTimeSeconds;
			}
		}

		public bool IsDirty { get; private set; } = false;

		public event Action<KaraokeTrack>? OnSyncDataApplied;

		public SyncForm()
		{
			InitializeComponent();
		}

		internal void Open(KaraokeProject project, KaraokeTrack track)
		{
			_currentProject = project;
			_lyricsTrack = track;
			LoadEvents(track);
			_generator = new SyncFormVideoGenerator(this, _events);

			video.SetVideoGenerator(_generator);
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
			var existingEvents = new Dictionary<int, KaraokeEvent>();
			foreach (var ev in _events)
			{
				existingEvents[ev.Id] = ev;
			}

			var currentTime = CurrentTime;
			var newIndex = -1;

			// port over previous event changes
			var events = new List<KaraokeEvent>();
			foreach (var ev in _lyricsTrack.Events.OrderBy(ev => ev.StartTimeSeconds))
			{
				var newEv = new KaraokeEvent(ev);
				if (newIndex == -1 && newEv.StartTimeSeconds > currentTime)
				{
					newIndex = events.Count;
				}

				if (existingEvents.ContainsKey(ev.Id))
				{
					newEv.SetTiming(
						existingEvents[ev.Id].StartTime,
						existingEvents[ev.Id].EndTime);
				}
				events.Add(newEv);
			}

			_events = events.ToArray();
			_eventIndex = newIndex == -1 && _events.Length > 0 ? 0 : newIndex;
			if (_eventIndex > -1)
			{
				while (_events[_eventIndex].Type != KaraokeEventType.Lyric) { _eventIndex++; }
			}
			_generator?.SetEvents(events);

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
				IsDirty = false;
				return true;
			}

			return false;
		}

		private void LoadEvents(KaraokeTrack track)
		{
			_events = track.Events.Select(ev => new KaraokeEvent(ev)).ToArray();
			for (var i = 0; i < _events.Length; i++)
			{
				if (_events[i].Type == KaraokeEventType.Lyric)
				{
					_firstLyricIndex = i;
					break;
				}
			}
			_eventIndex = _events.Any() ? 0 : -1;
			if (_eventIndex > -1)
			{
				while (_events[_eventIndex].Type != KaraokeEventType.Lyric) { _eventIndex++; }
			}

			_finishedLastSyllable = true;
		}

		private void ApplyChanges()
		{
			IsDirty = false;
			if (_lyricsTrack == null)
			{
				return;
			}

			var eventDict = new Dictionary<int, KaraokeEvent>();
			foreach (var ev in _events)
			{
				eventDict[ev.Id] = ev;
			}

			foreach (var ev in _lyricsTrack.Events)
			{
				if(eventDict.ContainsKey(ev.Id))
				{
					ev.SetTiming(eventDict[ev.Id].StartTime, eventDict[ev.Id].EndTime);
				}
			}

			OnSyncDataApplied?.Invoke(_lyricsTrack);
		}

		private void UpdateButtons()
		{
			undoLineButton.Enabled = _eventIndex > _firstLyricIndex;
			undoWordButton.Enabled = _eventIndex > _firstLyricIndex;
			syncButton.Enabled = _lyricsTrack != null && _lyricsTrack.Events.Any() && _eventIndex < _events.Length;
			breakButton.Enabled = !_finishedLastSyllable;
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
			if (_eventIndex == -1 || _currentProject == null)
			{
				return;
			}

			var time = _currentProject.PlaybackState.Position;

			if (!_finishedLastSyllable && _lastEventIndex > -1)
			{
				_events[_lastEventIndex].SetTiming(_events[_lastEventIndex].StartTime, new TimeSpanTimecode(time));
			}

			var length = _events[_eventIndex].LengthSeconds;
			// move event to new start time instead of just moving its start point
			_events[_eventIndex].SetTiming(new TimeSpanTimecode(time), new TimeSpanTimecode(time + length));

			_lastEventIndex = _eventIndex;
			_finishedLastSyllable = false;

			// skip non-lyric events
			while (_events[++_eventIndex].Type != KaraokeEventType.Lyric) { }

			IsDirty = true;
			UpdateButtons();
			video.ForceRerender();
		}

		private void HandleBreak()
		{
			if (_lastEventIndex == -1 || _currentProject == null || _finishedLastSyllable)
			{
				return;
			}

			var time = _currentProject.PlaybackState.Position;
			_events[_lastEventIndex].SetTiming(_events[_lastEventIndex].StartTime, new TimeSpanTimecode(time));
			_finishedLastSyllable = true;
			IsDirty = true;
			UpdateButtons();
			video.ForceRerender();
		}

		private void HandleRewindWord()
		{
			if (_eventIndex > 0)
			{
				while (_eventIndex > _firstLyricIndex && _events[--_eventIndex].Type != KaraokeEventType.Lyric) { }

				_currentProject?.PlaybackState.SeekAbsolute(_events[_eventIndex].StartTimeSeconds);
				_finishedLastSyllable = true;
				UpdateButtons();
				video.ForceRerender();
			}
		}

		private void HandleRewindLine()
		{
			// skip until we hit line break or 0
			while (
				_eventIndex > _firstLyricIndex &&
				_events[_eventIndex].Type != KaraokeEventType.LineBreak &&
				_events[_eventIndex].Type != KaraokeEventType.ParagraphBreak)
			{
				_eventIndex--;
			}

			if (_eventIndex > -1 && _events[_eventIndex].Type != KaraokeEventType.Lyric)
			{
				while (_eventIndex > _firstLyricIndex && _events[--_eventIndex].Type != KaraokeEventType.Lyric) { }
			}

			_currentProject?.PlaybackState.SeekAbsolute(_events[_eventIndex].StartTimeSeconds);

			_finishedLastSyllable = true;
			UpdateButtons();
			video.ForceRerender();
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			var isActive = ActiveForm == this;
			if (isActive && keyData == Keys.Z)
			{
				HandleSync();
				return true;
			}
			else if (isActive && keyData == Keys.X)
			{
				HandleBreak();
				return true;
			}
			else if (isActive && keyData == Keys.Control)
			{
				HandleRewindWord();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void SyncForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !OnProjectWillChange();
		}

		private void undoLineButton_Click(object sender, EventArgs e) => HandleRewindLine();

		private void undoWordButton_Click(object sender, EventArgs e) => HandleRewindWord();

		private void syncButton_Click(object sender, EventArgs e) => HandleSync();

		private void breakButton_Click(object sender, EventArgs e) => HandleBreak();

		private void applyButton_Click(object sender, EventArgs e)
		{
			ApplyChanges();
		}

		private void revertButton_Click(object sender, EventArgs e)
		{
			if(_lyricsTrack == null)
			{
				return;
			}

			LoadEvents(_lyricsTrack);
			_generator?.SetEvents(_events);
			UpdateButtons();
			UpdateText();
		}

		private void cancelButton_Click(object sender, EventArgs e)
		{
			Hide();
		}
	}

	internal class SyncFormVideoGenerator : IVideoGenerator
	{
		private bool _isElementsStale = true;
		private KaraokeEvent[] _events;
		private VideoContext? _context;
		private VideoStyle? _style;
		private KaraokeConfig? _config;
		private SKMatrix? _matrix;
		private IVideoElement[] _elements;
		private SyncForm _form;

		public SyncFormVideoGenerator(SyncForm form, IEnumerable<KaraokeEvent> events)
		{
			_form = form;
			_events = events.ToArray();
			_elements = new IVideoElement[0];
		}

		public void SetEvents(IEnumerable<KaraokeEvent> events)
		{
			_events = events.ToArray();
			_isElementsStale = true;
		}

		public void Invalidate()
		{
			_isElementsStale = true;
		}

		public void Render(KaraokeProject project, VideoTimecode timecode, SKSurface surface)
		{
			surface.Canvas.Clear();

			if (_context == null || _config == null || _style == null)
			{
				return;
			}

			if (_isElementsStale || _matrix == null)
			{
				_isElementsStale = false;
				var layoutState = new VideoLayoutState();
				var textElements = LyricsEditorText.CreateElements(_events);
				var lineElements = new List<LyricsEditorTextElement>();
				var line = 0;
				var nextElementId = 0;
				var videoElements = new List<IVideoElement>();
				foreach (var elem in textElements)
				{
					if ((elem.Type == KaraokeEventType.LineBreak || elem.Type == KaraokeEventType.ParagraphBreak) && lineElements.Any())
					{
						var lineEvents = lineElements.SelectMany(e => e.Events).ToArray();
						HyphenateEvents(layoutState, lineEvents);
						videoElements.Add(new VideoTextElement(_context, layoutState, lineEvents, line * _style.LineHeight, nextElementId++, 0));
						lineElements.Clear();
					}

					switch (elem.Type)
					{
						case KaraokeEventType.LineBreak:
							line++;
							break;
						case KaraokeEventType.ParagraphBreak:
							line += 2;
							break;
						case KaraokeEventType.Lyric:
							lineElements.Add(elem);
							break;
						default:
							throw new NotSupportedException($"Unknown KaraokeEventType {elem.Type}");
					}
				}

				if (lineElements.Any())
				{
					var lineEvents = lineElements.SelectMany(e => e.Events).ToArray();
					HyphenateEvents(layoutState, lineEvents);
					videoElements.Add(new VideoTextElement(_context, layoutState, lineEvents, line * _style.LineHeight, nextElementId++, 0));
				}

				var widestLine = videoElements.Select(v => v.Size.Width).Max();
				var scaleFactor = _style.GetSafeArea(_config.VideoSize).Width / widestLine;

				_matrix = SKMatrix.CreateScale(scaleFactor, scaleFactor, 0, 0);
				_elements = videoElements.OrderBy(e => e.StartTimecode.GetTimeSeconds()).ToArray();
			}

			var activeElement = GetActiveElement(_form.CurrentTime);
			var startYPos = activeElement != null ? activeElement.Position.Y : 0;
			var centerYPos = _config.VideoSize.Height * 0.4f;

			var canvas = surface.Canvas;
			var savePoint = canvas.Save();

			canvas.DrawRect(0, 0, _config.VideoSize.Width, _config.VideoSize.Height, _style.BackgroundPaint);

			canvas.SetMatrix(_matrix ?? SKMatrix.Identity);
			canvas.Translate(0, centerYPos - startYPos);

			foreach (var elem in _elements)
			{
				elem.Render(_context, canvas, _form.CurrentTime);
			}

			canvas.RestoreToCount(savePoint);
		}

		private void HyphenateEvents(VideoLayoutState layoutState, KaraokeEvent[] events)
		{
			var lastId = -1;
			for(var i = 0; i < events.Length; i++)
			{
				if (events[i].LinkedId != -1 && events[i].LinkedId == lastId)
				{
					layoutState.SetHyphenated(events[i - 1]);
				}

				lastId = events[i].Id;
			}
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
			for (var i = 0; i < _elements.Length; i++)
			{
				var elem = _elements[i];
				if (videoPos < elem.StartTimecode.GetTimeSeconds())
				{
					return _elements[i > 0 ? i - 1 : i];
				}
				else if (videoPos < elem.EndTimecode.GetTimeSeconds())
				{
					return _elements[i];
				}
			}

			return _elements[_elements.Length - 1];
		}
	}
}
