namespace KaraokeStudio
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
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

		#region Windows Form Designer generated code

		/// <summary>
		///  Required method for Designer support - do not modify
		///  the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.midiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.timelineSplit = new System.Windows.Forms.SplitContainer();
			this.videoSplit = new System.Windows.Forms.SplitContainer();
			this.videoPanel = new System.Windows.Forms.Panel();
			this.videoSkiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
			this.controlsPanel = new System.Windows.Forms.Panel();
			this.currentPosLabel = new System.Windows.Forms.Label();
			this.endPosLabel = new System.Windows.Forms.Label();
			this.startPosLabel = new System.Windows.Forms.Label();
			this.playPauseButton = new System.Windows.Forms.Button();
			this.forwardButton = new System.Windows.Forms.Button();
			this.backButton = new System.Windows.Forms.Button();
			this.positionBar = new System.Windows.Forms.TrackBar();
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.timelineSplit)).BeginInit();
			this.timelineSplit.Panel1.SuspendLayout();
			this.timelineSplit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.videoSplit)).BeginInit();
			this.videoSplit.Panel2.SuspendLayout();
			this.videoSplit.SuspendLayout();
			this.videoPanel.SuspendLayout();
			this.controlsPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.positionBar)).BeginInit();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.projectToolStripMenuItem});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(884, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newToolStripMenuItem,
            this.openToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.importToolStripMenuItem,
            this.toolStripSeparator2,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
			this.fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			this.newToolStripMenuItem.Name = "newToolStripMenuItem";
			this.newToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
			this.newToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.newToolStripMenuItem.Text = "New...";
			this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
			// 
			// openToolStripMenuItem
			// 
			this.openToolStripMenuItem.Name = "openToolStripMenuItem";
			this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.openToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.openToolStripMenuItem.Text = "Open...";
			this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
			// 
			// saveToolStripMenuItem
			// 
			this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			this.saveToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.saveToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.saveToolStripMenuItem.Text = "Save";
			this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
			// 
			// saveAsToolStripMenuItem
			// 
			this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			this.saveAsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift) 
            | System.Windows.Forms.Keys.S)));
			this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.saveAsToolStripMenuItem.Text = "Save as...";
			this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuitem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(190, 6);
			// 
			// importToolStripMenuItem
			// 
			this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.midiToolStripMenuItem});
			this.importToolStripMenuItem.Name = "importToolStripMenuItem";
			this.importToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.importToolStripMenuItem.Text = "Import";
			// 
			// midiToolStripMenuItem
			// 
			this.midiToolStripMenuItem.Name = "midiToolStripMenuItem";
			this.midiToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
			this.midiToolStripMenuItem.Text = "MIDI...";
			this.midiToolStripMenuItem.Click += new System.EventHandler(this.midiToolStripMenuItem_Click);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(190, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.exitToolStripMenuItem.Text = "Exit";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// timelineSplit
			// 
			this.timelineSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.timelineSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timelineSplit.Location = new System.Drawing.Point(0, 24);
			this.timelineSplit.Name = "timelineSplit";
			this.timelineSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// timelineSplit.Panel1
			// 
			this.timelineSplit.Panel1.Controls.Add(this.videoSplit);
			this.timelineSplit.Size = new System.Drawing.Size(884, 426);
			this.timelineSplit.SplitterDistance = 299;
			this.timelineSplit.TabIndex = 1;
			// 
			// videoSplit
			// 
			this.videoSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.videoSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoSplit.Location = new System.Drawing.Point(0, 0);
			this.videoSplit.Name = "videoSplit";
			// 
			// videoSplit.Panel2
			// 
			this.videoSplit.Panel2.Controls.Add(this.videoPanel);
			this.videoSplit.Panel2.Controls.Add(this.controlsPanel);
			this.videoSplit.Size = new System.Drawing.Size(884, 299);
			this.videoSplit.SplitterDistance = 551;
			this.videoSplit.TabIndex = 0;
			// 
			// videoPanel
			// 
			this.videoPanel.BackColor = System.Drawing.SystemColors.Control;
			this.videoPanel.Controls.Add(this.videoSkiaControl);
			this.videoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoPanel.Location = new System.Drawing.Point(0, 0);
			this.videoPanel.Name = "videoPanel";
			this.videoPanel.Size = new System.Drawing.Size(327, 233);
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
			this.controlsPanel.Controls.Add(this.currentPosLabel);
			this.controlsPanel.Controls.Add(this.endPosLabel);
			this.controlsPanel.Controls.Add(this.startPosLabel);
			this.controlsPanel.Controls.Add(this.playPauseButton);
			this.controlsPanel.Controls.Add(this.forwardButton);
			this.controlsPanel.Controls.Add(this.backButton);
			this.controlsPanel.Controls.Add(this.positionBar);
			this.controlsPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.controlsPanel.Location = new System.Drawing.Point(0, 233);
			this.controlsPanel.Name = "controlsPanel";
			this.controlsPanel.Size = new System.Drawing.Size(327, 64);
			this.controlsPanel.TabIndex = 0;
			// 
			// currentPosLabel
			// 
			this.currentPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.currentPosLabel.Location = new System.Drawing.Point(111, 20);
			this.currentPosLabel.Name = "currentPosLabel";
			this.currentPosLabel.Size = new System.Drawing.Size(108, 15);
			this.currentPosLabel.TabIndex = 6;
			this.currentPosLabel.Text = "0:00";
			this.currentPosLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// endPosLabel
			// 
			this.endPosLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.endPosLabel.Location = new System.Drawing.Point(249, 20);
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
			this.playPauseButton.Location = new System.Drawing.Point(111, 38);
			this.playPauseButton.Name = "playPauseButton";
			this.playPauseButton.Size = new System.Drawing.Size(108, 23);
			this.playPauseButton.TabIndex = 3;
			this.playPauseButton.Text = "Play";
			this.playPauseButton.UseVisualStyleBackColor = true;
			this.playPauseButton.Click += new System.EventHandler(this.playPauseButton_Click);
			// 
			// forwardButton
			// 
			this.forwardButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.forwardButton.Location = new System.Drawing.Point(249, 38);
			this.forwardButton.Name = "forwardButton";
			this.forwardButton.Size = new System.Drawing.Size(75, 23);
			this.forwardButton.TabIndex = 2;
			this.forwardButton.Text = ">>";
			this.forwardButton.UseVisualStyleBackColor = true;
			this.forwardButton.Click += new System.EventHandler(this.forwardButton_Click);
			// 
			// backButton
			// 
			this.backButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.backButton.Location = new System.Drawing.Point(3, 38);
			this.backButton.Name = "backButton";
			this.backButton.Size = new System.Drawing.Size(75, 23);
			this.backButton.TabIndex = 1;
			this.backButton.Text = "<<";
			this.backButton.UseVisualStyleBackColor = true;
			this.backButton.Click += new System.EventHandler(this.backButton_Click);
			// 
			// positionBar
			// 
			this.positionBar.Dock = System.Windows.Forms.DockStyle.Top;
			this.positionBar.Location = new System.Drawing.Point(0, 0);
			this.positionBar.Name = "positionBar";
			this.positionBar.Size = new System.Drawing.Size(327, 45);
			this.positionBar.TabIndex = 0;
			this.positionBar.TickStyle = System.Windows.Forms.TickStyle.None;
			this.positionBar.Scroll += new System.EventHandler(this.positionBar_Scroll);
			// 
			// projectToolStripMenuItem
			// 
			this.projectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editStyleToolStripMenuItem});
			this.projectToolStripMenuItem.Name = "projectToolStripMenuItem";
			this.projectToolStripMenuItem.Size = new System.Drawing.Size(56, 20);
			this.projectToolStripMenuItem.Text = "Project";
			// 
			// editStyleToolStripMenuItem
			// 
			this.editStyleToolStripMenuItem.Name = "editStyleToolStripMenuItem";
			this.editStyleToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.editStyleToolStripMenuItem.Text = "Style...";
			this.editStyleToolStripMenuItem.Click += new System.EventHandler(this.editStyleToolStripMenuItem_Click);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 450);
			this.Controls.Add(this.timelineSplit);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "Karaoke Studio";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.timelineSplit.Panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.timelineSplit)).EndInit();
			this.timelineSplit.ResumeLayout(false);
			this.videoSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.videoSplit)).EndInit();
			this.videoSplit.ResumeLayout(false);
			this.videoPanel.ResumeLayout(false);
			this.controlsPanel.ResumeLayout(false);
			this.controlsPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.positionBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private SplitContainer timelineSplit;
		private SplitContainer videoSplit;
		private Panel controlsPanel;
		private Button forwardButton;
		private Button backButton;
		private TrackBar positionBar;
		private Button playPauseButton;
		private Panel videoPanel;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem importToolStripMenuItem;
		private ToolStripMenuItem midiToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private Label currentPosLabel;
		private Label endPosLabel;
		private Label startPosLabel;
		private SkiaSharp.Views.Desktop.SKGLControl videoSkiaControl;
		private ToolStripMenuItem projectToolStripMenuItem;
		private ToolStripMenuItem editStyleToolStripMenuItem;
	}
}