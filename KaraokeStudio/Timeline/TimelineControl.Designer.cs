namespace KaraokeStudio.Timeline
{
	partial class TimelineControl
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
			this.horizScroll = new System.Windows.Forms.HScrollBar();
			this.verticalScroll = new System.Windows.Forms.VScrollBar();
			this.skiaControl = new SkiaSharp.Views.Desktop.SKGLControl();
			this.verticalMinusButton = new FontAwesome.Sharp.IconButton();
			this.verticalPlusButton = new FontAwesome.Sharp.IconButton();
			this.verticalScrollLayout = new System.Windows.Forms.TableLayoutPanel();
			this.verticalButtonContainer = new System.Windows.Forms.Panel();
			this.horizScrollLayout = new System.Windows.Forms.TableLayoutPanel();
			this.horizButtonContainer = new System.Windows.Forms.Panel();
			this.horizMinusButton = new FontAwesome.Sharp.IconButton();
			this.horizPlusButton = new FontAwesome.Sharp.IconButton();
			this.verticalScrollLayout.SuspendLayout();
			this.verticalButtonContainer.SuspendLayout();
			this.horizScrollLayout.SuspendLayout();
			this.horizButtonContainer.SuspendLayout();
			this.SuspendLayout();
			// 
			// horizScroll
			// 
			this.horizScroll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.horizScroll.Location = new System.Drawing.Point(0, 0);
			this.horizScroll.Name = "horizScroll";
			this.horizScroll.Size = new System.Drawing.Size(448, 17);
			this.horizScroll.TabIndex = 0;
			this.horizScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.horizScroll_Scroll);
			// 
			// verticalScroll
			// 
			this.verticalScroll.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verticalScroll.Location = new System.Drawing.Point(0, 0);
			this.verticalScroll.Name = "verticalScroll";
			this.verticalScroll.Size = new System.Drawing.Size(17, 115);
			this.verticalScroll.TabIndex = 1;
			this.verticalScroll.Scroll += new System.Windows.Forms.ScrollEventHandler(this.verticalScroll_Scroll);
			// 
			// skiaControl
			// 
			this.skiaControl.BackColor = System.Drawing.Color.Black;
			this.skiaControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.skiaControl.Location = new System.Drawing.Point(0, 0);
			this.skiaControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
			this.skiaControl.Name = "skiaControl";
			this.skiaControl.Size = new System.Drawing.Size(500, 150);
			this.skiaControl.TabIndex = 2;
			this.skiaControl.VSync = true;
			this.skiaControl.PaintSurface += new System.EventHandler<SkiaSharp.Views.Desktop.SKPaintGLSurfaceEventArgs>(this.skiaControl_PaintSurface);
			this.skiaControl.Layout += new System.Windows.Forms.LayoutEventHandler(this.skiaControl_Layout);
			this.skiaControl.MouseDown += new System.Windows.Forms.MouseEventHandler(this.skiaControl_MouseDown);
			this.skiaControl.MouseMove += new System.Windows.Forms.MouseEventHandler(this.skiaControl_MouseMove);
			this.skiaControl.MouseUp += new System.Windows.Forms.MouseEventHandler(this.skiaControl_MouseUp);
			// 
			// verticalMinusButton
			// 
			this.verticalMinusButton.BackColor = System.Drawing.Color.WhiteSmoke;
			this.verticalMinusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.verticalMinusButton.IconChar = FontAwesome.Sharp.IconChar.CircleMinus;
			this.verticalMinusButton.IconColor = System.Drawing.Color.Black;
			this.verticalMinusButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.verticalMinusButton.IconSize = 12;
			this.verticalMinusButton.Location = new System.Drawing.Point(0, 18);
			this.verticalMinusButton.Name = "verticalMinusButton";
			this.verticalMinusButton.Size = new System.Drawing.Size(17, 17);
			this.verticalMinusButton.TabIndex = 3;
			this.verticalMinusButton.UseVisualStyleBackColor = false;
			this.verticalMinusButton.Click += new System.EventHandler(this.verticalMinusButton_Click);
			// 
			// verticalPlusButton
			// 
			this.verticalPlusButton.BackColor = System.Drawing.Color.WhiteSmoke;
			this.verticalPlusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.verticalPlusButton.IconChar = FontAwesome.Sharp.IconChar.PlusCircle;
			this.verticalPlusButton.IconColor = System.Drawing.Color.Black;
			this.verticalPlusButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.verticalPlusButton.IconSize = 12;
			this.verticalPlusButton.Location = new System.Drawing.Point(0, 0);
			this.verticalPlusButton.Name = "verticalPlusButton";
			this.verticalPlusButton.Size = new System.Drawing.Size(17, 17);
			this.verticalPlusButton.TabIndex = 2;
			this.verticalPlusButton.UseVisualStyleBackColor = false;
			this.verticalPlusButton.Click += new System.EventHandler(this.verticalPlusButton_Click);
			// 
			// verticalScrollLayout
			// 
			this.verticalScrollLayout.ColumnCount = 1;
			this.verticalScrollLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.verticalScrollLayout.Controls.Add(this.verticalButtonContainer, 0, 1);
			this.verticalScrollLayout.Controls.Add(this.verticalScroll, 0, 0);
			this.verticalScrollLayout.Dock = System.Windows.Forms.DockStyle.Right;
			this.verticalScrollLayout.Location = new System.Drawing.Point(483, 0);
			this.verticalScrollLayout.Name = "verticalScrollLayout";
			this.verticalScrollLayout.RowCount = 2;
			this.verticalScrollLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.verticalScrollLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.verticalScrollLayout.Size = new System.Drawing.Size(17, 150);
			this.verticalScrollLayout.TabIndex = 4;
			// 
			// verticalButtonContainer
			// 
			this.verticalButtonContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.verticalButtonContainer.Controls.Add(this.verticalMinusButton);
			this.verticalButtonContainer.Controls.Add(this.verticalPlusButton);
			this.verticalButtonContainer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.verticalButtonContainer.Location = new System.Drawing.Point(0, 115);
			this.verticalButtonContainer.Margin = new System.Windows.Forms.Padding(0);
			this.verticalButtonContainer.Name = "verticalButtonContainer";
			this.verticalButtonContainer.Size = new System.Drawing.Size(17, 35);
			this.verticalButtonContainer.TabIndex = 4;
			// 
			// horizScrollLayout
			// 
			this.horizScrollLayout.ColumnCount = 2;
			this.horizScrollLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.horizScrollLayout.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 35F));
			this.horizScrollLayout.Controls.Add(this.horizButtonContainer, 1, 0);
			this.horizScrollLayout.Controls.Add(this.horizScroll, 0, 0);
			this.horizScrollLayout.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.horizScrollLayout.Location = new System.Drawing.Point(0, 133);
			this.horizScrollLayout.Name = "horizScrollLayout";
			this.horizScrollLayout.RowCount = 1;
			this.horizScrollLayout.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.horizScrollLayout.Size = new System.Drawing.Size(483, 17);
			this.horizScrollLayout.TabIndex = 5;
			// 
			// horizButtonContainer
			// 
			this.horizButtonContainer.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
			this.horizButtonContainer.Controls.Add(this.horizMinusButton);
			this.horizButtonContainer.Controls.Add(this.horizPlusButton);
			this.horizButtonContainer.Location = new System.Drawing.Point(448, 0);
			this.horizButtonContainer.Margin = new System.Windows.Forms.Padding(0);
			this.horizButtonContainer.Name = "horizButtonContainer";
			this.horizButtonContainer.Size = new System.Drawing.Size(35, 17);
			this.horizButtonContainer.TabIndex = 4;
			// 
			// horizMinusButton
			// 
			this.horizMinusButton.BackColor = System.Drawing.Color.WhiteSmoke;
			this.horizMinusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.horizMinusButton.IconChar = FontAwesome.Sharp.IconChar.CircleMinus;
			this.horizMinusButton.IconColor = System.Drawing.Color.Black;
			this.horizMinusButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.horizMinusButton.IconSize = 12;
			this.horizMinusButton.Location = new System.Drawing.Point(18, 0);
			this.horizMinusButton.Name = "horizMinusButton";
			this.horizMinusButton.Size = new System.Drawing.Size(17, 17);
			this.horizMinusButton.TabIndex = 3;
			this.horizMinusButton.UseVisualStyleBackColor = false;
			this.horizMinusButton.Click += new System.EventHandler(this.horizMinusButton_Click);
			// 
			// horizPlusButton
			// 
			this.horizPlusButton.BackColor = System.Drawing.Color.WhiteSmoke;
			this.horizPlusButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.horizPlusButton.IconChar = FontAwesome.Sharp.IconChar.PlusCircle;
			this.horizPlusButton.IconColor = System.Drawing.Color.Black;
			this.horizPlusButton.IconFont = FontAwesome.Sharp.IconFont.Auto;
			this.horizPlusButton.IconSize = 12;
			this.horizPlusButton.Location = new System.Drawing.Point(0, 0);
			this.horizPlusButton.Name = "horizPlusButton";
			this.horizPlusButton.Size = new System.Drawing.Size(17, 17);
			this.horizPlusButton.TabIndex = 2;
			this.horizPlusButton.UseVisualStyleBackColor = false;
			this.horizPlusButton.Click += new System.EventHandler(this.horizPlusButton_Click);
			// 
			// TimelineControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.horizScrollLayout);
			this.Controls.Add(this.verticalScrollLayout);
			this.Controls.Add(this.skiaControl);
			this.Name = "TimelineControl";
			this.Size = new System.Drawing.Size(500, 150);
			this.verticalScrollLayout.ResumeLayout(false);
			this.verticalButtonContainer.ResumeLayout(false);
			this.horizScrollLayout.ResumeLayout(false);
			this.horizButtonContainer.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private HScrollBar horizScroll;
		private VScrollBar verticalScroll;
		private SkiaSharp.Views.Desktop.SKGLControl skiaControl;
		private FontAwesome.Sharp.IconButton verticalPlusButton;
		private FontAwesome.Sharp.IconButton verticalMinusButton;
		private TableLayoutPanel verticalScrollLayout;
		private Panel verticalButtonContainer;
		private TableLayoutPanel horizScrollLayout;
		private Panel horizButtonContainer;
		private FontAwesome.Sharp.IconButton horizMinusButton;
		private FontAwesome.Sharp.IconButton horizPlusButton;
	}
}
