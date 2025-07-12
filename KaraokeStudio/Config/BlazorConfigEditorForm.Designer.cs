namespace KaraokeStudio.Config
{
	partial class BlazorConfigEditorForm
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
			splitContainer1 = new SplitContainer();
			buttonsLayout = new FlowLayoutPanel();
			cancelButton = new Button();
			revertButton = new Button();
			applyButton = new Button();
			okButton = new Button();
			blazorView = new Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView();
			((System.ComponentModel.ISupportInitialize)splitContainer1).BeginInit();
			splitContainer1.Panel1.SuspendLayout();
			splitContainer1.Panel2.SuspendLayout();
			splitContainer1.SuspendLayout();
			buttonsLayout.SuspendLayout();
			SuspendLayout();
			// 
			// splitContainer1
			// 
			splitContainer1.Dock = DockStyle.Fill;
			splitContainer1.FixedPanel = FixedPanel.Panel2;
			splitContainer1.IsSplitterFixed = true;
			splitContainer1.Location = new Point(0, 0);
			splitContainer1.Name = "splitContainer1";
			splitContainer1.Orientation = Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			splitContainer1.Panel1.Controls.Add(blazorView);
			// 
			// splitContainer1.Panel2
			// 
			splitContainer1.Panel2.Controls.Add(buttonsLayout);
			splitContainer1.Size = new Size(525, 399);
			splitContainer1.SplitterDistance = 366;
			splitContainer1.TabIndex = 0;
			// 
			// buttonsLayout
			// 
			buttonsLayout.Controls.Add(cancelButton);
			buttonsLayout.Controls.Add(revertButton);
			buttonsLayout.Controls.Add(applyButton);
			buttonsLayout.Controls.Add(okButton);
			buttonsLayout.Dock = DockStyle.Fill;
			buttonsLayout.FlowDirection = FlowDirection.RightToLeft;
			buttonsLayout.Location = new Point(0, 0);
			buttonsLayout.Name = "buttonsLayout";
			buttonsLayout.Size = new Size(525, 29);
			buttonsLayout.TabIndex = 0;
			// 
			// cancelButton
			// 
			cancelButton.Location = new Point(447, 3);
			cancelButton.Name = "cancelButton";
			cancelButton.Size = new Size(75, 23);
			cancelButton.TabIndex = 0;
			cancelButton.Text = "Cancel";
			cancelButton.UseVisualStyleBackColor = true;
			cancelButton.Click += cancelButton_Click;
			// 
			// revertButton
			// 
			revertButton.Location = new Point(366, 3);
			revertButton.Name = "revertButton";
			revertButton.Size = new Size(75, 23);
			revertButton.TabIndex = 1;
			revertButton.Text = "Revert";
			revertButton.UseVisualStyleBackColor = true;
			revertButton.Click += revertButton_Click;
			// 
			// applyButton
			// 
			applyButton.Location = new Point(285, 3);
			applyButton.Name = "applyButton";
			applyButton.Size = new Size(75, 23);
			applyButton.TabIndex = 2;
			applyButton.Text = "Apply";
			applyButton.UseVisualStyleBackColor = true;
			applyButton.Click += applyButton_Click;
			// 
			// okButton
			// 
			okButton.Location = new Point(204, 3);
			okButton.Name = "okButton";
			okButton.Size = new Size(75, 23);
			okButton.TabIndex = 3;
			okButton.Text = "OK";
			okButton.UseVisualStyleBackColor = true;
			okButton.Click += okButton_Click;
			// 
			// blazorView
			// 
			blazorView.Dock = DockStyle.Fill;
			blazorView.Location = new Point(0, 0);
			blazorView.Name = "blazorView";
			blazorView.Size = new Size(525, 366);
			blazorView.StartPath = "/";
			blazorView.TabIndex = 0;
			blazorView.Text = "WebView";
			// 
			// BlazorConfigEditorForm
			// 
			AutoScaleDimensions = new SizeF(7F, 15F);
			AutoScaleMode = AutoScaleMode.Font;
			ClientSize = new Size(525, 399);
			Controls.Add(splitContainer1);
			FormBorderStyle = FormBorderStyle.SizableToolWindow;
			Name = "BlazorConfigEditorForm";
			Text = "GenericConfigEditor";
			splitContainer1.Panel1.ResumeLayout(false);
			splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)splitContainer1).EndInit();
			splitContainer1.ResumeLayout(false);
			buttonsLayout.ResumeLayout(false);
			ResumeLayout(false);
		}

		#endregion

		private SplitContainer splitContainer1;
		private FlowLayoutPanel buttonsLayout;
		private Button cancelButton;
		private Button revertButton;
		private Button applyButton;
		private Button okButton;
		private Microsoft.AspNetCore.Components.WebView.WindowsForms.BlazorWebView blazorView;
	}
}