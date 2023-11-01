namespace KaraokeStudio.Config
{
	partial class GenericConfigEditorForm
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
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.configEditor = new KaraokeStudio.Config.ConfigEditor();
			this.buttonsLayout = new System.Windows.Forms.FlowLayoutPanel();
			this.cancelButton = new System.Windows.Forms.Button();
			this.revertButton = new System.Windows.Forms.Button();
			this.applyButton = new System.Windows.Forms.Button();
			this.okButton = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.buttonsLayout.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer1
			// 
			this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainer1.IsSplitterFixed = true;
			this.splitContainer1.Location = new System.Drawing.Point(0, 0);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.configEditor);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.buttonsLayout);
			this.splitContainer1.Size = new System.Drawing.Size(525, 399);
			this.splitContainer1.SplitterDistance = 366;
			this.splitContainer1.TabIndex = 0;
			// 
			// configEditor
			// 
			this.configEditor.Config = null;
			this.configEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.configEditor.Location = new System.Drawing.Point(0, 0);
			this.configEditor.Name = "configEditor";
			this.configEditor.Size = new System.Drawing.Size(525, 366);
			this.configEditor.TabIndex = 0;
			// 
			// buttonsLayout
			// 
			this.buttonsLayout.Controls.Add(this.cancelButton);
			this.buttonsLayout.Controls.Add(this.revertButton);
			this.buttonsLayout.Controls.Add(this.applyButton);
			this.buttonsLayout.Controls.Add(this.okButton);
			this.buttonsLayout.Dock = System.Windows.Forms.DockStyle.Fill;
			this.buttonsLayout.FlowDirection = System.Windows.Forms.FlowDirection.RightToLeft;
			this.buttonsLayout.Location = new System.Drawing.Point(0, 0);
			this.buttonsLayout.Name = "buttonsLayout";
			this.buttonsLayout.Size = new System.Drawing.Size(525, 29);
			this.buttonsLayout.TabIndex = 0;
			// 
			// cancelButton
			// 
			this.cancelButton.Location = new System.Drawing.Point(447, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 0;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// revertButton
			// 
			this.revertButton.Location = new System.Drawing.Point(366, 3);
			this.revertButton.Name = "revertButton";
			this.revertButton.Size = new System.Drawing.Size(75, 23);
			this.revertButton.TabIndex = 1;
			this.revertButton.Text = "Revert";
			this.revertButton.UseVisualStyleBackColor = true;
			this.revertButton.Click += new System.EventHandler(this.revertButton_Click);
			// 
			// applyButton
			// 
			this.applyButton.Location = new System.Drawing.Point(285, 3);
			this.applyButton.Name = "applyButton";
			this.applyButton.Size = new System.Drawing.Size(75, 23);
			this.applyButton.TabIndex = 2;
			this.applyButton.Text = "Apply";
			this.applyButton.UseVisualStyleBackColor = true;
			this.applyButton.Click += new System.EventHandler(this.applyButton_Click);
			// 
			// okButton
			// 
			this.okButton.Location = new System.Drawing.Point(204, 3);
			this.okButton.Name = "okButton";
			this.okButton.Size = new System.Drawing.Size(75, 23);
			this.okButton.TabIndex = 3;
			this.okButton.Text = "OK";
			this.okButton.UseVisualStyleBackColor = true;
			this.okButton.Click += new System.EventHandler(this.okButton_Click);
			// 
			// GenericConfigEditorForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(525, 399);
			this.Controls.Add(this.splitContainer1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "GenericConfigEditorForm";
			this.Text = "GenericConfigEditor";
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.buttonsLayout.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer splitContainer1;
		private FlowLayoutPanel buttonsLayout;
		private Button cancelButton;
		private Button revertButton;
		private Button applyButton;
		private Button okButton;
		private ConfigEditor configEditor;
	}
}