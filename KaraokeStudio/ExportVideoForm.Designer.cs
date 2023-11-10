namespace KaraokeStudio
{
	partial class ExportVideoForm
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
			this.mainSplit = new System.Windows.Forms.SplitContainer();
			this.optionsSplit = new System.Windows.Forms.SplitContainer();
			this.exportButton = new System.Windows.Forms.Button();
			this.cancelButton = new System.Windows.Forms.Button();
			this.typeComboBox = new System.Windows.Forms.ComboBox();
			this.typeLabel = new System.Windows.Forms.Label();
			this.encoderConfigEditor = new KaraokeStudio.Config.ConfigEditor();
			this.messageBox = new System.Windows.Forms.TextBox();
			this.exportProgress = new System.Windows.Forms.ProgressBar();
			((System.ComponentModel.ISupportInitialize)(this.mainSplit)).BeginInit();
			this.mainSplit.Panel1.SuspendLayout();
			this.mainSplit.Panel2.SuspendLayout();
			this.mainSplit.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.optionsSplit)).BeginInit();
			this.optionsSplit.Panel1.SuspendLayout();
			this.optionsSplit.Panel2.SuspendLayout();
			this.optionsSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// mainSplit
			// 
			this.mainSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mainSplit.Location = new System.Drawing.Point(0, 0);
			this.mainSplit.Name = "mainSplit";
			this.mainSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// mainSplit.Panel1
			// 
			this.mainSplit.Panel1.Controls.Add(this.optionsSplit);
			// 
			// mainSplit.Panel2
			// 
			this.mainSplit.Panel2.Controls.Add(this.messageBox);
			this.mainSplit.Panel2.Controls.Add(this.exportProgress);
			this.mainSplit.Size = new System.Drawing.Size(800, 450);
			this.mainSplit.SplitterDistance = 266;
			this.mainSplit.TabIndex = 0;
			// 
			// optionsSplit
			// 
			this.optionsSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.optionsSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.optionsSplit.IsSplitterFixed = true;
			this.optionsSplit.Location = new System.Drawing.Point(0, 0);
			this.optionsSplit.Name = "optionsSplit";
			this.optionsSplit.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// optionsSplit.Panel1
			// 
			this.optionsSplit.Panel1.Controls.Add(this.exportButton);
			this.optionsSplit.Panel1.Controls.Add(this.cancelButton);
			this.optionsSplit.Panel1.Controls.Add(this.typeComboBox);
			this.optionsSplit.Panel1.Controls.Add(this.typeLabel);
			this.optionsSplit.Panel1MinSize = 29;
			// 
			// optionsSplit.Panel2
			// 
			this.optionsSplit.Panel2.Controls.Add(this.encoderConfigEditor);
			this.optionsSplit.Size = new System.Drawing.Size(800, 266);
			this.optionsSplit.SplitterDistance = 29;
			this.optionsSplit.SplitterWidth = 1;
			this.optionsSplit.TabIndex = 0;
			// 
			// exportButton
			// 
			this.exportButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.exportButton.Location = new System.Drawing.Point(641, 3);
			this.exportButton.Name = "exportButton";
			this.exportButton.Size = new System.Drawing.Size(75, 23);
			this.exportButton.TabIndex = 3;
			this.exportButton.Text = "Export";
			this.exportButton.UseVisualStyleBackColor = true;
			this.exportButton.Click += new System.EventHandler(this.exportButton_Click);
			// 
			// cancelButton
			// 
			this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.cancelButton.Location = new System.Drawing.Point(722, 3);
			this.cancelButton.Name = "cancelButton";
			this.cancelButton.Size = new System.Drawing.Size(75, 23);
			this.cancelButton.TabIndex = 2;
			this.cancelButton.Text = "Cancel";
			this.cancelButton.UseVisualStyleBackColor = true;
			this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
			// 
			// typeComboBox
			// 
			this.typeComboBox.FormattingEnabled = true;
			this.typeComboBox.Location = new System.Drawing.Point(43, 3);
			this.typeComboBox.Name = "typeComboBox";
			this.typeComboBox.Size = new System.Drawing.Size(197, 23);
			this.typeComboBox.TabIndex = 1;
			this.typeComboBox.SelectedIndexChanged += new System.EventHandler(this.typeComboBox_SelectedIndexChanged);
			// 
			// typeLabel
			// 
			this.typeLabel.AutoSize = true;
			this.typeLabel.Location = new System.Drawing.Point(3, 6);
			this.typeLabel.Name = "typeLabel";
			this.typeLabel.Size = new System.Drawing.Size(34, 15);
			this.typeLabel.TabIndex = 0;
			this.typeLabel.Text = "Type:";
			// 
			// encoderConfigEditor
			// 
			this.encoderConfigEditor.Config = null;
			this.encoderConfigEditor.Dock = System.Windows.Forms.DockStyle.Fill;
			this.encoderConfigEditor.Location = new System.Drawing.Point(0, 0);
			this.encoderConfigEditor.Name = "encoderConfigEditor";
			this.encoderConfigEditor.Size = new System.Drawing.Size(800, 236);
			this.encoderConfigEditor.TabIndex = 0;
			// 
			// messageBox
			// 
			this.messageBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.messageBox.Location = new System.Drawing.Point(0, 23);
			this.messageBox.Multiline = true;
			this.messageBox.Name = "messageBox";
			this.messageBox.ReadOnly = true;
			this.messageBox.Size = new System.Drawing.Size(800, 157);
			this.messageBox.TabIndex = 1;
			// 
			// exportProgress
			// 
			this.exportProgress.Dock = System.Windows.Forms.DockStyle.Top;
			this.exportProgress.Location = new System.Drawing.Point(0, 0);
			this.exportProgress.Name = "exportProgress";
			this.exportProgress.Size = new System.Drawing.Size(800, 23);
			this.exportProgress.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
			this.exportProgress.TabIndex = 0;
			// 
			// ExportVideoForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this.mainSplit);
			this.Name = "ExportVideoForm";
			this.Text = "Export Video";
			this.mainSplit.Panel1.ResumeLayout(false);
			this.mainSplit.Panel2.ResumeLayout(false);
			this.mainSplit.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.mainSplit)).EndInit();
			this.mainSplit.ResumeLayout(false);
			this.optionsSplit.Panel1.ResumeLayout(false);
			this.optionsSplit.Panel1.PerformLayout();
			this.optionsSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.optionsSplit)).EndInit();
			this.optionsSplit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer mainSplit;
		private SplitContainer optionsSplit;
		private ComboBox typeComboBox;
		private Label typeLabel;
		private Button exportButton;
		private Button cancelButton;
		private Config.ConfigEditor encoderConfigEditor;
		private ProgressBar exportProgress;
		private TextBox messageBox;
	}
}