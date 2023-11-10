namespace KaraokeStudio.Config.Controls
{
	partial class ColorConfigControl
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
			this.colorDisplayPanel = new System.Windows.Forms.Panel();
			this.colorPickButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// colorDisplayPanel
			// 
			this.colorDisplayPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.colorDisplayPanel.BackColor = System.Drawing.SystemColors.Desktop;
			this.colorDisplayPanel.Location = new System.Drawing.Point(3, 3);
			this.colorDisplayPanel.Name = "colorDisplayPanel";
			this.colorDisplayPanel.Size = new System.Drawing.Size(131, 23);
			this.colorDisplayPanel.TabIndex = 0;
			this.colorDisplayPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.colorDisplayPanel_Paint);
			// 
			// colorPickButton
			// 
			this.colorPickButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.colorPickButton.Location = new System.Drawing.Point(140, 3);
			this.colorPickButton.Name = "colorPickButton";
			this.colorPickButton.Size = new System.Drawing.Size(114, 23);
			this.colorPickButton.TabIndex = 1;
			this.colorPickButton.Text = "Choose Color...";
			this.colorPickButton.UseVisualStyleBackColor = true;
			this.colorPickButton.Click += new System.EventHandler(this.colorPickButton_Click);
			// 
			// ColorConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.colorPickButton);
			this.Controls.Add(this.colorDisplayPanel);
			this.Name = "ColorConfigControl";
			this.Size = new System.Drawing.Size(257, 29);
			this.ResumeLayout(false);

		}

		#endregion

		private Panel colorDisplayPanel;
		private Button colorPickButton;
	}
}
