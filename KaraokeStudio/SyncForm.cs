﻿using KaraokeLib.Config;
using KaraokeLib.Events;
using KaraokeLib.Tracks;
using KaraokeLib.Util;
using KaraokeLib.Video;
using KaraokeLib.Video.Elements;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.LyricsEditor;
using KaraokeStudio.Project;
using KaraokeStudio.Video;
using SkiaSharp;
using System.Data;

namespace KaraokeStudio
{
	public partial class SyncForm : Form
	{
		private KaraokeProject? _currentProject;
		private KaraokeTrack? _lyricsTrack;
		private KaraokeEvent[] _events = new KaraokeEvent[0];
		private KaraokeEvent[] _originalEvents = new KaraokeEvent[0];
		private int _eventIndex = -1;
		private int _lastEventIndex = -1;
		private bool _finishedLastSyllable = true;
		private SyncFormVideoGenerator? _generator;
		private List<KaraokeEvent> _eventsNeedRepositioning = new List<KaraokeEvent>();

		private List<Action> _onDispose = new List<Action>();

		private Stack<(int EventIndex, double VideoPosition)> _undoContext = new Stack<(int EventIndex, double VideoPosition)>();
		private Stack<(int EventIndex, double VideoPosition)> _undoLineContext = new Stack<(int EventIndex, double VideoPosition)>();

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
					return _originalEvents[_events.Length - 1].EndTimeSeconds;
				}

