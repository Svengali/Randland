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

public partial class RandlandForm : Form
{
	public RandlandForm()
	{
		InitializeComponent( );
	}

	private void Main_Load( object sender, EventArgs e )
	{
		foreach( var family in Randland.s_functions )
		{
			TreeNode tnFamily = new( family.Key );

			//* Map refactor
			foreach( var mapExp in family.Value.MapExp )
			{
				TreeNode tnFn = new( mapExp.Name );
				tnFn.Tag = mapExp;

				tnFamily.Nodes.Add( tnFn );
			}
			//*/

			_tvMethods.Nodes.Add( tnFamily );
		}
	}

	private void newMapToolStripMenuItem_Click( object sender, EventArgs e )
	{
		/*
		var mapForm = new MapForm();
		mapForm.Show( this );
		*/
	}

	private void _tvMethods_NodeMouseDoubleClick( object sender, TreeNodeMouseClickEventArgs e )
	{
		var mapExp = (MapExp)e.Node.Tag;

		var map = new Map( MapLayer.Perlin with { Fn = mapExp.Fn } );

		var mapForm = new MapForm( map );
		mapForm.Show( this );
	}
}
