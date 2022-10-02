namespace rl
{
	partial class MapForm
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose( bool disposing )
		{
			if( disposing && (components != null) )
			{
				components.Dispose();
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this._grid = new System.Windows.Forms.PropertyGrid();
			this.menuStrip1 = new System.Windows.Forms.MenuStrip();
			this.mapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.findBestValuesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.resetWorldOriginToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this._status = new System.Windows.Forms.StatusStrip();
			this._statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this._statusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
			this._mapControl = new MapControl();
			this.menuStrip1.SuspendLayout();
			this._status.SuspendLayout();
			this.SuspendLayout();
			// 
			// _grid
			// 
			this._grid.Dock = System.Windows.Forms.DockStyle.Right;
			this._grid.Location = new System.Drawing.Point(804, 24);
			this._grid.Name = "_grid";
			this._grid.Size = new System.Drawing.Size(170, 506);
			this._grid.TabIndex = 1;
			this._grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this._grid_PropertyValueChanged);
			this._grid.Click += new System.EventHandler(this.propertyGrid1_Click);
			// 
			// menuStrip1
			// 
			this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mapToolStripMenuItem});
			this.menuStrip1.Location = new System.Drawing.Point(0, 0);
			this.menuStrip1.Name = "menuStrip1";
			this.menuStrip1.Size = new System.Drawing.Size(974, 24);
			this.menuStrip1.TabIndex = 2;
			this.menuStrip1.Text = "menuStrip1";
			// 
			// mapToolStripMenuItem
			// 
			this.mapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.findBestValuesToolStripMenuItem,
            this.resetWorldOriginToolStripMenuItem});
			this.mapToolStripMenuItem.Name = "mapToolStripMenuItem";
			this.mapToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
			this.mapToolStripMenuItem.Text = "Map";
			this.mapToolStripMenuItem.Click += new System.EventHandler(this.mapToolStripMenuItem_Click);
			// 
			// findBestValuesToolStripMenuItem
			// 
			this.findBestValuesToolStripMenuItem.Name = "findBestValuesToolStripMenuItem";
			this.findBestValuesToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.findBestValuesToolStripMenuItem.Text = "Find Best Values";
			this.findBestValuesToolStripMenuItem.Click += new System.EventHandler(this.findBestValuesToolStripMenuItem_Click);
			// 
			// resetWorldOriginToolStripMenuItem
			// 
			this.resetWorldOriginToolStripMenuItem.Name = "resetWorldOriginToolStripMenuItem";
			this.resetWorldOriginToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
			this.resetWorldOriginToolStripMenuItem.Text = "Reset World Origin";
			this.resetWorldOriginToolStripMenuItem.Click += new System.EventHandler(this.resetWorldOriginToolStripMenuItem_Click);
			// 
			// _status
			// 
			this._status.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this._statusLabel,
            this._statusLabel2});
			this._status.Location = new System.Drawing.Point(0, 530);
			this._status.Name = "_status";
			this._status.Size = new System.Drawing.Size(974, 22);
			this._status.TabIndex = 3;
			this._status.Text = "Status";
			this._status.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this._status_ItemClicked);
			// 
			// _statusLabel
			// 
			this._statusLabel.Name = "_statusLabel";
			this._statusLabel.Size = new System.Drawing.Size(27, 17);
			this._statusLabel.Text = "Test";
			// 
			// _statusLabel2
			// 
			this._statusLabel2.Name = "_statusLabel2";
			this._statusLabel2.Size = new System.Drawing.Size(77, 17);
			this._statusLabel2.Text = "_statusLabel2";
			this._statusLabel2.Click += new System.EventHandler(this.toolStripStatusLabel2_Click);
			// 
			// _mapControl
			// 
			this._mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
			this._mapControl.Location = new System.Drawing.Point(0, 24);
			this._mapControl.Name = "_mapControl";
			this._mapControl.Size = new System.Drawing.Size(804, 506);
			this._mapControl.TabIndex = 4;
			// 
			// MapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(974, 552);
			this.Controls.Add(this._mapControl);
			this.Controls.Add(this._grid);
			this.Controls.Add(this._status);
			this.Controls.Add(this.menuStrip1);
			this.MainMenuStrip = this.menuStrip1;
			this.Name = "MapForm";
			this.Text = "MapForm";
			this.Load += new System.EventHandler(this.MapForm_Load);
			this.menuStrip1.ResumeLayout(false);
			this.menuStrip1.PerformLayout();
			this._status.ResumeLayout(false);
			this._status.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.PropertyGrid _grid;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem mapToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem findBestValuesToolStripMenuItem;
		private System.Windows.Forms.StatusStrip _status;
		private System.Windows.Forms.ToolStripMenuItem resetWorldOriginToolStripMenuItem;
		private System.Windows.Forms.ToolStripStatusLabel _statusLabel;
		private System.Windows.Forms.ToolStripStatusLabel _statusLabel2;
		private MapControl _mapControl;
	}
}