namespace KaraokeStudio.Config.Controls
{
	partial class NumericConfigControl
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
			this.numericUpDown = new System.Windows.Forms.NumericUpDown();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDown
			// 
			this.numericUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.numericUpDown.Location = new System.Drawing.Point(0, 0);
			this.numericUpDown.Name = "numericUpDown";
			this.numericUpDown.Size = new System.Drawing.Size(150, 23);
			this.numericUpDown.TabIndex = 0;
			this.numericUpDown.ValueChanged += new System.EventHandler(this.numericUpDown_ValueChanged);
			// 
			// NumericConfigControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.Controls.Add(this.numericUpDown);
			this.Name = "NumericConfigControl";
			this.Size = new System.Drawing.Size(153, 26);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDown)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private NumericUpDown numericUpDown;
	}
}
