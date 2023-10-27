﻿using KaraokeStudio.Timeline;

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
			this.projectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.editStyleToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.lyricsEditor = new KaraokeStudio.LyricsEditor.LyricsEditorControl();
			this.timelineSplit = new System.Windows.Forms.SplitContainer();
			this.videoSplit = new System.Windows.Forms.SplitContainer();
			this.video = new KaraokeStudio.Video.KaraokeVideoControl();
			this.timeline = new KaraokeStudio.Timeline.TimelineControl();
			this.trackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.eventToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.syncLyricsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.timelineSplit)).BeginInit();
			this.timelineSplit.Panel1.SuspendLayout();
			this.timelineSplit.Panel2.SuspendLayout();
			this.timelineSplit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.videoSplit)).BeginInit();
			this.videoSplit.Panel1.SuspendLayout();
			this.videoSplit.Panel2.SuspendLayout();
			this.videoSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.fileToolStripMenuItem,
			this.projectToolStripMenuItem,
			this.trackToolStripMenuItem,
			this.eventToolStripMenuItem});
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
			this.lRCFileToolStripMenuItem});
			this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			this.exportToolStripMenuItem.Size = new System.Drawing.Size(193, 22);
			this.exportToolStripMenuItem.Text = "Export";
			// 
			// lRCFileToolStripMenuItem
			// 
			this.lRCFileToolStripMenuItem.Name = "lRCFileToolStripMenuItem";
			this.lRCFileToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
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
			this.editStyleToolStripMenuItem.Size = new System.Drawing.Size(108, 22);
			this.editStyleToolStripMenuItem.Text = "Style...";
			this.editStyleToolStripMenuItem.Click += new System.EventHandler(this.editStyleToolStripMenuItem_Click);
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
			// 
			// timelineSplit.Panel2
			// 
			this.timelineSplit.Panel2.Controls.Add(this.timeline);
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
			// videoSplit.Panel1
			//
			this.videoSplit.Panel1.Controls.Add(this.lyricsEditor);
			// 
			// videoSplit.Panel2
			// 
			this.videoSplit.Panel2.Controls.Add(this.video);
			this.videoSplit.Size = new System.Drawing.Size(884, 299);
			this.videoSplit.SplitterDistance = 551;
			this.videoSplit.TabIndex = 0;
			// 
			// lyricsEditor
			//
			this.lyricsEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lyricsEditor.Location = new System.Drawing.Point(0, 0);
			this.lyricsEditor.Name = "lyricsEditor";
			this.lyricsEditor.Size = new System.Drawing.Size(549, 297);
			this.lyricsEditor.TabIndex = 0;
			//
			// video
			// 
			this.video.Dock = System.Windows.Forms.DockStyle.Fill;
			this.video.Location = new System.Drawing.Point(0, 0);
			this.video.Name = "video";
			this.video.Size = new System.Drawing.Size(327, 297);
			this.video.TabIndex = 0;
			// 
			// timeline
			// 
			this.timeline.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timeline.Location = new System.Drawing.Point(0, 0);
			this.timeline.Name = "timeline";
			this.timeline.Size = new System.Drawing.Size(882, 121);
			this.timeline.TabIndex = 0;
			// 
			// trackToolStripMenuItem
			// 
			this.trackToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
			this.syncLyricsToolStripMenuItem});
			this.trackToolStripMenuItem.Name = "trackToolStripMenuItem";
			this.trackToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
			this.trackToolStripMenuItem.Text = "Track";
			// 
			// eventToolStripMenuItem
			// 
			this.eventToolStripMenuItem.Name = "eventToolStripMenuItem";
			this.eventToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
			this.eventToolStripMenuItem.Text = "Event";
			// 
			// syncLyricsToolStripMenuItem
			// 
			this.syncLyricsToolStripMenuItem.Name = "syncLyricsToolStripMenuItem";
			this.syncLyricsToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
			this.syncLyricsToolStripMenuItem.Text = "Sync Lyrics";
			this.syncLyricsToolStripMenuItem.Click += new System.EventHandler(this.syncLyricsToolStripMenuItem_Click);
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
			this.timelineSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.timelineSplit)).EndInit();
			this.timelineSplit.ResumeLayout(false);
			this.videoSplit.Panel1.ResumeLayout(false);
			this.videoSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.videoSplit)).EndInit();
			this.videoSplit.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private MenuStrip menuStrip;
		private ToolStripMenuItem fileToolStripMenuItem;
		private SplitContainer timelineSplit;
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
		private ToolStripMenuItem projectToolStripMenuItem;
		private ToolStripMenuItem editStyleToolStripMenuItem;
		private ToolStripMenuItem openRecentToolStripMenuItem;
		private Video.KaraokeVideoControl video;
		private TimelineControl timeline;
		private LyricsEditor.LyricsEditorControl lyricsEditor;
		private ToolStripMenuItem exportToolStripMenuItem;
		private ToolStripMenuItem lRCFileToolStripMenuItem;
		private ToolStripMenuItem trackToolStripMenuItem;
		private ToolStripMenuItem syncLyricsToolStripMenuItem;
		private ToolStripMenuItem eventToolStripMenuItem;
	}
}