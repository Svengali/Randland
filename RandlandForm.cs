using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rl;

public partial class RandlandForm : Form
{
	public RandlandForm()
	{
		InitializeComponent( );
	}

	// @@@ TODO :: Make this update the node tree, not clear and replace
	public void CreateNewNodes()
	{
		_tvMethods.Nodes.Clear();

		try
		{
			Monitor.Enter( Randland.s_functions );

			foreach( var family in Randland.s_functions )
			{
				TreeNode tnFamily = new( family.Key );
				tnFamily.Name = family.Key;

				//* Map refactor
				foreach( var mapExp in family.Value.MapExp )
				{
					TreeNode tnFn = new( mapExp.Name );
					tnFn.Name = mapExp.Name;
					tnFn.Tag = mapExp;

					tnFamily.Nodes.Add( tnFn );
				}
				//*/

				_tvMethods.Nodes.Add( tnFamily );
			}
		}
		finally
		{
			Monitor.Exit( Randland.s_functions );
		}
	}

	private void Main_Load( object sender, EventArgs e )
	{
		CreateNewNodes();
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
		//var cat  = e.Node.Parent.Name;
		//var name = e.Node.Name;

		var mapExp = (MapExp)e.Node.Tag;

		if( mapExp == null )
		{
			return;
		}

		var map = new Map( MapLayer.Perlin with { Fn = mapExp.Fn } );

		var mapForm = new MapForm( map );

		Randland.AddMapForm( mapForm._mapControl, e.Node );

		mapForm.Text = $"{e.Node.Parent.Name} .:. {e.Node.Name}";

		mapForm.Show( this );
	}
}
