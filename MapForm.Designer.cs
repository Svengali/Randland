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
			this.panel1 = new rl.MapControl();
			this._grid = new System.Windows.Forms.PropertyGrid();
			this.SuspendLayout();
			// 
			// panel1
			// 
			this.panel1.Dock = System.Windows.Forms.DockStyle.Left;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(658, 433);
			this.panel1.TabIndex = 0;
			this.panel1.Load += new System.EventHandler(this.panel1_Load);
			// 
			// _grid
			// 
			this._grid.Dock = System.Windows.Forms.DockStyle.Right;
			this._grid.Location = new System.Drawing.Point(658, 0);
			this._grid.Name = "_grid";
			this._grid.Size = new System.Drawing.Size(176, 433);
			this._grid.TabIndex = 1;
			this._grid.PropertyValueChanged += new System.Windows.Forms.PropertyValueChangedEventHandler(this._grid_PropertyValueChanged);
			this._grid.Click += new System.EventHandler(this.propertyGrid1_Click);
			// 
			// MapForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(800, 450);
			this.Controls.Add(this._grid);
			this.Controls.Add(this.panel1);
			this.Name = "MapForm";
			this.Text = "MapForm";
			this.ResumeLayout(false);

		}

		#endregion

		private MapControl panel1;
		private System.Windows.Forms.PropertyGrid _grid;
	}
}