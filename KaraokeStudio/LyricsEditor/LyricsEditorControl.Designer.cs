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
			this.scrollBar = new System.Windows.Forms.VScrollBar();
			this.skiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
			this.SuspendLayout();
			// 
			// scrollBar
			// 
			this.scrollBar.Dock = System.Windows.Forms.DockStyle.Right;
			this.scrollBar.Location = new System.Drawing.Point(516, 0);
			this.scrollBar.Name = "scrollBar";
			this.scrollBar.Size = new System.Drawing.Size(17, 486);
			this.scrollBar.TabIndex = 0;
			this.scrollBar.Scroll += new System.Windows.Forms.ScrollEventHandler(this.scrollBar_Scroll);
			// 
			// skiaControl
			// 
			this.skiaControl.BackColor = System.Drawing.Color.Black;
			this.skiaControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.skiaControl.Location = new System.Drawing.Point(0, 0);
			this.skiaControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.skiaControl.Name = "skiaControl";
			this.skiaControl.Size = new System.Drawing.Size(516, 486);
			this.skiaControl.TabIndex = 1;
			this.skiaControl.VSync = true;
			this.skiaControl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skiaControl_PaintSurface);
			this.skiaControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.skiaControl_MouseDown);
			// 
			// LyricsEditorControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.skiaControl);
			this.Controls.Add(this.scrollBar);
			this.Name = "LyricsEditorControl";
			this.Size = new System.Drawing.Size(533, 486);
			this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.LyricsEditorControl_KeyPress);
			this.ResumeLayout(false);

		}

		#endregion

		private VScrollBar scrollBar;
		private SkiaSharp.Views.Desktop.SKGLControl skiaControl;
	}
}
