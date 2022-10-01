﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace rl;

public record MapInfo(MapLayer Layer, MapViewInfo ViewInfo);

/*

public class PropDesc : PropertyDescriptor
{
	public PropDesc( MemberDescriptor descr ) : base( descr )
	{
	}

	public PropDesc( string name, Attribute[] attrs ) : base( name, attrs )
	{
	}

	public PropDesc( MemberDescriptor descr, Attribute[] attrs ) : base( descr, attrs )
	{
	}

	public override Type ComponentType => throw new NotImplementedException();

	public override bool IsReadOnly => throw new NotImplementedException();

	public override Type PropertyType => throw new NotImplementedException();

	public override bool CanResetValue( object component )
	{
		throw new NotImplementedException();
	}

	public override object GetValue( object component )
	{
		throw new NotImplementedException();
	}

	public override void ResetValue( object component )
	{
		throw new NotImplementedException();
	}

	public override void SetValue( object component, object value )
	{
		throw new NotImplementedException();
	}

	public override bool ShouldSerializeValue( object component )
	{
		throw new NotImplementedException();
	}
}

public class LayerTab : PropertyTab
{
	public override string TabName { get; }

	public override PropertyDescriptorCollection GetProperties( object component, Attribute[] attributes )
	{
		return _props;
	}

	PropertyDescriptorCollection _props = new(new [] { new PropDesc() });
}

*/

public partial class MapForm : Form, IMapView
{
	Map _map = new Map( new MapLayer( (v) => 0.5f, 1.0f, 1.0f, 1.0f,  1.0f, 1.0f, 0.5f ) );


	public MapForm() : this( new Map( new MapLayer( ( v ) => 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f ) ) )
	{
	}

	public MapForm( Map map )
	{
		_map = map;

		InitializeComponent();

		//_grid.PropertyTabs.AddTabType( typeof( PropertyTab ), PropertyTabScope.Global );
		//_grid.PropertyTabs.AddTabType( typeof( PropertyTab ), PropertyTabScope.Global );

		_grid.SelectedObject = _map.Layer;
		//_grid.SelectedObjects = new[] { (object)_map.Layer, (object)panel1._viewInfo };

		panel1.Size = new Size( 2048, 1024 );

		Invalidate();
	}


	public void DoUpdate( Func<g3.Vector2f, float> Fn )
	{
		_map = _map with { Layer = _map.Layer with { Fn = Fn } };
		panel1.DoUpdate( Fn );
	}

	private void panel1_Load( object sender, EventArgs e )
	{
		//findBestValuesToolStripMenuItem_Click( null, null );

		Invalidate();
	}

	private void propertyGrid1_Click( object sender, EventArgs e )
	{

		
	}

	private void _grid_PropertyValueChanged( object s, PropertyValueChangedEventArgs e )
	{
		panel1._map = _map;

		panel1.DoUpdate( _map.Layer.Fn );

		panel1.Invalidate();
	}

	private void mapToolStripMenuItem_Click( object sender, EventArgs e )
	{

	}

	private void findBestValuesToolStripMenuItem_Click( object sender, EventArgs e )
	{
		// @@@ TODO :: Replace this with shared code from the 
		var startPos = new g3.Vector2f( _map.Layer.transX, _map.Layer.transY );

		var size = new g3.Vector2f( _map.Layer.scaleX, _map.Layer.scaleY );

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

				var vPerlin = _map.Layer.Fn( perlin );

				//var vPerlin = rl.Perlin.Fbm( perlin, ( p ) => 2, ( p ) => 0.5f, ( p ) => 4 );

				smallestV = Math.Min( smallestV, vPerlin );
				largestV  = Math.Max( largestV,  vPerlin );
			}
		}

		var spread = Math.Max( 0.0001f, largestV - smallestV );

		var scale = 1.0f / spread;

		var translate = 0.0f - (smallestV * scale);

		var newLayer = _map.Layer with { scaleZ = scale, transZ = translate };

		_map = _map with { Layer = newLayer };

		_grid.SelectedObject = _map.Layer;
		//_grid.SelectedObjects = new[] { (object)_map.Layer, (object)panel1._viewInfo };


		Invalidate();

	}

}
