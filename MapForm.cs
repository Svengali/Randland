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

		panel1.Size = new Size( 2048, 1024 );


		Refresh();
	}

	private void panel1_Load( object sender, EventArgs e )
	{
		findBestValuesToolStripMenuItem_Click( null, null );

		panel1.FillinBitmap( _layer );

		Refresh();
	}

	private void propertyGrid1_Click( object sender, EventArgs e )
	{

		
	}

	private void _grid_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
	{
		panel1.FillinBitmap( _layer );

		Refresh();
	}

	private void mapToolStripMenuItem_Click( object sender, EventArgs e )
	{

	}

	private void findBestValuesToolStripMenuItem_Click( object sender, EventArgs e )
	{
		var startPos = new g3.Vector2f( _layer.transX, _layer.transY );

		var size = new g3.Vector2f( _layer.scaleX, _layer.scaleY );

		var step = new g3.Vector2f( 1.0f / 255.0f ) * size;

		var smallestV = float.MaxValue;
		var largestV  = float.MinValue;

		for( int y = 0; y < 1024; y += 32 )
		{
			var yF = (float)y;

			var perlinY = startPos.y + yF * step.y;

			for( int x = 0; x < 2048; x+= 32 )
			{
				var xF = (float)x;

				var perlinX = startPos.x + xF * step.x;

				var perlin = new g3.Vector2f( perlinX, perlinY );

				var vPerlin = rl.Perlin.Fbm( perlin, ( p ) => 2, ( p ) => 0.5f, ( p ) => 4 );

				smallestV = Math.Min( smallestV, vPerlin );
				largestV  = Math.Max( largestV,  vPerlin );
			}
		}

		var spread = largestV - smallestV;

		var scale = 1.0f / spread;

		var translate = 0.0f - (smallestV * scale);

		_layer = _layer with { scaleZ = scale, transZ = translate };

		_grid.SelectedObject = _layer;

		Refresh();

	}

}
