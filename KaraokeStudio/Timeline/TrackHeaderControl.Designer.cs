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
			this.trackTitleLabel = new System.Windows.Forms.Label();
			this.trackTypeLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// trackTitleLabel
			// 
			this.trackTitleLabel.AutoSize = true;
			this.trackTitleLabel.BackColor = System.Drawing.Color.Transparent;
			this.trackTitleLabel.Font = new System.Drawing.Font("Open Sans", 18F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
			this.trackTitleLabel.ForeColor = System.Drawing.Color.White;
			this.trackTitleLabel.Location = new System.Drawing.Point(0, 0);
			this.trackTitleLabel.Name = "trackTitleLabel";
			this.trackTitleLabel.Size = new System.Drawing.Size(139, 35);
			this.trackTitleLabel.TabIndex = 0;
			this.trackTitleLabel.Text = "Track Title";
			this.trackTitleLabel.Click += new System.EventHandler(this.trackTitleLabel_Click);
			// 
			// trackTypeLabel
			// 
			this.trackTypeLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.trackTypeLabel.BackColor = System.Drawing.Color.Transparent;
			this.trackTypeLabel.Font = new System.Drawing.Font("Open Sans", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point);
			this.trackTypeLabel.ForeColor = System.Drawing.Color.LightGray;
			this.trackTypeLabel.Location = new System.Drawing.Point(0, 9);
			this.trackTypeLabel.Name = "trackTypeLabel";
			this.trackTypeLabel.Size = new System.Drawing.Size(313, 26);
			this.trackTypeLabel.TabIndex = 1;
			this.trackTypeLabel.Text = "Track Type";
			this.trackTypeLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			this.trackTypeLabel.Click += new System.EventHandler(this.trackTypeLabel_Click);
			// 
			// TrackHeaderControl
			// 
			this.AutoSize = true;
			this.BackColor = System.Drawing.Color.Black;
			this.Controls.Add(this.trackTitleLabel);
			this.Controls.Add(this.trackTypeLabel);
			this.ForeColor = System.Drawing.Color.White;
			this.Name = "TrackHeaderControl";
			this.Size = new System.Drawing.Size(316, 85);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TrackHeaderControl_Paint);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private Label trackTitleLabel;
		private Label trackTypeLabel;
	}
}
