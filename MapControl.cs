using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using System.Drawing.Imaging;
using System.Collections.Concurrent;
using System.Threading;

namespace rl; 

public record MapViewInfo(
	float transX, float transY, float transZ,
	float scaleX, float scaleY, float scaleZ
	)
{

}

public record Chunk( 
	MapLayer Layer, 
	math.Vec2 PerlinPos, 
	math.Int2 Pos )
{
	//public Bitmap Overview = new Bitmap( 256, 256, PixelFormat.Format32bppRgb );
	public Bitmap Detail   = new Bitmap( 256, 256, PixelFormat.Format32bppRgb );

	public void GenerateDetail( MapControl mapControl2, MapViewInfo viewInfo )
	{
		var data = Detail.LockBits( new Rectangle( 0, 0, 256, 256 ), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb );

		var startPos = PerlinPos; // + new math.Vec2(viewInfo.transX, viewInfo.transY);

		var size = new math.Vec2( Layer.scaleX, Layer.scaleY );

		var step = new math.Vec2( 1.0f / 255.0f ) * size;

		try
		{
			unsafe
			{
				uint* pBase = (uint*)data.Scan0.ToPointer();

				for( int y = 0; y < 256; y += 1 )
				{
					var yF = (float)y;

					var perlinY = startPos.Y + yF * step.Y;

					for( int x = 0; x < 256; x += 1 )
					{
						var xF = (float)x;

						var perlinX = startPos.X + xF * step.X;

						var perlin = new math.Vec2( perlinX, perlinY );

						//var vPerlin = rl.Perlin.Fbm( perlin, ( p ) => 2, ( p ) => 0.5f, ( p ) => 4 );

						var vPerlin = Layer.Fn( perlin );

						var vScaled = vPerlin * Layer.scaleZ;

						var vScaledMoved = vScaled + Layer.transZ;

						var vFull = vScaledMoved;

						var v = Math.Max( 0.0f, Math.Min( vFull, 1.0f ) );

						uint isSmallerThan0 = ((uint)(x+y)&0x03) * (vFull < 0.0f ? (uint)1 : (uint)0);
						uint isLargerThan1  = ((uint)(x+y)&0x03) * (vFull > 1.0f ? (uint)1 : (uint)0);

						//V [0..1]

						uint final = 0x00;

						if( v < 0.5f )
						{
							var water = v * 2.0f;
							var finalF = water * 255.0f;

							uint red = isSmallerThan0 * 0x0f0000;

							final = (uint)finalF | red;
						}
						else
						{
							var byteV = v * 255.0f;

							var byteAsByte = (uint)byteV;

							uint red = isLargerThan1 * 0x0f;

							final = byteAsByte << 16 | (byteAsByte - red) << 8 | (byteAsByte - red) << 0 | red;
						}

						var addr = pBase + y * (data.Stride / 4) + x;

						*addr = final;
					}
				}
			}
		}
		finally
		{
			Detail.UnlockBits( data );
		}
	}

}

public partial class MapControl : UserControl, IMapView
{
	Bitmap _bmp = new Bitmap(Map.Max.X, Map.Max.Y);

	public Map _map;

	ConcurrentQueue<Chunk> _chunksNeeded = new();
	ConcurrentDictionary<math.Int2, Chunk> _chunks = new();

	//Monitor _processChunks = new();
	volatile bool _redo = false;

	public MapViewInfo _viewInfo = new MapViewInfo( 0, 0, 0,  1, 1, 1 );

	public MapControl()
	{
		InitializeComponent();

		Size = new Size( Map.Max.X, Map.Max.Y );

		AutoScroll = true;
	}

	public MapControl( Map map )
	{
		InitializeComponent();

		//SetStyle( ControlStyles.Opaque, true );

		Size = new Size( Map.Max.X, Map.Max.Y );
		AutoScroll = true;

		_map = map;
	}

	public void DoUpdate( Func<math.Vec2, float> Fn )
	{
		_map = _map with { Layer = _map.Layer with { Fn = Fn } };

		_redo = true;

		var startPos = new math.Vec2( _map.Layer.transX, _map.Layer.transY );

		var size = new math.Vec2( _map.Layer.scaleX, _map.Layer.scaleY );

		var step = new math.Vec2( 1.0f / 255.0f ) * size;


		Monitor.Enter( this );
		_redo = false;

		while( !_chunksNeeded.IsEmpty ) _chunksNeeded.TryDequeue( out _ );
		_chunks.Clear();

		// @@@ TODO: Configify this
		for( int y = 0; y < Map.Max.Y; y += 256 )
		{
			float fY = (float)y;

			var perlinY = startPos.Y + fY * step.Y;

			for( int x = 0; x < Map.Max.X; x += 256 )
			{
				var fX = (float)x;

				var perlinX = startPos.X + fX * step.X;

				var perlin = new math.Vec2( perlinX, perlinY );

				var chunk = new Chunk( _map.Layer, perlin, new math.Int2( x, y ) );

				_chunksNeeded.Enqueue( chunk );
			}
		}

		Monitor.Exit( this );

		//FillinBitmap( _map.Layer );
	}

