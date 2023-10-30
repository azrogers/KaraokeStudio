namespace KaraokeStudio.Timeline
{
	partial class TimelineContainerControl
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
			this.timelineSplit = new System.Windows.Forms.SplitContainer();
			this.headersContainer = new System.Windows.Forms.Panel();
			this.timeline = new KaraokeStudio.Timeline.TimelineControl();
			((System.ComponentModel.ISupportInitialize)(this.timelineSplit)).BeginInit();
			this.timelineSplit.Panel1.SuspendLayout();
			this.timelineSplit.Panel2.SuspendLayout();
			this.timelineSplit.SuspendLayout();
			this.SuspendLayout();
			// 
			// timelineSplit
			// 
			this.timelineSplit.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timelineSplit.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.timelineSplit.Location = new System.Drawing.Point(0, 0);
			this.timelineSplit.Name = "timelineSplit";
			// 
			// timelineSplit.Panel1
			// 
			this.timelineSplit.Panel1.Controls.Add(this.headersContainer);
			// 
			// timelineSplit.Panel2
			// 
			this.timelineSplit.Panel2.Controls.Add(this.timeline);
			this.timelineSplit.Size = new System.Drawing.Size(1010, 185);
			this.timelineSplit.SplitterDistance = 346;
			this.timelineSplit.TabIndex = 2;
			// 
			// headersContainer
			// 
			this.headersContainer.AutoSize = true;
			this.headersContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.headersContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.headersContainer.Location = new System.Drawing.Point(0, 0);
			this.headersContainer.Name = "headersContainer";
			this.headersContainer.Size = new System.Drawing.Size(346, 185);
			this.headersContainer.TabIndex = 0;
			this.headersContainer.SizeChanged += new System.EventHandler(this.headersContainer_SizeChanged);
			this.headersContainer.Layout += new System.Windows.Forms.LayoutEventHandler(this.headersContainer_Layout);
			// 
			// timeline
			// 
			this.timeline.Dock = System.Windows.Forms.DockStyle.Fill;
			this.timeline.Location = new System.Drawing.Point(0, 0);
			this.timeline.Name = "timeline";
			this.timeline.Size = new System.Drawing.Size(660, 185);
			this.timeline.TabIndex = 0;
			// 
			// TimelineContainerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoSize = true;
			this.Controls.Add(this.timelineSplit);
			this.MinimumSize = new System.Drawing.Size(50, 50);
			this.Name = "TimelineContainerControl";
			this.Size = new System.Drawing.Size(1010, 185);
			this.timelineSplit.Panel1.ResumeLayout(false);
			this.timelineSplit.Panel1.PerformLayout();
			this.timelineSplit.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.timelineSplit)).EndInit();
			this.timelineSplit.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainer timelineSplit;
		private TimelineControl timeline;
		private Panel headersContainer;
	}
}
