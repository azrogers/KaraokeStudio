namespace KaraokeStudio.Config.Controls
{
	partial class RangeConfigControl
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
			this.trackBar = new System.Windows.Forms.TrackBar();
			this.valueLabel = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).BeginInit();
			this.SuspendLayout();
			// 
			// trackBar
			// 
			this.trackBar.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.trackBar.Location = new System.Drawing.Point(56, 3);
			this.trackBar.Name = "trackBar";
			this.trackBar.Size = new System.Drawing.Size(144, 45);
			this.trackBar.TabIndex = 0;
			this.trackBar.ValueChanged += new System.EventHandler(this.trackBar_ValueChanged);
			// 
			// valueLabel
			// 
			this.valueLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.valueLabel.AutoSize = true;
			this.valueLabel.Location = new System.Drawing.Point(28, 18);
			this.valueLabel.Name = "valueLabel";
			this.valueLabel.Size = new System.Drawing.Size(22, 15);
			this.valueLabel.TabIndex = 1;
			this.valueLabel.Text = "0.0";
			this.valueLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// RangeConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.valueLabel);
			this.Controls.Add(this.trackBar);
			this.Name = "RangeConfigControl";
			this.Size = new System.Drawing.Size(203, 51);
			((System.ComponentModel.ISupportInitialize)(this.trackBar)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TrackBar trackBar;
		private Label valueLabel;
	}
}
