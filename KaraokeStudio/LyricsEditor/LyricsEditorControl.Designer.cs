namespace KaraokeStudio.LyricsEditor
{
	partial class LyricsEditorControl
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
			this.lyricsEditorStatusStrip = new System.Windows.Forms.StatusStrip();
			this.updateLyricsButton = new System.Windows.Forms.ToolStripSplitButton();
			this.lyricsEditorStatusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// lyricsEditorStatusStrip
			// 
			this.lyricsEditorStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.updateLyricsButton});
			this.lyricsEditorStatusStrip.Location = new System.Drawing.Point(0, 464);
			this.lyricsEditorStatusStrip.Name = "lyricsEditorStatusStrip";
			this.lyricsEditorStatusStrip.Size = new System.Drawing.Size(533, 22);
			this.lyricsEditorStatusStrip.TabIndex = 0;
			this.lyricsEditorStatusStrip.Text = "statusStrip1";
			// 
			// updateLyricsButton
			// 
			this.updateLyricsButton.DropDownButtonWidth = 0;
			this.updateLyricsButton.Image = global::KaraokeStudio.Properties.Resources.RefreshIcon32;
			this.updateLyricsButton.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.updateLyricsButton.Name = "updateLyricsButton";
			this.updateLyricsButton.Size = new System.Drawing.Size(98, 20);
			this.updateLyricsButton.Text = "Update Lyrics";
			this.updateLyricsButton.ToolTipText = "Update Lyrics";
			this.updateLyricsButton.ButtonClick += new System.EventHandler(this.updateLyricsButton_ButtonClick);
			// 
			// LyricsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.lyricsEditorStatusStrip);
			this.Name = "LyricsEditorControl";
			this.Size = new System.Drawing.Size(533, 486);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LyricsEditorControl_KeyPress);
			this.lyricsEditorStatusStrip.ResumeLayout(false);
			this.lyricsEditorStatusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private StatusStrip lyricsEditorStatusStrip;
		private ToolStripSplitButton updateLyricsButton;
	}
}
