using System;
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

	//Custom Control
	//public MapControl _mapControl;

	public MapControl MapControl => _mapControl;



	public MapForm() : this( new Map( new MapLayer( ( v ) => 0.5f, 1.0f, 1.0f, 1.0f, 1.0f, 1.0f, 0.5f ) ) )
	{
	}

	public MapForm( Map map )
	{
		_map = map;

		/*
		_mapControl = new MapControl();

		_mapControl.Dock = System.Windows.Forms.DockStyle.Fill;
		_mapControl.Location = new System.Drawing.Point( 0, 24 );
		_mapControl.Name = "_mapControl";
		_mapControl.Size = new System.Drawing.Size( 804, 506 );
		_mapControl.TabIndex = 4;
		*/


		InitializeComponent();

		MapControl.FnChangeStatus = (str) => {
			_statusLabel.Text = str;
		};

		_mapControl._map = _map;

		//_grid.PropertyTabs.AddTabType( typeof( PropertyTab ), PropertyTabScope.Global );
		//_grid.PropertyTabs.AddTabType( typeof( PropertyTab ), PropertyTabScope.Global );

		_grid.SelectedObject = _map.Layer;
		//_grid.SelectedObjects = new[] { (object)_map.Layer, (object)_mapControl._viewInfo };

		_mapControl.Size = new Size( Map.Max.X, Map.Max.Y );

		Invalidate();
	}

	public void DoUpdate( Func<math.Vec2, float> Fn )
	{
		_map = _map with { Layer = _map.Layer with { Fn = Fn } };
		_mapControl.DoUpdate( Fn );
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
		_mapControl._map = _map;

		_mapControl.DoUpdate( _map.Layer.Fn );

		_mapControl.Invalidate();
	}

	private void mapToolStripMenuItem_Click( object sender, EventArgs e )
	{

	}

	private void findBestValuesToolStripMenuItem_Click( object sender, EventArgs e )
	{
		// @@@ TODO :: Replace this with shared code from the 
		var startPos = new math.Vec2( _map.Layer.transX, _map.Layer.transY );

		var size = new math.Vec2( _map.Layer.scaleX, _map.Layer.scaleY );

		var step = new math.Vec2( 1.0f / 255.0f ) * size;

		var smallestV = float.MaxValue;
		var largestV  = float.MinValue;

		for( int y = 0; y < Map.Max.Y; y += 32 )
		{
			var yF = (float)y;

			var perlinY = startPos.Y + yF * step.Y;

			for( int x = 0; x < Map.Max.X; x+= 32 )
			{
				var xF = (float)x;

				var perlinX = startPos.X + xF * step.X;

				var perlin = new math.Vec2( perlinX, perlinY );

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
		//_grid.SelectedObjects = new[] { (object)_map.Layer, (object)_mapControl._viewInfo };


		Invalidate();

	}

	private void resetWorldOriginToolStripMenuItem_Click( object sender, EventArgs e )
	{
		_mapControl.ResetOrigin();
		Invalidate();
	}

	private void MapForm_Load( object sender, EventArgs e )
	{

	}

	private void toolStripSplitButton1_ButtonClick( object sender, EventArgs e )
	{

	}

	private void toolStripStatusLabel2_Click( object sender, EventArgs e )
	{

	}

	private void _status_ItemClicked( object sender, ToolStripItemClickedEventArgs e )
	{

	}
}
