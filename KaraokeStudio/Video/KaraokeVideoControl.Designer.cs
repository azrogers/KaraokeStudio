namespace KaraokeStudio.Video
{
	partial class KaraokeVideoControl
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
			this.videoPanel = new System.Windows.Forms.Panel();
			this.videoSkiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
			this.controlsPanel = new System.Windows.Forms.Panel();
			this.volumeSlider = new NAudio.Gui.VolumeSlider();
			this.currentPosLabel = new System.Windows.Forms.Label();
			this.endPosLabel = new System.Windows.Forms.Label();
			this.startPosLabel = new System.Windows.Forms.Label();
			this.playPauseButton = new FontAwesome.Sharp.IconButton();
			this.forwardButton = new FontAwesome.Sharp.IconButton();
			this.backButton = new FontAwesome.Sharp.IconButton();
			this.positionBar = new System.Windows.Forms.TrackBar();
			this.playbackRateSelector = new System.Windows.Forms.ComboBox();
			this.videoPanel.SuspendLayout();
			this.controlsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.positionBar)).BeginInit();
			this.SuspendLayout();
			// 
			// videoPanel
			// 
			this.videoPanel.BackColor = System.Drawing.SystemColors.Control;
			this.videoPanel.Controls.Add(this.videoSkiaControl);
			this.videoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoPanel.Location = new System.Drawing.Point(0, 0);
			this.videoPanel.Name = "videoPanel";
			this.videoPanel.Size = new System.Drawing.Size(564, 465);
			this.videoPanel.TabIndex = 1;
			this.videoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.videoPanel_Paint);
			// 
			// videoSkiaControl
			// 
			this.videoSkiaControl.BackColor = System.Drawing.Color.Black;
			this.videoSkiaControl.Location = new System.Drawing.Point(0, 0);
			this.videoSkiaControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.videoSkiaControl.Name = "videoSkiaControl";
			this.videoSkiaControl.Size = new System.Drawing.Size(327, 233);
			this.videoSkiaControl.TabIndex = 0;
			this.videoSkiaControl.VSync = true;
			this.videoSkiaControl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.videoSkiaControl_PaintSurface);
			// 
			// controlsPanel
			// 
			this.controlsPanel.Controls.Add(this.playbackRateSelector);
			this.controlsPanel.Controls.Add(this.volumeSlider);
			this.controlsPanel.Controls.Add(this.currentPosLabel);
			this.controlsPanel.Controls.Add(this.endPosLabel);
			this.controlsPanel.Controls.Add(this.startPosLabel);
			this.controlsPanel.Controls.Add(this.playPauseButton);
			this.controlsPanel.Controls.Add(this.forwardButton);
			this.controlsPanel.Controls.Add(this.backButton);
			this.controlsPanel.Controls.Add(this.positionBar);
			this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.controlsPanel.Location = new System.Drawing.Point(0, 465);
			this.controlsPanel.Name = "controlsPanel";
			this.controlsPanel.Size = new System.Drawing.Size(564, 64);
			this.controlsPanel.TabIndex = 0;
			// 
			// volumeSlider
			// 
			this.volumeSlider.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.volumeSlider.Location = new System.Drawing.Point(486, 38);
			this.volumeSlider.Name = "volumeSlider";
			this.volumeSlider.Size = new System.Drawing.Size(75, 23);
			this.volumeSlider.TabIndex = 7;
			this.volumeSlider.VolumeChanged += new System.EventHandler(this.volumeSlider_VolumeChanged);
			// 
			// currentPosLabel
			// 
			this.currentPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.currentPosLabel.Location = new System.Drawing.Point(111, 20);
			this.currentPosLabel.Name = "currentPosLabel";
			this.currentPosLabel.Size = new System.Drawing.Size(345, 15);
			this.currentPosLabel.TabIndex = 6;
			this.currentPosLabel.Text = "0:00";
			this.currentPosLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// endPosLabel
			// 
			this.endPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.endPosLabel.Location = new System.Drawing.Point(486, 20);
			this.endPosLabel.Name = "endPosLabel";
			this.endPosLabel.Size = new System.Drawing.Size(75, 15);
			this.endPosLabel.TabIndex = 5;
			this.endPosLabel.Text = "0:00";
			this.endPosLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// startPosLabel
			// 
			this.startPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.startPosLabel.Location = new System.Drawing.Point(3, 20);
			this.startPosLabel.Name = "startPosLabel";
			this.startPosLabel.Size = new System.Drawing.Size(75, 15);
			this.startPosLabel.TabIndex = 4;
			this.startPosLabel.Text = "0:00";
			// 
			// playPauseButton
			// 
			this.playPauseButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.playPauseButton.IconChar = FontAwesome.Sharp.IconChar.Play;
			this.playPauseButton.IconColor = System.Drawing.Color.Black;
			this.playPauseButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.playPauseButton.IconSize = 14;
			this.playPauseButton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.playPauseButton.Location = new System.Drawing.Point(266, 38);
			this.playPauseButton.Name = "playPauseButton";
			this.playPauseButton.Size = new System.Drawing.Size(32, 23);
			this.playPauseButton.TabIndex = 3;
			this.playPauseButton.UseVisualStyleBackColor = true;
			this.playPauseButton.Click += new System.EventHandler(this.playPauseButton_Click);
			// 
			// forwardButton
			// 
			this.forwardButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.forwardButton.IconChar = FontAwesome.Sharp.IconChar.Forward;
			this.forwardButton.IconColor = System.Drawing.Color.Black;
			this.forwardButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.forwardButton.IconSize = 16;
			this.forwardButton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.forwardButton.Location = new System.Drawing.Point(304, 38);
			this.forwardButton.Name = "forwardButton";
			this.forwardButton.Size = new System.Drawing.Size(32, 23);
			this.forwardButton.TabIndex = 2;
			this.forwardButton.UseVisualStyleBackColor = true;
			this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
			// 
			// backButton
			// 
			this.backButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
			this.backButton.IconChar = FontAwesome.Sharp.IconChar.Backward;
			this.backButton.IconColor = System.Drawing.Color.Black;
			this.backButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.backButton.IconSize = 16;
			this.backButton.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.backButton.Location = new System.Drawing.Point(228, 38);
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(32, 23);
			this.backButton.TabIndex = 1;
			this.backButton.UseVisualStyleBackColor = true;
			this.backButton.Click += new System.EventHandler(this.backButton_Click);
			// 
			// positionBar
			// 
			this.positionBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.positionBar.Location = new System.Drawing.Point(0, 0);
			this.positionBar.Name = "positionBar";
			this.positionBar.Size = new System.Drawing.Size(564, 45);
			this.positionBar.TabIndex = 0;
			this.positionBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.positionBar.Scroll += new System.EventHandler(this.positionBar_Scroll);
			// 
			// playbackRateSelector
			// 
			this.playbackRateSelector.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.playbackRateSelector.FormattingEnabled = true;
			this.playbackRateSelector.Items.AddRange(new object[] {
            "0.1x",
            "0.25x",
            "0.5x",
            "0.75x",
            "1x",
            "1.25x",
            "1.5x",
            "2x"});
			this.playbackRateSelector.Location = new System.Drawing.Point(421, 38);
			this.playbackRateSelector.Name = "playbackRateSelector";
			this.playbackRateSelector.Size = new System.Drawing.Size(59, 23);
			this.playbackRateSelector.TabIndex = 8;
			this.playbackRateSelector.SelectedIndexChanged += new System.EventHandler(this.playbackRateSelector_SelectedIndexChanged);
			// 
			// KaraokeVideoControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.videoPanel);
			this.Controls.Add(this.controlsPanel);
			this.Name = "KaraokeVideoControl";
			this.Size = new System.Drawing.Size(564, 529);
			this.videoPanel.ResumeLayout(false);
			this.controlsPanel.ResumeLayout(false);
			this.controlsPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.positionBar)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private Panel controlsPanel;
		private FontAwesome.Sharp.IconButton forwardButton;
		private FontAwesome.Sharp.IconButton backButton;
		private TrackBar positionBar;
		private FontAwesome.Sharp.IconButton playPauseButton;
		private Panel videoPanel;
		private Label currentPosLabel;
		private Label endPosLabel;
		private Label startPosLabel;
		private SkiaSharp.Views.Desktop.SKGLControl videoSkiaControl;
		private NAudio.Gui.VolumeSlider volumeSlider;
		private ComboBox playbackRateSelector;
	}
}
