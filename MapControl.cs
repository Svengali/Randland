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
	g3.Vector2f PerlinPos, 
	g3.Vector2i Pos )
{
	//public Bitmap Overview = new Bitmap( 256, 256, PixelFormat.Format32bppRgb );
	public Bitmap Detail   = new Bitmap( 256, 256, PixelFormat.Format32bppRgb );

	public void GenerateDetail( MapControl mapControl2, MapViewInfo viewInfo )
	{
		var data = Detail.LockBits( new Rectangle( 0, 0, 256, 256 ), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb );

		var startPos = PerlinPos; // + new g3.Vector2f(viewInfo.transX, viewInfo.transY);

		var size = new g3.Vector2f( Layer.scaleX, Layer.scaleY );

		var step = new g3.Vector2f( 1.0f / 255.0f ) * size;

		try
		{
			unsafe
			{
				uint* pBase = (uint*)data.Scan0.ToPointer();

				for( int y = 0; y < 256; y += 1 )
				{
					var yF = (float)y;

					var perlinY = startPos.y + yF * step.y;

					for( int x = 0; x < 256; x += 1 )
					{
						var xF = (float)x;

						var perlinX = startPos.x + xF * step.x;

						var perlin = new g3.Vector2f( perlinX, perlinY );

						//var vPerlin = rl.Perlin.Fbm( perlin, ( p ) => 2, ( p ) => 0.5f, ( p ) => 4 );

						var vPerlin = Layer.Fn( perlin );

						var vScaled = vPerlin * Layer.scaleZ;

						var vScaledMoved = vScaled + Layer.transZ;

						var vFull = vScaledMoved;

						var v = Math.Max( 0.0f, Math.Min( vFull, 1.0f ) );

						//V [0..1]

						uint final = 0x00;

						if( v < 0.5f )
						{
							var water = v * 2.0f;
							var finalF = water * 255.0f;
							final = (uint)finalF;
						}
						else
						{
							var byteV = v * 255.0f;

							var byteAsByte = (uint)byteV;

							final = byteAsByte << 16 | byteAsByte << 8 | byteAsByte << 0;
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
	Bitmap _bmp = new Bitmap(2048, 1024);

	public Map _map;

	ConcurrentQueue<Chunk> _chunksNeeded = new();
	ConcurrentDictionary<g3.Vector2i, Chunk> _chunks = new();

	//Monitor _processChunks = new();
	volatile bool _redo = false;

	public MapViewInfo _viewInfo = new MapViewInfo( 0, 0, 0,  1, 1, 1 );

	public MapControl( Map map )
	{
		InitializeComponent();

		//SetStyle( ControlStyles.Opaque, true );

		_map = map;
	}

	public void DoUpdate( Func<g3.Vector2f, float> Fn )
	{
		_map = _map with { Layer = _map.Layer with { Fn = Fn } };

		_redo = true;

		var startPos = new g3.Vector2f( _map.Layer.transX, _map.Layer.transY );

		var size = new g3.Vector2f( _map.Layer.scaleX, _map.Layer.scaleY );

		var step = new g3.Vector2f( 1.0f / 255.0f ) * size;


		Monitor.Enter( this );
		_redo = false;

		while( !_chunksNeeded.IsEmpty ) _chunksNeeded.TryDequeue( out _ );
		_chunks.Clear();

		// @@@ TODO: Configify this
		for( int y = 0; y < 1024; y += 256 )
		{
			float fY = (float)y;

			var perlinY = startPos.y + fY * step.y;

			for( int x = 0; x < 2048; x += 256 )
			{
				var fX = (float)x;

				var perlinX = startPos.x + fX * step.x;

				var perlin = new g3.Vector2f( perlinX, perlinY );

				var chunk = new Chunk( _map.Layer, perlin, new g3.Vector2i( x, y ) );

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

								Invalidate( new Rectangle( new Point( chunk.Pos.x, chunk.Pos.y ) , new Size(256, 256) ) );

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

	protected override void OnPaint( PaintEventArgs e )
	{
		//e.Graphics.DrawImageUnscaled( _bmp, 0, 0 );

		for( int y = 0; y < 1024; y += 256 )
		{
			for( int x = 0; x < 2048; x += 256 )
			{
				var p = new g3.Vector2i( x, y );

				var gotChunk = _chunks.TryGetValue( p, out Chunk chunk );
				if( gotChunk )
				{
					e.Graphics.DrawImageUnscaled( chunk.Detail, x, y );
				}
			}
		}

		base.OnPaint( e );
	}

	protected override void OnPaintBackground( PaintEventArgs e )
	{
		//e.Graphics.DrawImageUnscaledAndClipped

		base.OnPaintBackground( e );
	}
}
