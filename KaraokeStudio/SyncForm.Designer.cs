namespace KaraokeStudio
{
	partial class SyncForm
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

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.video = new KaraokeStudio.Video.KaraokeVideoControl();
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.syncButtonContainer = new System.Windows.Forms.Panel();
			this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
			this.breakButton = new System.Windows.Forms.Button();
			this.syncButton = new System.Windows.Forms.Button();
			this.undoWordButton = new System.Windows.Forms.Button();
			this.undoLineButton = new System.Windows.Forms.Button();
			this.rightSplit = new System.Windows.Forms.SplitContainer();
			this.lyricsBox = new System.Windows.Forms.TextBox();
			this.applyButton = new System.Windows.Forms.Button();
			this.revertButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.syncButtonContainer.SuspendLayout();
			this.flowLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rightSplit)).BeginInit();
			this.rightSplit.Panel1.SuspendLayout();
			this.rightSplit.Panel2.SuspendLayout();
			this.rightSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// video
			// 
			this.video.AutoSize = true;
			this.video.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.video.Dock = System.Windows.Forms.DockStyle.Fill;
			this.video.Location = new System.Drawing.Point(0, 0);
			this.video.Name = "video";
			this.video.Padding = new System.Windows.Forms.Padding(0, 0, 0, 100);
			this.video.Size = new System.Drawing.Size(508, 450);
			this.video.TabIndex = 0;
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.syncButtonContainer);
			this.splitContainer.Panel1.Controls.Add(this.video);
			this.splitContainer.Panel1MinSize = 330;
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.rightSplit);
			this.splitContainer.Panel2MinSize = 100;
			this.splitContainer.Size = new System.Drawing.Size(802, 450);
			this.splitContainer.SplitterDistance = 508;
			this.splitContainer.TabIndex = 1;
			// 
			// syncButtonContainer
			// 
			this.syncButtonContainer.Controls.Add(this.flowLayoutPanel);
			this.syncButtonContainer.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.syncButtonContainer.Location = new System.Drawing.Point(0, 386);
			this.syncButtonContainer.Name = "syncButtonContainer";
			this.syncButtonContainer.Size = new System.Drawing.Size(508, 64);
			this.syncButtonContainer.TabIndex = 1;
			// 
			// flowLayoutPanel
			// 
			this.flowLayoutPanel.Controls.Add(this.breakButton);
			this.flowLayoutPanel.Controls.Add(this.syncButton);
			this.flowLayoutPanel.Controls.Add(this.undoWordButton);
			this.flowLayoutPanel.Controls.Add(this.undoLineButton);
			this.flowLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.flowLayoutPanel.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.flowLayoutPanel.Location = new System.Drawing.Point(0, 0);
			this.flowLayoutPanel.Name = "flowLayoutPanel";
			this.flowLayoutPanel.Size = new System.Drawing.Size(508, 64);
			this.flowLayoutPanel.TabIndex = 1;
			this.flowLayoutPanel.WrapContents = false;
			// 
			// breakButton
			// 
			this.breakButton.AutoSize = true;
			this.breakButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.breakButton.Location = new System.Drawing.Point(430, 3);
			this.breakButton.MinimumSize = new System.Drawing.Size(75, 58);
			this.breakButton.Name = "breakButton";
			this.breakButton.Size = new System.Drawing.Size(75, 58);
			this.breakButton.TabIndex = 0;
			this.breakButton.Text = "Break";
			this.breakButton.UseVisualStyleBackColor = true;
			this.breakButton.Click += new System.EventHandler(this.breakButton_Click);
			// 
			// syncButton
			// 
			this.syncButton.AutoSize = true;
			this.syncButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.syncButton.Location = new System.Drawing.Point(349, 3);
			this.syncButton.MinimumSize = new System.Drawing.Size(75, 58);
			this.syncButton.Name = "syncButton";
			this.syncButton.Size = new System.Drawing.Size(75, 58);
			this.syncButton.TabIndex = 1;
			this.syncButton.Text = "Sync";
			this.syncButton.UseVisualStyleBackColor = true;
			this.syncButton.Click += new System.EventHandler(this.syncButton_Click);
			// 
			// undoWordButton
			// 
			this.undoWordButton.AutoSize = true;
			this.undoWordButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.undoWordButton.Location = new System.Drawing.Point(265, 3);
			this.undoWordButton.MinimumSize = new System.Drawing.Size(75, 58);
			this.undoWordButton.Name = "undoWordButton";
			this.undoWordButton.Size = new System.Drawing.Size(78, 58);
			this.undoWordButton.TabIndex = 2;
			this.undoWordButton.Text = "Undo Word";
			this.undoWordButton.UseVisualStyleBackColor = true;
			this.undoWordButton.Click += new System.EventHandler(this.undoWordButton_Click);
			// 
			// undoLineButton
			// 
			this.undoLineButton.AutoSize = true;
			this.undoLineButton.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.undoLineButton.Location = new System.Drawing.Point(184, 3);
			this.undoLineButton.MinimumSize = new System.Drawing.Size(75, 58);
			this.undoLineButton.Name = "undoLineButton";
			this.undoLineButton.Size = new System.Drawing.Size(75, 58);
			this.undoLineButton.TabIndex = 3;
			this.undoLineButton.Text = "Undo Line";
			this.undoLineButton.UseVisualStyleBackColor = true;
			this.undoLineButton.Click += new System.EventHandler(this.undoLineButton_Click);
			// 
			// rightSplit
			// 
			this.rightSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rightSplit.IsSplitterFixed = true;
			this.rightSplit.Location = new System.Drawing.Point(0, 0);
			this.rightSplit.Name = "rightSplit";
			this.rightSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// rightSplit.Panel1
			// 
			this.rightSplit.Panel1.Controls.Add(this.lyricsBox);
			// 
			// rightSplit.Panel2
			// 
			this.rightSplit.Panel2.Controls.Add(this.applyButton);
			this.rightSplit.Panel2.Controls.Add(this.revertButton);
			this.rightSplit.Panel2.Controls.Add(this.cancelButton);
			this.rightSplit.Panel2MinSize = 72;
			this.rightSplit.Size = new System.Drawing.Size(290, 450);
			this.rightSplit.SplitterDistance = 374;
			this.rightSplit.TabIndex = 5;
			// 
			// lyricsBox
			// 
			this.lyricsBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.lyricsBox.Location = new System.Drawing.Point(0, 0);
			this.lyricsBox.Multiline = true;
			this.lyricsBox.Name = "lyricsBox";
			this.lyricsBox.ReadOnly = true;
			this.lyricsBox.Size = new System.Drawing.Size(290, 374);
			this.lyricsBox.TabIndex = 0;
			// 
			// applyButton
			// 
			this.applyButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.applyButton.Location = new System.Drawing.Point(0, 3);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(290, 23);
			this.applyButton.TabIndex = 6;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// revertButton
			// 
			this.revertButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.revertButton.Location = new System.Drawing.Point(0, 26);
			this.revertButton.Name = "revertButton";
			this.revertButton.Size = new System.Drawing.Size(290, 23);
			this.revertButton.TabIndex = 5;
			this.revertButton.Text = "Revert";
			this.revertButton.UseVisualStyleBackColor = true;
			this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.cancelButton.Location = new System.Drawing.Point(0, 49);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(290, 23);
			this.cancelButton.TabIndex = 4;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// SyncForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(802, 450);
			this.Controls.Add(this.splitContainer);
			this.Name = "SyncForm";
			this.Text = "Sync Lyrics";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SyncForm_FormClosing);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel1.PerformLayout();
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.syncButtonContainer.ResumeLayout(false);
			this.flowLayoutPanel.ResumeLayout(false);
			this.flowLayoutPanel.PerformLayout();
			this.rightSplit.Panel1.ResumeLayout(false);
			this.rightSplit.Panel1.PerformLayout();
			this.rightSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rightSplit)).EndInit();
			this.rightSplit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private Video.KaraokeVideoControl video;
		private SplitContainer splitContainer;
		private Panel syncButtonContainer;
		private TextBox lyricsBox;
		private FlowLayoutPanel flowLayoutPanel;
		private Button breakButton;
		private Button syncButton;
		private Button undoWordButton;
		private Button undoLineButton;
		private SplitContainer rightSplit;
		private Button applyButton;
		private Button revertButton;
		private Button cancelButton;
	}
}