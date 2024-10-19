namespace KaraokeStudio.Config.Controls
{
	partial class FileConfigControl
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
			fontButton = new Button();
			fileBox = new TextBox();
			SuspendLayout();
			// 
			// fontButton
			// 
			fontButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			fontButton.Location = new Point(228, 4);
			fontButton.Name = "fontButton";
			fontButton.Size = new Size(104, 23);
			fontButton.TabIndex = 1;
			fontButton.Text = "Choose File...";
			fontButton.UseVisualStyleBackColor = true;
			fontButton.Click += fileButton_Click;
			// 
			// fileBox
			// 
			fileBox.Anchor = AnchorStyles.Top | AnchorStyles.Right;
			fileBox.Location = new Point(3, 4);
			fileBox.Name = "fileBox";
			fileBox.Size = new Size(219, 23);
			fileBox.TabIndex = 2;
			fileBox.TextChanged += fileBox_TextChanged;
			// 
			// FileConfigControl
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			AutoSize = true;
			AutoSizeMode = AutoSizeMode.GrowAndShrink;
			Controls.Add(fileBox);
			Controls.Add(fontButton);
			Name = "FileConfigControl";
			Size = new Size(335, 30);
			ResumeLayout(false);
			PerformLayout();
		}

		#endregion
		private Button fontButton;
		private TextBox fileBox;
	}
}
