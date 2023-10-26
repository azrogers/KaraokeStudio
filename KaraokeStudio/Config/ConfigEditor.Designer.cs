namespace KaraokeStudio.Config
{
	partial class ConfigEditor
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
			this.configContainer = new System.Windows.Forms.TableLayoutPanel();
			this.SuspendLayout();
			// 
			// configContainer
			// 
			this.configContainer.AutoScroll = true;
			this.configContainer.AutoSize = true;
			this.configContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.configContainer.ColumnCount = 2;
			this.configContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 100F));
			this.configContainer.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.configContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configContainer.Location = new System.Drawing.Point(0, 0);
			this.configContainer.Name = "configContainer";
			this.configContainer.RowCount = 1;
			this.configContainer.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.configContainer.Size = new System.Drawing.Size(150, 150);
			this.configContainer.TabIndex = 1;
			// 
			// EditableConfig
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.configContainer);
			this.Name = "EditableConfig";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private TableLayoutPanel configContainer;
	}
}
