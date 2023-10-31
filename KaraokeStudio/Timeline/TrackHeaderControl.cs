﻿using FontAwesome.Sharp;
using KaraokeLib;
using KaraokeStudio.Util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace KaraokeStudio.Timeline
{
	public partial class TrackHeaderControl : UserControl
	{
		public KaraokeTrack? Track
		{
			get => _track;
			set {
				_track = value;
				UpdateComponent();
			}
		}

		private Pen _selectedTrackPen;
		private Brush _highlightBrush;
		private KaraokeTrack? _track;
		private Dictionary<IconButton, EventHandler> _buttonOnClickHandlers = new Dictionary<IconButton, EventHandler>();
		private bool _selected = false;

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
			foreach(var (button, handler) in _buttonOnClickHandlers)
			{
				button.Click -= handler;
			}

			_buttonOnClickHandlers.Clear();
			trackButtonsContainer.Controls.Clear();

			if(Track != null && Track.Type == KaraokeTrackType.Audio)
			{
				CreateButton(IconChar.VolumeMute, (o, e) => { });
			}

			trackButtonsContainer.PerformLayout();
		}

		private void CreateButton(IconChar icon, EventHandler onClick)
		{
			var button = new IconButton();
			button.IconChar = icon;
			button.IconSize = 24;
			button.Size = new Size(trackButtonsContainer.Height, trackButtonsContainer.Height);
			button.Click += _buttonOnClickHandlers[button] = onClick;
			trackButtonsContainer.Controls.Add(button);
		}

		#region UI Events
		private void TrackHeaderControl_Paint(object sender, PaintEventArgs e)
		{
			e.Graphics.SetClip(e.ClipRectangle);

			if(_selected)
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
