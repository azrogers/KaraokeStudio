namespace KaraokeStudio.Config
{
	partial class FontConfigControl
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
			this.fontLabel = new System.Windows.Forms.Label();
			this.fontButton = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// fontLabel
			// 
			this.fontLabel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.fontLabel.Location = new System.Drawing.Point(0, 0);
			this.fontLabel.Name = "fontLabel";
			this.fontLabel.Padding = new System.Windows.Forms.Padding(0, 7, 110, 4);
			this.fontLabel.Size = new System.Drawing.Size(335, 29);
			this.fontLabel.TabIndex = 0;
			this.fontLabel.Text = "label1";
			this.fontLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// fontButton
			// 
			this.fontButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.fontButton.Location = new System.Drawing.Point(228, 3);
			this.fontButton.Name = "fontButton";
			this.fontButton.Size = new System.Drawing.Size(104, 23);
			this.fontButton.TabIndex = 1;
			this.fontButton.Text = "Choose Font...";
			this.fontButton.UseVisualStyleBackColor = true;
			this.fontButton.Click += new System.EventHandler(this.fontButton_Click);
			// 
			// FontConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.fontButton);
			this.Controls.Add(this.fontLabel);
			this.Name = "FontConfigControl";
			this.Size = new System.Drawing.Size(335, 29);
			this.ResumeLayout(false);

		}

		#endregion

		private Label fontLabel;
		private Button fontButton;
	}
}
