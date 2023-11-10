namespace KaraokeStudio.Config.Controls
{
	partial class SizeConfigControl
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
			this.widthControl = new System.Windows.Forms.NumericUpDown();
			this.widthLabel = new System.Windows.Forms.Label();
			this.heightLabel = new System.Windows.Forms.Label();
			this.heightControl = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.widthControl)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.heightControl)).BeginInit();
			this.SuspendLayout();
			// 
			// widthControl
			// 
			this.widthControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.widthControl.Location = new System.Drawing.Point(27, 3);
			this.widthControl.Maximum = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.widthControl.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.widthControl.Name = "widthControl";
			this.widthControl.Size = new System.Drawing.Size(120, 23);
			this.widthControl.TabIndex = 0;
			this.widthControl.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.widthControl.ValueChanged += new System.EventHandler(this.widthControl_ValueChanged);
			// 
			// widthLabel
			// 
			this.widthLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.widthLabel.AutoSize = true;
			this.widthLabel.Location = new System.Drawing.Point(3, 6);
			this.widthLabel.Name = "widthLabel";
			this.widthLabel.Size = new System.Drawing.Size(18, 15);
			this.widthLabel.TabIndex = 1;
			this.widthLabel.Text = "W";
			// 
			// heightLabel
			// 
			this.heightLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.heightLabel.AutoSize = true;
			this.heightLabel.Location = new System.Drawing.Point(153, 6);
			this.heightLabel.Name = "heightLabel";
			this.heightLabel.Size = new System.Drawing.Size(16, 15);
			this.heightLabel.TabIndex = 2;
			this.heightLabel.Text = "H";
			// 
			// heightControl
			// 
			this.heightControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.heightControl.Location = new System.Drawing.Point(175, 3);
			this.heightControl.Maximum = new decimal(new int[] {
			100000,
			0,
			0,
			0});
			this.heightControl.Minimum = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.heightControl.Name = "heightControl";
			this.heightControl.Size = new System.Drawing.Size(120, 23);
			this.heightControl.TabIndex = 3;
			this.heightControl.Value = new decimal(new int[] {
			1,
			0,
			0,
			0});
			this.heightControl.ValueChanged += new System.EventHandler(this.heightControl_ValueChanged);
			// 
			// SizeConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.heightControl);
			this.Controls.Add(this.heightLabel);
			this.Controls.Add(this.widthLabel);
			this.Controls.Add(this.widthControl);
			this.Name = "SizeConfigControl";
			this.Size = new System.Drawing.Size(298, 29);
			((System.ComponentModel.ISupportInitialize)(this.widthControl)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.heightControl)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private NumericUpDown widthControl;
		private Label widthLabel;
		private Label heightLabel;
		private NumericUpDown heightControl;
	}
}
