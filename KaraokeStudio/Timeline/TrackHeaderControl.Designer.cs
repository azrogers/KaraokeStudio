namespace KaraokeStudio.Timeline
{
	partial class TrackHeaderControl
	{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			trackButtonsContainer = new FlowLayoutPanel();
			trackTypeLabel = new Label();
			trackTitleLabel = new Label();
			SuspendLayout();
			// 
			// trackButtonsContainer
			// 
			trackButtonsContainer.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			trackButtonsContainer.BackColor = Color.Transparent;
			trackButtonsContainer.Dock = DockStyle.Top;
			trackButtonsContainer.FlowDirection = FlowDirection.RightToLeft;
			trackButtonsContainer.Location = new Point(0, 0);
			trackButtonsContainer.MinimumSize = new Size(0, 25);
			trackButtonsContainer.Name = "trackButtonsContainer";
			trackButtonsContainer.Size = new Size(503, 25);
			trackButtonsContainer.TabIndex = 2;
			trackButtonsContainer.Click += trackButtonsContainer_Click;
			trackButtonsContainer.MouseDown += trackButtonsContainer_MouseDown;
			// 
			// trackTypeLabel
			// 
			trackTypeLabel.AutoSize = true;
			trackTypeLabel.BackColor = Color.Transparent;
			trackTypeLabel.Font = new Font("Open Sans", 9.75F, FontStyle.Italic);
			trackTypeLabel.ForeColor = Color.LightGray;
			trackTypeLabel.Location = new Point(3, 27);
			trackTypeLabel.Name = "trackTypeLabel";
			trackTypeLabel.Size = new Size(68, 19);
			trackTypeLabel.TabIndex = 1;
			trackTypeLabel.Text = "Track Type";
			trackTypeLabel.Click += trackTypeLabel_Click;
			trackTypeLabel.MouseDown += trackTypeLabel_MouseDown;
			// 
			// trackTitleLabel
			// 
			trackTitleLabel.AutoSize = true;
			trackTitleLabel.BackColor = Color.Transparent;
			trackTitleLabel.Font = new Font("Open Sans", 14.25F, FontStyle.Bold);
			trackTitleLabel.ForeColor = Color.White;
			trackTitleLabel.Location = new Point(0, 0);
			trackTitleLabel.Name = "trackTitleLabel";
			trackTitleLabel.Size = new Size(112, 27);
			trackTitleLabel.TabIndex = 0;
			trackTitleLabel.Text = "Track Title";
			trackTitleLabel.Click += trackTitleLabel_Click;
			trackTitleLabel.MouseDown += trackTitleLabel_MouseDown;
			// 
			// TrackHeaderControl
			// 
			AutoSize = true;
			BackColor = Color.Black;
			Controls.Add(trackTypeLabel);
			Controls.Add(trackTitleLabel);
			Controls.Add(trackButtonsContainer);
			ForeColor = Color.White;
			Name = "TrackHeaderControl";
			Size = new Size(503, 74);
			Paint += TrackHeaderControl_Paint;
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private FlowLayoutPanel trackButtonsContainer;
		private Label trackTypeLabel;
		private Label trackTitleLabel;
	}
}