				return _originalEvents[_eventIndex].StartTimeSeconds;
			}
		}

		public bool IsDirty { get; private set; } = false;

		public SyncForm()
		{
			InitializeComponent();

			_onDispose.Add(UpdateDispatcher.RegisterHandler<ProjectUpdate>(update =>
			{
				_currentProject = update.Project;
				Hide();
			}).Release);

			_onDispose.Add(UpdateDispatcher.RegisterHandler<EventsUpdate>(update =>
			{
				// TODO: actually use information in update to tell which events to update
				OnProjectEventsChanged();
			}).Release);

			_onDispose.Add(UpdateDispatcher.RegisterHandler<TracksUpdate>(update =>
			{
				// track we're syncing was deleted
				if (update.Type == TracksUpdate.UpdateType.Removed && update.TrackIds.Any(t => t == _lyricsTrack?.Id))
				{
					Hide();
				}
			}).Release);
		}

		internal void Open(KaraokeProject project, KaraokeTrack track)
		{
			_undoContext.Clear();
			_undoLineContext.Clear();
			_currentProject = project;
			_lyricsTrack = track;
			LoadEvents(track);
			_originalEvents = track.Events.ToArray();
			_generator = new SyncFormVideoGenerator(this, _originalEvents);

			video.SetVideoGenerator(_generator);
			video.OnProjectChanged(project);

			IsDirty = false;

			UpdateButtons();
			UpdateText();

			Show();
		}

		internal void OnProjectEventsChanged()
		{
			if (_lyricsTrack == null || _currentProject == null || !Visible)
			{
				return;
			}

			var trackMatch = _currentProject.Tracks.Where(t => t.Id == _lyricsTrack.Id).FirstOrDefault();
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
			_originalEvents = _lyricsTrack.Events.OrderBy(ev => ev.StartTimeSeconds).ToArray();
			_generator?.SetEvents(_originalEvents);

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

			CommandDispatcher.Dispatch(new SetEventTimingsCommand(new KaraokeTrack[] { _lyricsTrack }, _events));

			_undoContext.Clear();
			_undoLineContext.Clear();
			UpdateButtons();
			_originalEvents = _lyricsTrack.Events.ToArray();
			LoadEvents(_lyricsTrack);
			_generator?.SetEvents(_originalEvents);
			video.ForceRerender();
		}

		private void UpdateButtons()
		{
			undoLineButton.Enabled = _undoLineContext.Any();
			undoWordButton.Enabled = _undoContext.Any();
			syncButton.Enabled = _lyricsTrack != null && _lyricsTrack.Events.Any() && _eventIndex < _events.Length;
			breakButton.Enabled = !_finishedLastSyllable;
			revertButton.Enabled = !IsDirty;
			applyButton.Enabled = IsDirty;
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
			if (_eventIndex == -1 || _currentProject == null || _eventIndex >= _events.Length)
			{
				return;
			}

			var time = _currentProject.PlaybackState.Position;



			if (!_finishedLastSyllable && _lastEventIndex > -1)
			{
				_events[_lastEventIndex].SetTiming(_events[_lastEventIndex].StartTime, new TimeSpanTimecode(time));
			}

			var length = _events[_eventIndex].LengthSeconds;
			var endTime = time + length;

			// reposition events we skipped (like line breaks)
			if (_eventsNeedRepositioning.Any())
			{
				var reposStartTime = _events[_lastEventIndex].EndTimeSeconds;
				var origStartTime = _eventsNeedRepositioning.Min(e => e.StartTimeSeconds);
				var origEndTime = _eventsNeedRepositioning.Max(e => e.EndTimeSeconds);

				for (var i = 0; i < _eventsNeedRepositioning.Count; i++)
				{
					var evLength = 0.05;
					var start = reposStartTime;
					_eventsNeedRepositioning[i].SetTiming(new TimeSpanTimecode(start), new TimeSpanTimecode(start + evLength));
					reposStartTime += evLength;
				}

				_eventsNeedRepositioning.Clear();
			}

			// move event to new start time instead of just moving its start point
			_events[_eventIndex].SetTiming(new TimeSpanTimecode(time), new TimeSpanTimecode(endTime));

			_lastEventIndex = _eventIndex;
			_finishedLastSyllable = false;

			_undoContext.Push((_eventIndex, time));

			// skip non-lyric events
			var passedLineBreaks = false;
			var passedEvents = new List<KaraokeEvent>();
			while (++_eventIndex < _events.Length && _events[_eventIndex].Type != KaraokeEventType.Lyric)
			{
				if (_events[_eventIndex].Type == KaraokeEventType.LineBreak || _events[_eventIndex].Type == KaraokeEventType.ParagraphBreak)
				{
					passedLineBreaks = true;
				}

				// make sure we also reposition the events we skipped
				_eventsNeedRepositioning.Add(_events[_eventIndex]);
			}

			if (passedLineBreaks)
			{
				_undoLineContext.Push((_lastEventIndex, time));
			}

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
			if (_undoContext.Any())
			{
				var frame = _undoContext.Pop();
				_eventsNeedRepositioning.Clear();
				_eventIndex = frame.EventIndex;
				if (_currentProject?.PlaybackState.Position > frame.VideoPosition)
				{
					_currentProject?.PlaybackState.SeekAbsolute(frame.VideoPosition);
				}
				// undo line context no longer valid
				if (_undoLineContext.Any() && frame.EventIndex <= _undoLineContext.Peek().EventIndex)
				{
					_undoLineContext.Pop();
				}
				_finishedLastSyllable = true;
				UpdateButtons();
				video.ForceRerender();
			}
		}

		private void HandleRewindLine()
		{
			if (_undoLineContext.Any())
			{
				var frame = _undoLineContext.Pop();
				_eventsNeedRepositioning.Clear();
				_eventIndex = frame.EventIndex;
				// undo context no longer valid
				while (_undoContext.Any() && _undoContext.Peek().EventIndex >= _eventIndex) { _undoContext.Pop(); }
				if (_currentProject?.PlaybackState.Position > frame.VideoPosition)
				{
					_currentProject?.PlaybackState.SeekAbsolute(frame.VideoPosition);
				}
				_finishedLastSyllable = true;
				UpdateButtons();
				video.ForceRerender();
			}
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			var isActive = ContainsFocus;
			var lParam = msg.LParam;
			var previousKeyState = ((lParam >> 30) & 1) == 1;
			var repeatCount = (lParam & 0xffff);
			var isHotkey = isActive && !previousKeyState && repeatCount < 2;
			if (isHotkey && (keyData == Keys.Z || keyData == Keys.X))
			{
				HandleSync();
				return true;
			}
			else if (isHotkey && keyData == Keys.Space)
			{
				HandleBreak();
				return true;
			}
			else if (isHotkey && keyData == Keys.C)
			{
				HandleRewindWord();
				return true;
			}
			else if (isHotkey && keyData == Keys.A)
			{
				HandleRewindLine();
				return true;
			}

			return base.ProcessCmdKey(ref msg, keyData);
		}

		private void SyncForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			e.Cancel = !OnProjectWillChange();
			if (!e.Cancel)
			{
				foreach (var d in _onDispose)
				{
					d();
				}
			}
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
			if (_lyricsTrack == null)
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
			if (OnProjectWillChange())
			{
				Hide();
			}
		}
	}

	internal class SyncFormVideoGenerator : IVideoGenerator
	{
		private bool _isElementsStale = true;
		private KaraokeEvent[] _events;
		private VideoContext? _context;
		private VideoStyle? _style;
		private KaraokeConfig? _config;
		private float _scaleFactor = 1.0f;
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

			if (_context == null || _config == null || _style == null || _events.Length == 0)
			{
				return;
			}

			if (_isElementsStale)
			{
				_isElementsStale = false;
				var layoutState = new VideoLayoutState(_context);
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
				_scaleFactor = 1.0f;//_style.GetSafeArea(_config.VideoSize).Width / widestLine;

				_elements = videoElements.OrderBy(e => e.StartTimecode.GetTimeSeconds()).ToArray();
			}

			var activeElement = GetActiveElement(_form.CurrentTime);
			var startYPos = activeElement != null ? activeElement.Position.Y : 0;
			var centerYPos = _config.VideoSize.Height * 0.4f;

			var canvas = surface.Canvas;
			var savePoint = canvas.Save();

			canvas.DrawRect(0, 0, _config.VideoSize.Width, _config.VideoSize.Height, _style.BackgroundPaint);

			canvas.Translate(0, centerYPos - startYPos);
			canvas.Scale(_scaleFactor);

			foreach (var elem in _elements)
			{
				elem.Render(_context, canvas, _form.CurrentTime);
			}

			canvas.RestoreToCount(savePoint);
		}

		private void HyphenateEvents(VideoLayoutState layoutState, KaraokeEvent[] events)
		{
			var lastId = -1;
			for (var i = 0; i < events.Length; i++)
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

		public void Dispose()
		{

		}
	}
}