	protected override void OnHandleCreated( EventArgs e )
	{
		base.OnHandleCreated( e );

		Task.Run( () => {
			while( !this.IsDisposed )
			{
				List<Task> runningTasks = new();

				// @@@ TODO :: Refactor this to wait for things, vs just run
				Monitor.Enter( this );

				while( !_chunksNeeded.IsEmpty && !_redo )
				{
					bool newChunk = _chunksNeeded.TryDequeue( out Chunk chunk );

					if( newChunk )
					{
						runningTasks.Add( Task.Run( () => {

							chunk.GenerateDetail( this, _viewInfo );

							var chunkAdded = _chunks.TryAdd( chunk.Pos, chunk );

							if( !chunkAdded )
							{
								log.warn( $"Error adding a chunk {chunk.Pos}" );
							}

							this.Invoke( () => {

								Invalidate( new Rectangle( new Point( chunk.Pos.X, chunk.Pos.Y ) , new Size(256, 256) ) );

							} );

						} ) );
					}
				}

				Monitor.Exit( this );

				Task.WaitAll( runningTasks.ToArray() );

				Task.Delay( 500 );
			}
		} );

		DoUpdate( _map.Layer.Fn );
	}

	protected override void OnHandleDestroyed( EventArgs e )
	{
		_redo = true;

		Monitor.Enter( this );
		_redo = false;

		while( !_chunksNeeded.IsEmpty ) _chunksNeeded.TryDequeue( out _ );
		_chunks.Clear();

		Monitor.Exit( this );

		base.OnHandleDestroyed( e );
	}

	private Point _worldOrigin = Point.Empty;

	protected override void OnPaint( PaintEventArgs e )
	{
		//e.Graphics.DrawImageUnscaled( _bmp, 0, 0 );

		var chunkStartX = ((-_worldOrigin.X + e.ClipRectangle.Location.X) >> 8);
		var chunkStartY = ((-_worldOrigin.Y + e.ClipRectangle.Location.Y) >> 8);

		var chunkEndX = chunkStartX + ((e.ClipRectangle.Width + 255) >> 8);
		var chunkEndY = chunkStartY + ((e.ClipRectangle.Height + 255) >> 8);

		log.debug( $"Chunk ({chunkStartX}, {chunkStartY})x({chunkEndX}, {chunkEndY}) _worldOrigin({_worldOrigin.X}, {_worldOrigin.Y}) Rect({e.ClipRectangle.ToString()})" );

		for( int y = chunkStartY * 256; y <= chunkEndY * 256; y += 256 )
		{
			for( int x = chunkStartX * 256; x <= chunkEndX * 256; x += 256 )
			{
				var p = new math.Int2( x, y );

				log.debug( $"Chunk ({p})" );

				var gotChunk = _chunks.TryGetValue( p, out Chunk chunk );
				if( gotChunk )
				{
					var origX = x + _worldOrigin.X;
					var origY = y + _worldOrigin.Y;

					log.debug( $"Draw ({origX}, {origY})" );

					e.Graphics.DrawImageUnscaled( chunk.Detail, origX, origY );
				}
			}
		}

		base.OnPaint( e );
	}

	protected override void OnPaintBackground( PaintEventArgs e )
	{
		//e.Graphics.DrawImageUnscaledAndClipped

		//base.OnPaintBackground( e );
	}

	public Action<string> FnChangeStatus = (str) => { };

	public string _status = "Left Click to see height information";

	private void MapControl_MouseClick( object sender, MouseEventArgs e )
	{
		if( e.Button != MouseButtons.Left ) return;


		//var chunk = new Chunk( _map.Layer, perlin, new math.Int2( x, y ) );
	}

	private void MapControl_Load( object sender, EventArgs e )
	{

	}

	bool _draggingWorld = false;
	Point _mouseStart;


	private void MapControl_MouseDown( object sender, MouseEventArgs e )
	{
		if( e.Button == MouseButtons.Middle ) 
		{
			_draggingWorld = true;
			_mouseStart = e.Location;
		}
	}

	private void MapControl_MouseMove( object sender, MouseEventArgs e )
	{
		if( _draggingWorld )
		{
			var mouseDeltaX = e.Location.X - _mouseStart.X;
			var mouseDeltaY = e.Location.Y - _mouseStart.Y;

			_worldOrigin = new Point( _worldOrigin.X + mouseDeltaX, _worldOrigin.Y + mouseDeltaY );

			_mouseStart = e.Location;

			Invalidate();
		}

		var worldX = -_worldOrigin.X + e.Location.X;
		var worldY = -_worldOrigin.Y + e.Location.Y;

		var startPos = new math.Vec2( _map.Layer.transX, _map.Layer.transY );

		var size = new math.Vec2( _map.Layer.scaleX, _map.Layer.scaleY );

		var step = new math.Vec2( 1.0f / 255.0f ) * size;


		float fY = (float)worldY;

		var perlinY = startPos.Y + fY * step.Y;

		var fX = (float)worldX;

		var perlinX = startPos.X + fX * step.X;

		var perlin = new math.Vec2( perlinX, perlinY );

		var height = _map.Layer.Fn( perlin );

		_status = $"World Pixel({worldX}, {worldY}) Perlin({perlin.X:F3}, {perlin.Y:F3}), Height({height:F3})";

		FnChangeStatus( _status );

	}

	private void MapControl_MouseUp( object sender, MouseEventArgs e )
	{
		if( e.Button == MouseButtons.Middle ) _draggingWorld = false;
	}

	private void MapControl_KeyDown( object sender, KeyEventArgs e )
	{

	}

	private void MapControl_KeyUp( object sender, KeyEventArgs e )
	{

	}

	internal void ResetOrigin()
	{
		_worldOrigin = new Point( 0, 0 );
	}
}
