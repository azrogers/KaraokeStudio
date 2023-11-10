using KaraokeStudio.Timeline;

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
			this.openRecentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.midiToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lRCFileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.trackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.addAudioTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.removeTrackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.syncLyricsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.consoleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.verticalSplit = new System.Windows.Forms.SplitContainer();
			this.videoSplit = new System.Windows.Forms.SplitContainer();
			this.video = new KaraokeStudio.Video.KaraokeVideoControl();
			this.timelineContainer = new KaraokeStudio.Timeline.TimelineContainerControl();
			this.exportVideoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.verticalSplit)).BeginInit();
			this.verticalSplit.Panel1.SuspendLayout();
			this.verticalSplit.Panel2.SuspendLayout();
			this.verticalSplit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.videoSplit)).BeginInit();
			this.videoSplit.Panel2.SuspendLayout();
			this.videoSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.editToolStripMenuItem,
            this.trackToolStripMenuItem,
            this.eventToolStripMenuItem,
            this.debugToolStripMenuItem});
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
            this.openRecentToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripSeparator1,
            this.importToolStripMenuItem,
            this.exportToolStripMenuItem,
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
			// openRecentToolStripMenuItem
			// 
			this.openRecentToolStripMenuItem.Name = "openRecentToolStripMenuItem";
			this.openRecentToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.openRecentToolStripMenuItem.Text = "Open recent...";
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
			// exportToolStripMenuItem
			// 
			this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.exportVideoToolStripMenuItem,
            this.toolStripSeparator4,
            this.lRCFileToolStripMenuItem});
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.exportToolStripMenuItem.Text = "Export";
			// 
			// lRCFileToolStripMenuItem
			// 
			this.lRCFileToolStripMenuItem.Name = "lRCFileToolStripMenuItem";
			this.lRCFileToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.lRCFileToolStripMenuItem.Text = "LRC file...";
			this.lRCFileToolStripMenuItem.Click += new System.EventHandler(this.lRCFileToolStripMenuItem_Click);
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
			// editToolStripMenuItem
			// 
			this.editToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.editStyleToolStripMenuItem,
            this.toolStripSeparator3,
            this.undoToolStripMenuItem});
			this.editToolStripMenuItem.Name = "editToolStripMenuItem";
			this.editToolStripMenuItem.Size = new System.Drawing.Size(39, 20);
			this.editToolStripMenuItem.Text = "Edit";
			// 
			// editStyleToolStripMenuItem
			// 
			this.editStyleToolStripMenuItem.Name = "editStyleToolStripMenuItem";
			this.editStyleToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.editStyleToolStripMenuItem.Text = "Style...";
			this.editStyleToolStripMenuItem.Click += new System.EventHandler(this.editStyleToolStripMenuItem_Click);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(141, 6);
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			this.undoToolStripMenuItem.Click += new System.EventHandler(this.undoToolStripMenuItem_Click);
			// 
			// trackToolStripMenuItem
			// 
			this.trackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addTrackToolStripMenuItem,
            this.removeTrackToolStripMenuItem,
            this.syncLyricsToolStripMenuItem});
			this.trackToolStripMenuItem.Name = "trackToolStripMenuItem";
			this.trackToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.trackToolStripMenuItem.Text = "Track";
			// 
			// addTrackToolStripMenuItem
			// 
			this.addTrackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addAudioTrackToolStripMenuItem});
			this.addTrackToolStripMenuItem.Name = "addTrackToolStripMenuItem";
			this.addTrackToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.addTrackToolStripMenuItem.Text = "Add";
			// 
			// addAudioTrackToolStripMenuItem
			// 
			this.addAudioTrackToolStripMenuItem.Name = "addAudioTrackToolStripMenuItem";
			this.addAudioTrackToolStripMenuItem.Size = new System.Drawing.Size(115, 22);
			this.addAudioTrackToolStripMenuItem.Text = "Audio...";
			this.addAudioTrackToolStripMenuItem.Click += new System.EventHandler(this.audioToolStripMenuItem_Click);
			// 
			// removeTrackToolStripMenuItem
			// 
			this.removeTrackToolStripMenuItem.Name = "removeTrackToolStripMenuItem";
			this.removeTrackToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.removeTrackToolStripMenuItem.Text = "Remove";
			this.removeTrackToolStripMenuItem.Click += new System.EventHandler(this.removeTrackToolStripMenuItem_Click);
			// 
			// syncLyricsToolStripMenuItem
			// 
			this.syncLyricsToolStripMenuItem.Name = "syncLyricsToolStripMenuItem";
			this.syncLyricsToolStripMenuItem.Size = new System.Drawing.Size(131, 22);
			this.syncLyricsToolStripMenuItem.Text = "Sync Lyrics";
			this.syncLyricsToolStripMenuItem.Click += new System.EventHandler(this.syncLyricsToolStripMenuItem_Click);
			// 
			// eventToolStripMenuItem
			// 
			this.eventToolStripMenuItem.Name = "eventToolStripMenuItem";
			this.eventToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.eventToolStripMenuItem.Text = "Event";
			// 
			// debugToolStripMenuItem
			// 
			this.debugToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.consoleToolStripMenuItem});
			this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			this.debugToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
			this.debugToolStripMenuItem.Text = "Debug";
			// 
			// consoleToolStripMenuItem
			// 
			this.consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
			this.consoleToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
			this.consoleToolStripMenuItem.Text = "Console";
			this.consoleToolStripMenuItem.Click += new System.EventHandler(this.consoleToolStripMenuItem_Click);
			// 
			// verticalSplit
			// 
			this.verticalSplit.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.verticalSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verticalSplit.Location = new System.Drawing.Point(0, 24);
			this.verticalSplit.Name = "verticalSplit";
			this.verticalSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// verticalSplit.Panel1
			// 
			this.verticalSplit.Panel1.Controls.Add(this.videoSplit);
			// 
			// verticalSplit.Panel2
			// 
			this.verticalSplit.Panel2.Controls.Add(this.timelineContainer);
			this.verticalSplit.Size = new System.Drawing.Size(884, 426);
			this.verticalSplit.SplitterDistance = 299;
			this.verticalSplit.TabIndex = 1;
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
			this.videoSplit.Panel2.Controls.Add(this.video);
			this.videoSplit.Size = new System.Drawing.Size(884, 299);
			this.videoSplit.SplitterDistance = 551;
			this.videoSplit.TabIndex = 0;
			// 
			// video
			// 
			this.video.Dock = System.Windows.Forms.DockStyle.Fill;
			this.video.Location = new System.Drawing.Point(0, 0);
			this.video.Name = "video";
			this.video.Size = new System.Drawing.Size(327, 297);
			this.video.TabIndex = 0;
			// 
			// timelineContainer
			// 
			this.timelineContainer.AutoSize = true;
			this.timelineContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(39)))), ((int)(((byte)(41)))), ((int)(((byte)(50)))));
			this.timelineContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timelineContainer.Location = new System.Drawing.Point(0, 0);
			this.timelineContainer.MinimumSize = new System.Drawing.Size(50, 50);
			this.timelineContainer.Name = "timelineContainer";
			this.timelineContainer.Size = new System.Drawing.Size(882, 121);
			this.timelineContainer.TabIndex = 0;
			// 
			// exportVideoToolStripMenuItem
			// 
			this.exportVideoToolStripMenuItem.Name = "exportVideoToolStripMenuItem";
			this.exportVideoToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.exportVideoToolStripMenuItem.Text = "Video...";
			this.exportVideoToolStripMenuItem.Click += new System.EventHandler(this.exportVideoToolStripMenuItem_Click);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(177, 6);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(884, 450);
			this.Controls.Add(this.verticalSplit);
			this.Controls.Add(this.menuStrip);
			this.MainMenuStrip = this.menuStrip;
			this.Name = "MainForm";
			this.Text = "Karaoke Studio";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.verticalSplit.Panel1.ResumeLayout(false);
			this.verticalSplit.Panel2.ResumeLayout(false);
			this.verticalSplit.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.verticalSplit)).EndInit();
			this.verticalSplit.ResumeLayout(false);
			this.videoSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.videoSplit)).EndInit();
			this.videoSplit.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private SplitContainer verticalSplit;
		private SplitContainer videoSplit;
		private ToolStripMenuItem newToolStripMenuItem;
		private ToolStripMenuItem openToolStripMenuItem;
		private ToolStripMenuItem saveToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator1;
		private ToolStripMenuItem importToolStripMenuItem;
		private ToolStripMenuItem midiToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator2;
		private ToolStripMenuItem exitToolStripMenuItem;
		private ToolStripMenuItem saveAsToolStripMenuItem;
		private ToolStripMenuItem editToolStripMenuItem;
		private ToolStripMenuItem editStyleToolStripMenuItem;
		private ToolStripMenuItem openRecentToolStripMenuItem;
		private Video.KaraokeVideoControl video;
		private ToolStripMenuItem exportToolStripMenuItem;
		private ToolStripMenuItem lRCFileToolStripMenuItem;
		private ToolStripMenuItem trackToolStripMenuItem;
		private ToolStripMenuItem syncLyricsToolStripMenuItem;
		private ToolStripMenuItem eventToolStripMenuItem;
		private ToolStripMenuItem debugToolStripMenuItem;
		private ToolStripMenuItem consoleToolStripMenuItem;
		private ToolStripMenuItem addTrackToolStripMenuItem;
		private ToolStripMenuItem removeTrackToolStripMenuItem;
		private ToolStripMenuItem addAudioTrackToolStripMenuItem;
		private TimelineContainerControl timelineContainer;
		private ToolStripSeparator toolStripSeparator3;
		private ToolStripMenuItem undoToolStripMenuItem;
		private ToolStripMenuItem exportVideoToolStripMenuItem;
		private ToolStripSeparator toolStripSeparator4;
	}
}