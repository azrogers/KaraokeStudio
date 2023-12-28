using FontAwesome.Sharp;
using KaraokeLib.Tracks;
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

		/// <summary>
		/// Called when the settings of the track this header is for have been changed.
		/// </summary>
		public event Action<KaraokeTrack>? OnTrackConfigChanged;

		private Pen _selectedTrackPen;
		private Brush _highlightBrush;
		private KaraokeTrack? _track;
		private List<ToolTip> _tooltips = new List<ToolTip>();
		private Dictionary<IconButton, EventHandler> _buttonOnClickHandlers = new Dictionary<IconButton, EventHandler>();
		private bool _selected = false;
		private GenericConfigEditorForm? _configEditorForm;

		public TrackHeaderControl()
		{
			InitializeComponent();

			_selectedTrackPen = new Pen(Brushes.White, 3.0f);
			_highlightBrush = new SolidBrush(Color.FromArgb(50, 255, 255, 255));
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
				if (_configEditorForm == null || _configEditorForm.IsDisposed)
				{
					_configEditorForm = new GenericConfigEditorForm();
				}

				_configEditorForm.Open(Track.GetTrackConfig(), (newConfig) =>
				{
					Track.SetTrackConfig(newConfig);
					OnTrackConfigChanged?.Invoke(Track);
				});
			});

			if (Track.Type == KaraokeTrackType.Lyrics)
			{
				CreateButton("Sync Lyrics", IconChar.Music, (o, e) =>
				{
					if(Project != null)
					{
						WindowManager.OpenSyncForm(Project, Track);
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
					oldConfig.Muted = !oldConfig.Muted;
					button.BackColor = oldConfig.Muted ? Color.Red : Color.Transparent;
					Track.SetTrackConfig(oldConfig);

					OnTrackConfigChanged?.Invoke(Track);
				});

				muteButton.BackColor = config.Muted ? Color.Red : Color.Transparent;
			}

			trackButtonsContainer.PerformLayout();
		}

		private IconButton CreateButton(string info, IconChar icon, EventHandler onClick)
		{
			var button = new IconButton();
			button.IconChar = icon;
			button.IconSize = trackButtonsContainer.Height - 6;
			button.Size = new Size(trackButtonsContainer.Height - 3, trackButtonsContainer.Height - 3);
			button.Click += _buttonOnClickHandlers[button] = onClick;
			trackButtonsContainer.Controls.Add(button);

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
