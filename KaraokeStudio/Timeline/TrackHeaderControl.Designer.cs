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
			this.trackButtonsContainer = new System.Windows.Forms.FlowLayoutPanel();
			this.trackTypeLabel = new System.Windows.Forms.Label();
			this.trackTitleLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// trackButtonsContainer
			// 
			this.trackButtonsContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.trackButtonsContainer.BackColor = System.Drawing.Color.Transparent;
			this.trackButtonsContainer.Dock = System.Windows.Forms.DockStyle.Top;
			this.trackButtonsContainer.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.trackButtonsContainer.Location = new System.Drawing.Point(0, 0);
			this.trackButtonsContainer.MinimumSize = new System.Drawing.Size(0, 25);
			this.trackButtonsContainer.Name = "trackButtonsContainer";
			this.trackButtonsContainer.Size = new System.Drawing.Size(503, 25);
			this.trackButtonsContainer.TabIndex = 2;
			this.trackButtonsContainer.Click += new System.EventHandler(this.trackButtonsContainer_Click);
			// 
			// trackTypeLabel
			// 
			this.trackTypeLabel.AutoSize = true;
			this.trackTypeLabel.BackColor = System.Drawing.Color.Transparent;
			this.trackTypeLabel.Font = new System.Drawing.Font("Open Sans", 9.75F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
			this.trackTypeLabel.ForeColor = System.Drawing.Color.LightGray;
			this.trackTypeLabel.Location = new System.Drawing.Point(3, 27);
			this.trackTypeLabel.Name = "trackTypeLabel";
			this.trackTypeLabel.Size = new System.Drawing.Size(68, 19);
			this.trackTypeLabel.TabIndex = 1;
			this.trackTypeLabel.Text = "Track Type";
			this.trackTypeLabel.Click += new System.EventHandler(this.trackTypeLabel_Click);
			// 
			// trackTitleLabel
			// 
			this.trackTitleLabel.AutoSize = true;
			this.trackTitleLabel.BackColor = System.Drawing.Color.Transparent;
			this.trackTitleLabel.Font = new System.Drawing.Font("Open Sans", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.trackTitleLabel.ForeColor = System.Drawing.Color.White;
			this.trackTitleLabel.Location = new System.Drawing.Point(0, 0);
			this.trackTitleLabel.Name = "trackTitleLabel";
			this.trackTitleLabel.Size = new System.Drawing.Size(112, 27);
			this.trackTitleLabel.TabIndex = 0;
			this.trackTitleLabel.Text = "Track Title";
			this.trackTitleLabel.Click += new System.EventHandler(this.trackTitleLabel_Click);
			// 
			// TrackHeaderControl
			// 
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.trackTypeLabel);
			this.Controls.Add(this.trackTitleLabel);
			this.Controls.Add(this.trackButtonsContainer);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "TrackHeaderControl";
			this.Size = new System.Drawing.Size(503, 74);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TrackHeaderControl_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private FlowLayoutPanel trackButtonsContainer;
		private Label trackTypeLabel;
		private Label trackTitleLabel;
	}
}
