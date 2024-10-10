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
			menuStrip = new MenuStrip();
			fileToolStripMenuItem = new ToolStripMenuItem();
			newToolStripMenuItem = new ToolStripMenuItem();
			openToolStripMenuItem = new ToolStripMenuItem();
			openRecentToolStripMenuItem = new ToolStripMenuItem();
			saveToolStripMenuItem = new ToolStripMenuItem();
			saveAsToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator1 = new ToolStripSeparator();
			importToolStripMenuItem = new ToolStripMenuItem();
			midiToolStripMenuItem = new ToolStripMenuItem();
			exportToolStripMenuItem = new ToolStripMenuItem();
			exportVideoToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator4 = new ToolStripSeparator();
			lRCFileToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator2 = new ToolStripSeparator();
			exitToolStripMenuItem = new ToolStripMenuItem();
			editToolStripMenuItem = new ToolStripMenuItem();
			editStyleToolStripMenuItem = new ToolStripMenuItem();
			toolStripSeparator3 = new ToolStripSeparator();
			undoToolStripMenuItem = new ToolStripMenuItem();
			redoToolStripMenuItem = new ToolStripMenuItem();
			trackToolStripMenuItem = new ToolStripMenuItem();
			addTrackToolStripMenuItem = new ToolStripMenuItem();
			addAudioTrackToolStripMenuItem = new ToolStripMenuItem();
			removeTrackToolStripMenuItem = new ToolStripMenuItem();
			syncLyricsToolStripMenuItem = new ToolStripMenuItem();
			trackPropertiesToolStripMenuItem = new ToolStripMenuItem();
			eventToolStripMenuItem = new ToolStripMenuItem();
			addEventToolStripMenuItem = new ToolStripMenuItem();
			addAudioClipToolStripMenuItem = new ToolStripMenuItem();
			removeEventToolStripMenuItem = new ToolStripMenuItem();
			eventPropertiesToolStripMenuItem = new ToolStripMenuItem();
			debugToolStripMenuItem = new ToolStripMenuItem();
			consoleToolStripMenuItem = new ToolStripMenuItem();
			verticalSplit = new SplitContainer();
			videoSplit = new SplitContainer();
			video = new Video.KaraokeVideoControl();
			timelineContainer = new TimelineContainerControl();
			settingsMenuItem = new ToolStripMenuItem();
			audioSettingsToolStripMenuItem = new ToolStripMenuItem();
			menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)verticalSplit).BeginInit();
			verticalSplit.Panel1.SuspendLayout();
			verticalSplit.Panel2.SuspendLayout();
			verticalSplit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)videoSplit).BeginInit();
			videoSplit.Panel2.SuspendLayout();
			videoSplit.SuspendLayout();
			SuspendLayout();
			// 
			// menuStrip
			// 
			menuStrip.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, editToolStripMenuItem, trackToolStripMenuItem, eventToolStripMenuItem, debugToolStripMenuItem });
			menuStrip.Location = new Point(0, 0);
			menuStrip.Name = "menuStrip";
			menuStrip.Size = new Size(884, 24);
			menuStrip.TabIndex = 0;
			menuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { newToolStripMenuItem, openToolStripMenuItem, openRecentToolStripMenuItem, saveToolStripMenuItem, saveAsToolStripMenuItem, toolStripSeparator1, importToolStripMenuItem, exportToolStripMenuItem, toolStripSeparator2, settingsMenuItem, exitToolStripMenuItem });
			fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			fileToolStripMenuItem.Size = new Size(37, 20);
			fileToolStripMenuItem.Text = "File";
			// 
			// newToolStripMenuItem
			// 
			newToolStripMenuItem.Name = "newToolStripMenuItem";
			newToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.N;
			newToolStripMenuItem.Size = new Size(193, 22);
			newToolStripMenuItem.Text = "New...";
			newToolStripMenuItem.Click += newToolStripMenuItem_Click;
			// 
			// openToolStripMenuItem
			// 
			openToolStripMenuItem.Name = "openToolStripMenuItem";
			openToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.O;
			openToolStripMenuItem.Size = new Size(193, 22);
			openToolStripMenuItem.Text = "Open...";
			openToolStripMenuItem.Click += openToolStripMenuItem_Click;
			// 
			// openRecentToolStripMenuItem
			// 
			openRecentToolStripMenuItem.Name = "openRecentToolStripMenuItem";
			openRecentToolStripMenuItem.Size = new Size(193, 22);
			openRecentToolStripMenuItem.Text = "Open recent...";
			// 
			// saveToolStripMenuItem
			// 
			saveToolStripMenuItem.Name = "saveToolStripMenuItem";
			saveToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.S;
			saveToolStripMenuItem.Size = new Size(193, 22);
			saveToolStripMenuItem.Text = "Save";
			saveToolStripMenuItem.Click += saveToolStripMenuItem_Click;
			// 
			// saveAsToolStripMenuItem
			// 
			saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
			saveAsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Shift | Keys.S;
			saveAsToolStripMenuItem.Size = new Size(193, 22);
			saveAsToolStripMenuItem.Text = "Save as...";
			saveAsToolStripMenuItem.Click += saveAsToolStripMenuitem_Click;
			// 
			// toolStripSeparator1
			// 
			toolStripSeparator1.Name = "toolStripSeparator1";
			toolStripSeparator1.Size = new Size(190, 6);
			// 
			// importToolStripMenuItem
			// 
			importToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { midiToolStripMenuItem });
			importToolStripMenuItem.Name = "importToolStripMenuItem";
			importToolStripMenuItem.Size = new Size(193, 22);
			importToolStripMenuItem.Text = "Import";
			// 
			// midiToolStripMenuItem
			// 
			midiToolStripMenuItem.Name = "midiToolStripMenuItem";
			midiToolStripMenuItem.Size = new Size(108, 22);
			midiToolStripMenuItem.Text = "MIDI...";
			midiToolStripMenuItem.Click += midiToolStripMenuItem_Click;
			// 
			// exportToolStripMenuItem
			// 
			exportToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { exportVideoToolStripMenuItem, toolStripSeparator4, lRCFileToolStripMenuItem });
			exportToolStripMenuItem.Name = "exportToolStripMenuItem";
			exportToolStripMenuItem.Size = new Size(193, 22);
			exportToolStripMenuItem.Text = "Export";
			// 
			// exportVideoToolStripMenuItem
			// 
			exportVideoToolStripMenuItem.Name = "exportVideoToolStripMenuItem";
			exportVideoToolStripMenuItem.ShortcutKeys = Keys.F1;
			exportVideoToolStripMenuItem.Size = new Size(180, 22);
			exportVideoToolStripMenuItem.Text = "Video...";
			exportVideoToolStripMenuItem.Click += exportVideoToolStripMenuItem_Click;
			// 
			// toolStripSeparator4
			// 
			toolStripSeparator4.Name = "toolStripSeparator4";
			toolStripSeparator4.Size = new Size(177, 6);
			// 
			// lRCFileToolStripMenuItem
			// 
			lRCFileToolStripMenuItem.Name = "lRCFileToolStripMenuItem";
			lRCFileToolStripMenuItem.Size = new Size(180, 22);
			lRCFileToolStripMenuItem.Text = "LRC file...";
			lRCFileToolStripMenuItem.Click += lRCFileToolStripMenuItem_Click;
			// 
			// toolStripSeparator2
			// 
			toolStripSeparator2.Name = "toolStripSeparator2";
			toolStripSeparator2.Size = new Size(190, 6);
			// 
			// exitToolStripMenuItem
			// 
			exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			exitToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Q;
			exitToolStripMenuItem.Size = new Size(193, 22);
			exitToolStripMenuItem.Text = "Exit";
			exitToolStripMenuItem.Click += exitToolStripMenuItem_Click;
			// 
			// editToolStripMenuItem
			// 
			editToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { editStyleToolStripMenuItem, toolStripSeparator3, undoToolStripMenuItem, redoToolStripMenuItem });
			editToolStripMenuItem.Name = "editToolStripMenuItem";
			editToolStripMenuItem.Size = new Size(39, 20);
			editToolStripMenuItem.Text = "Edit";
			// 
			// editStyleToolStripMenuItem
			// 
			editStyleToolStripMenuItem.Name = "editStyleToolStripMenuItem";
			editStyleToolStripMenuItem.ShortcutKeys = Keys.Alt | Keys.S;
			editStyleToolStripMenuItem.Size = new Size(180, 22);
			editStyleToolStripMenuItem.Text = "Style...";
			editStyleToolStripMenuItem.Click += editStyleToolStripMenuItem_Click;
			// 
			// toolStripSeparator3
			// 
			toolStripSeparator3.Name = "toolStripSeparator3";
			toolStripSeparator3.Size = new Size(177, 6);
			// 
			// undoToolStripMenuItem
			// 
			undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			undoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Z;
			undoToolStripMenuItem.Size = new Size(180, 22);
			undoToolStripMenuItem.Text = "Undo";
			undoToolStripMenuItem.Click += undoToolStripMenuItem_Click;
			// 
			// redoToolStripMenuItem
			// 
			redoToolStripMenuItem.Name = "redoToolStripMenuItem";
			redoToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Y;
			redoToolStripMenuItem.Size = new Size(180, 22);
			redoToolStripMenuItem.Text = "Redo";
			redoToolStripMenuItem.Click += redoToolStripMenuItem_Click;
			// 
			// trackToolStripMenuItem
			// 
			trackToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addTrackToolStripMenuItem, removeTrackToolStripMenuItem, syncLyricsToolStripMenuItem, trackPropertiesToolStripMenuItem });
			trackToolStripMenuItem.Name = "trackToolStripMenuItem";
			trackToolStripMenuItem.Size = new Size(46, 20);
			trackToolStripMenuItem.Text = "Track";
			// 
			// addTrackToolStripMenuItem
			// 
			addTrackToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addAudioTrackToolStripMenuItem });
			addTrackToolStripMenuItem.Name = "addTrackToolStripMenuItem";
			addTrackToolStripMenuItem.Size = new Size(180, 22);
			addTrackToolStripMenuItem.Text = "Add";
			// 
			// addAudioTrackToolStripMenuItem
			// 
			addAudioTrackToolStripMenuItem.Name = "addAudioTrackToolStripMenuItem";
			addAudioTrackToolStripMenuItem.Size = new Size(115, 22);
			addAudioTrackToolStripMenuItem.Text = "Audio...";
			addAudioTrackToolStripMenuItem.Click += audioToolStripMenuItem_Click;
			// 
			// removeTrackToolStripMenuItem
			// 
			removeTrackToolStripMenuItem.Name = "removeTrackToolStripMenuItem";
			removeTrackToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.Delete;
			removeTrackToolStripMenuItem.Size = new Size(180, 22);
			removeTrackToolStripMenuItem.Text = "Remove";
			removeTrackToolStripMenuItem.Click += removeTrackToolStripMenuItem_Click;
			// 
			// syncLyricsToolStripMenuItem
			// 
			syncLyricsToolStripMenuItem.Name = "syncLyricsToolStripMenuItem";
			syncLyricsToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.D;
			syncLyricsToolStripMenuItem.Size = new Size(180, 22);
			syncLyricsToolStripMenuItem.Text = "Sync Lyrics";
			syncLyricsToolStripMenuItem.Click += syncLyricsToolStripMenuItem_Click;
			// 
			// trackPropertiesToolStripMenuItem
			// 
			trackPropertiesToolStripMenuItem.Name = "trackPropertiesToolStripMenuItem";
			trackPropertiesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.T;
			trackPropertiesToolStripMenuItem.Size = new Size(180, 22);
			trackPropertiesToolStripMenuItem.Text = "Properties";
			trackPropertiesToolStripMenuItem.Click += trackPropertiesToolStripMenuItem_Click;
			// 
			// eventToolStripMenuItem
			// 
			eventToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addEventToolStripMenuItem, removeEventToolStripMenuItem, eventPropertiesToolStripMenuItem });
			eventToolStripMenuItem.Name = "eventToolStripMenuItem";
			eventToolStripMenuItem.Size = new Size(48, 20);
			eventToolStripMenuItem.Text = "Event";
			// 
			// addEventToolStripMenuItem
			// 
			addEventToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { addAudioClipToolStripMenuItem });
			addEventToolStripMenuItem.Name = "addEventToolStripMenuItem";
			addEventToolStripMenuItem.Size = new Size(180, 22);
			addEventToolStripMenuItem.Text = "Add";
			// 
			// addAudioClipToolStripMenuItem
			// 
			addAudioClipToolStripMenuItem.Name = "addAudioClipToolStripMenuItem";
			addAudioClipToolStripMenuItem.Size = new Size(139, 22);
			addAudioClipToolStripMenuItem.Text = "Audio Clip...";
			addAudioClipToolStripMenuItem.Click += addAudioClipToolStripMenuItem_Click;
			// 
			// removeEventToolStripMenuItem
			// 
			removeEventToolStripMenuItem.Name = "removeEventToolStripMenuItem";
			removeEventToolStripMenuItem.ShortcutKeys = Keys.Delete;
			removeEventToolStripMenuItem.Size = new Size(180, 22);
			removeEventToolStripMenuItem.Text = "Remove";
			removeEventToolStripMenuItem.Click += removeEventToolStripMenuItem_Click;
			// 
			// eventPropertiesToolStripMenuItem
			// 
			eventPropertiesToolStripMenuItem.Name = "eventPropertiesToolStripMenuItem";
			eventPropertiesToolStripMenuItem.ShortcutKeys = Keys.Control | Keys.E;
			eventPropertiesToolStripMenuItem.Size = new Size(180, 22);
			eventPropertiesToolStripMenuItem.Text = "Properties";
			eventPropertiesToolStripMenuItem.Click += eventPropertiesToolStripMenuItem_Click;
			// 
			// debugToolStripMenuItem
			// 
			debugToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { consoleToolStripMenuItem });
			debugToolStripMenuItem.Name = "debugToolStripMenuItem";
			debugToolStripMenuItem.Size = new Size(54, 20);
			debugToolStripMenuItem.Text = "Debug";
			// 
			// consoleToolStripMenuItem
			// 
			consoleToolStripMenuItem.Name = "consoleToolStripMenuItem";
			consoleToolStripMenuItem.Size = new Size(180, 22);
			consoleToolStripMenuItem.Text = "Console";
			consoleToolStripMenuItem.Click += consoleToolStripMenuItem_Click;
			// 
			// verticalSplit
			// 
			verticalSplit.BorderStyle = BorderStyle.FixedSingle;
			verticalSplit.Dock = DockStyle.Fill;
			verticalSplit.Location = new Point(0, 24);
			verticalSplit.Name = "verticalSplit";
			verticalSplit.Orientation = Orientation.Horizontal;
			// 
			// verticalSplit.Panel1
			// 
			verticalSplit.Panel1.Controls.Add(videoSplit);
			// 
			// verticalSplit.Panel2
			// 
			verticalSplit.Panel2.Controls.Add(timelineContainer);
			verticalSplit.Size = new Size(884, 426);
			verticalSplit.SplitterDistance = 299;
			verticalSplit.TabIndex = 1;
			// 
			// videoSplit
			// 
			videoSplit.BorderStyle = BorderStyle.FixedSingle;
			videoSplit.Dock = DockStyle.Fill;
			videoSplit.Location = new Point(0, 0);
			videoSplit.Name = "videoSplit";
			// 
			// videoSplit.Panel2
			// 
			videoSplit.Panel2.Controls.Add(video);
			videoSplit.Size = new Size(884, 299);
			videoSplit.SplitterDistance = 551;
			videoSplit.TabIndex = 0;
			// 
			// video
			// 
			video.Dock = DockStyle.Fill;
			video.Location = new Point(0, 0);
			video.Name = "video";
			video.Size = new Size(327, 297);
			video.TabIndex = 0;
			// 
			// timelineContainer
			// 
			timelineContainer.AutoSize = true;
			timelineContainer.BackColor = Color.FromArgb(39, 41, 50);
			timelineContainer.Dock = DockStyle.Fill;
			timelineContainer.Location = new Point(0, 0);
			timelineContainer.MinimumSize = new Size(50, 50);
			timelineContainer.Name = "timelineContainer";
			timelineContainer.Size = new Size(882, 121);
			timelineContainer.TabIndex = 0;
			// 
			// settingsMenuItem
			// 
			settingsMenuItem.DropDownItems.AddRange(new ToolStripItem[] { audioSettingsToolStripMenuItem });
			settingsMenuItem.Name = "settingsMenuItem";
			settingsMenuItem.Size = new Size(193, 22);
			settingsMenuItem.Text = "Settings";
			// 
			// audioSettingsToolStripMenuItem
			// 
			audioSettingsToolStripMenuItem.Name = "audioSettingsToolStripMenuItem";
			audioSettingsToolStripMenuItem.Size = new Size(180, 22);
			audioSettingsToolStripMenuItem.Text = "Audio Settings...";
			audioSettingsToolStripMenuItem.Click += audioSettingsToolStripMenuItem_Click;
			// 
			// MainForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(884, 450);
			Controls.Add(verticalSplit);
			Controls.Add(menuStrip);
			MainMenuStrip = menuStrip;
			Name = "MainForm";
			Text = "Karaoke Studio";
			FormClosing += MainForm_FormClosing;
			FormClosed += MainForm_FormClosed;
			menuStrip.ResumeLayout(false);
			menuStrip.PerformLayout();
			verticalSplit.Panel1.ResumeLayout(false);
			verticalSplit.Panel2.ResumeLayout(false);
			verticalSplit.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)verticalSplit).EndInit();
			verticalSplit.ResumeLayout(false);
			videoSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)videoSplit).EndInit();
			videoSplit.ResumeLayout(false);
			ResumeLayout(false);
			PerformLayout();
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
		private ToolStripMenuItem trackPropertiesToolStripMenuItem;
		private ToolStripMenuItem addEventToolStripMenuItem;
		private ToolStripMenuItem addAudioClipToolStripMenuItem;
		private ToolStripMenuItem removeEventToolStripMenuItem;
		private ToolStripMenuItem eventPropertiesToolStripMenuItem;
		private ToolStripMenuItem redoToolStripMenuItem;
		private ToolStripMenuItem settingsMenuItem;
		private ToolStripMenuItem audioSettingsToolStripMenuItem;
	}
}