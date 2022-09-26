using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rl;

public partial class MapForm : Form
{
	MapLayer _layer = new ( 1.0f, 1.0f, 1.0f,  1.0f, 1.0f, 1.0f );

	public MapForm()
	{
		InitializeComponent();

		_grid.SelectedObject = _layer;
	}

	private void panel1_Load( object sender, EventArgs e )
	{

	}

	private void propertyGrid1_Click( object sender, EventArgs e )
	{

		
	}

	private void _grid_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
	{
		this.panel1.FillinBitmap( _layer );

		Refresh();
	}
}
