using FontAwesome.Sharp;
using KaraokeLib.Tracks;
using KaraokeStudio.Commands;
using KaraokeStudio.Commands.Updates;
using KaraokeStudio.Config;
using KaraokeStudio.Project;
using KaraokeStudio.Util;

namespace KaraokeStudio.Timeline
{
	public partial class TrackHeaderControl : UserControl
	{
		public KaraokeTrack? Track
		{
			get => _track;
			set
			{
				_track = value;
				UpdateComponent();
			}
		}

		internal KaraokeProject? Project;

		private Pen _selectedTrackPen;
		private Brush _highlightBrush;
		private KaraokeTrack? _track;
		private List<ToolTip> _tooltips = new List<ToolTip>();
		private Dictionary<IconButton, EventHandler> _buttonOnClickHandlers = new Dictionary<IconButton, EventHandler>();
		private Dictionary<IconButton, Action<IconButton>> _buttonUpdateHandlers = new Dictionary<IconButton, Action<IconButton>>();
		private bool _selected = false;
		private UpdateDispatcher.Handle _trackSettingsHandle;

		public TrackHeaderControl()
		{
			InitializeComponent();
			Disposed += OnDispose;

			_selectedTrackPen = new Pen(Brushes.White, 3.0f);
			_highlightBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));

			_trackSettingsHandle = UpdateDispatcher.RegisterHandler<TrackSettingsUpdate>(update =>
			{
				foreach(var (button, callback) in _buttonUpdateHandlers)
				{
					callback(button);
				}
			});
		}

		private void OnDispose(object? sender, EventArgs e)
		{
			_trackSettingsHandle.Release();
		}

		public void SetSelected(bool selected)
		{
			_selected = selected;
			Invalidate();
		}

		private void UpdateComponent()
		{
			trackTitleLabel.Text = $"Track {Track?.Id ?? -1}";
			trackTypeLabel.Text = Utility.HumanizeCamelCase(Track?.Type.ToString() ?? "Unknown");
			BackColor = Track != null && VisualStyle.TrackColors.ContainsKey(Track.Type) ? VisualStyle.TrackColors[Track.Type] : Color.Black;

			UpdateButtons();
		}

		private void UpdateButtons()
		{
			// remove old event handlers
			foreach (var (button, handler) in _buttonOnClickHandlers)
			{
				button.Click -= handler;
			}

			_buttonOnClickHandlers.Clear();
			_buttonUpdateHandlers.Clear();
			trackButtonsContainer.Controls.Clear();
			foreach (var tooltip in _tooltips)
			{
				tooltip.RemoveAll();
				tooltip.Dispose();
			}

			_tooltips.Clear();

			if (Track == null)
			{
				return;
			}

			CreateButton("Track Properties", IconChar.Gear, (o, e) =>
			{
				CommandDispatcher.Dispatch(new OpenTrackSettingsCommand(Track));
			});

			if (Track.Type == KaraokeTrackType.Lyrics)
			{
				CreateButton("Sync Lyrics", IconChar.Music, (o, e) =>
				{
					if(Project != null)
					{
						CommandDispatcher.Dispatch(new OpenSyncFormCommand(Project, Track));
					}
				});
			}

			if (Track.Type == KaraokeTrackType.Audio)
			{
				var config = Track.GetTrackConfig<AudioTrackSettings>();
				var muteButton = CreateButton("Mute", IconChar.VolumeMute, (o, e) =>
				{
					var button = o as IconButton;
					if (button == null)
					{
						return;
					}

					var oldConfig = Track.GetTrackConfig<AudioTrackSettings>();
					var newConfig = (AudioTrackSettings)oldConfig.Copy();
					newConfig.Muted = !oldConfig.Muted;
					CommandDispatcher.Dispatch(new SetTrackSettingsCommand(Track, newConfig, oldConfig.Muted ? "Unmute track" : "Mute track"));
				}, button =>
				{
					button.BackColor = Track.GetTrackConfig<AudioTrackSettings>().Muted ? Color.Red : Color.Transparent;
				});
			}

			trackButtonsContainer.PerformLayout();
		}

		private IconButton CreateButton(string info, IconChar icon, EventHandler onClick, Action<IconButton> onUpdate = null)
		{
			var button = new IconButton();
			button.IconChar = icon;
			button.IconSize = trackButtonsContainer.Height - 6;
			button.Size = new Size(trackButtonsContainer.Height - 3, trackButtonsContainer.Height - 3);
			button.Click += _buttonOnClickHandlers[button] = onClick;
			trackButtonsContainer.Controls.Add(button);

			if(onUpdate != null)
			{
				_buttonUpdateHandlers.Add(button, onUpdate);
				onUpdate(button);
			}

			var tooltip = new ToolTip();
			tooltip.SetToolTip(button, info);
			_tooltips.Add(tooltip);
			return button;
		}

		#region UI Events
		private void TrackHeaderControl_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SetClip(e.ClipRectangle);

			if (_selected)
			{
				e.Graphics.DrawRectangle(_selectedTrackPen, ClientRectangle);
				e.Graphics.FillRectangle(_highlightBrush, new Rectangle(Point.Empty, Size));
			}
		}

		private void trackTitleLabel_Click(object sender, EventArgs e)
		{
			OnClick(e);
		}

		private void trackTypeLabel_Click(object sender, EventArgs e)
		{
			OnClick(e);
		}

		private void trackButtonsContainer_Click(object sender, EventArgs e)
		{
			OnClick(e);
		}
		#endregion
	}
}
