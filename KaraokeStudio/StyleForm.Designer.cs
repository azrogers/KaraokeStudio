namespace KaraokeStudio
{
	partial class StyleForm
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
			this.splitContainer = new System.Windows.Forms.SplitContainer();
			this.configEditor = new KaraokeStudio.Config.ConfigEditor();
			this.importButton = new System.Windows.Forms.Button();
			this.exportButton = new System.Windows.Forms.Button();
			this.applyButton = new System.Windows.Forms.Button();
			this.revertButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.videoPanel = new System.Windows.Forms.Panel();
			this.previewSkiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
			this.splitContainer.Panel1.SuspendLayout();
			this.splitContainer.Panel2.SuspendLayout();
			this.splitContainer.SuspendLayout();
			this.videoPanel.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer
			// 
			this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer.Location = new System.Drawing.Point(0, 0);
			this.splitContainer.Name = "splitContainer";
			// 
			// splitContainer.Panel1
			// 
			this.splitContainer.Panel1.Controls.Add(this.configEditor);
			// 
			// splitContainer.Panel2
			// 
			this.splitContainer.Panel2.Controls.Add(this.importButton);
			this.splitContainer.Panel2.Controls.Add(this.exportButton);
			this.splitContainer.Panel2.Controls.Add(this.applyButton);
			this.splitContainer.Panel2.Controls.Add(this.revertButton);
			this.splitContainer.Panel2.Controls.Add(this.cancelButton);
			this.splitContainer.Panel2.Controls.Add(this.videoPanel);
			this.splitContainer.Size = new System.Drawing.Size(800, 512);
			this.splitContainer.SplitterDistance = 544;
			this.splitContainer.TabIndex = 0;
			// 
			// configEditor
			// 
			this.configEditor.AutoSize = true;
			this.configEditor.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.configEditor.Config = null;
			this.configEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configEditor.Location = new System.Drawing.Point(0, 0);
			this.configEditor.Name = "configEditor";
			this.configEditor.Size = new System.Drawing.Size(544, 512);
			this.configEditor.TabIndex = 0;
			this.configEditor.OnValueChanged += new System.Action(this.configEditor_OnValueChanged);
			// 
			// importButton
			// 
			this.importButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.importButton.Location = new System.Drawing.Point(0, 397);
			this.importButton.Name = "importButton";
			this.importButton.Size = new System.Drawing.Size(252, 23);
			this.importButton.TabIndex = 5;
			this.importButton.Text = "Import";
			this.importButton.UseVisualStyleBackColor = true;
			this.importButton.Click += new System.EventHandler(this.importButton_Click);
			// 
			// exportButton
			// 
			this.exportButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.exportButton.Location = new System.Drawing.Point(0, 420);
			this.exportButton.Name = "exportButton";
			this.exportButton.Size = new System.Drawing.Size(252, 23);
			this.exportButton.TabIndex = 4;
			this.exportButton.Text = "Export";
			this.exportButton.UseVisualStyleBackColor = true;
			this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
			// 
			// applyButton
			// 
			this.applyButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.applyButton.Location = new System.Drawing.Point(0, 443);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(252, 23);
			this.applyButton.TabIndex = 3;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// revertButton
			// 
			this.revertButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.revertButton.Location = new System.Drawing.Point(0, 466);
			this.revertButton.Name = "revertButton";
			this.revertButton.Size = new System.Drawing.Size(252, 23);
			this.revertButton.TabIndex = 2;
			this.revertButton.Text = "Revert";
			this.revertButton.UseVisualStyleBackColor = true;
			this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.cancelButton.Location = new System.Drawing.Point(0, 489);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(252, 23);
			this.cancelButton.TabIndex = 1;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// videoPanel
			// 
			this.videoPanel.BackColor = System.Drawing.SystemColors.Control;
			this.videoPanel.Controls.Add(this.previewSkiaControl);
			this.videoPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.videoPanel.Location = new System.Drawing.Point(0, 0);
			this.videoPanel.Name = "videoPanel";
			this.videoPanel.Size = new System.Drawing.Size(252, 512);
			this.videoPanel.TabIndex = 6;
			this.videoPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.videoPanel_Paint);
			// 
			// previewSkiaControl
			// 
			this.previewSkiaControl.BackColor = System.Drawing.SystemColors.Control;
			this.previewSkiaControl.Location = new System.Drawing.Point(0, 160);
			this.previewSkiaControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.previewSkiaControl.Name = "previewSkiaControl";
			this.previewSkiaControl.Size = new System.Drawing.Size(252, 193);
			this.previewSkiaControl.TabIndex = 1;
			this.previewSkiaControl.VSync = true;
			this.previewSkiaControl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.previewSkiaControl_PaintSurface);
			// 
			// StyleForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 512);
			this.Controls.Add(this.splitContainer);
			this.Name = "StyleForm";
			this.Text = "Style";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.StyleForm_FormClosing);
			this.splitContainer.Panel1.ResumeLayout(false);
			this.splitContainer.Panel1.PerformLayout();
			this.splitContainer.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
			this.splitContainer.ResumeLayout(false);
			this.videoPanel.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer splitContainer;
		private Button applyButton;
		private Button revertButton;
		private Button cancelButton;
		private Button importButton;
		private Button exportButton;
		private Panel videoPanel;
		private SkiaSharp.Views.Desktop.SKGLControl previewSkiaControl;
		private Config.ConfigEditor configEditor;
	}
}