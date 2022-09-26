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

namespace rl; 

public partial class MapControl : UserControl
{
	Bitmap _bmp = new Bitmap(2048, 1024);

	public MapControl()
	{
		InitializeComponent();

		//SetStyle( ControlStyles.Opaque, true );
	}

	public void FillinBitmap( MapLayer layer )
	{
		_bmp = new Bitmap( 2048, 1024, PixelFormat.Format32bppRgb );


		var data = _bmp.LockBits( new Rectangle( 0, 0, 2048, 1024 ), ImageLockMode.WriteOnly, PixelFormat.Format32bppRgb );

		var startPos = new g3.Vector2f( layer.transX, layer.transY );

		var size = new g3.Vector2f( layer.scaleX, layer.scaleY );

		var step = new g3.Vector2f( 1.0f / 255.0f ) * size;

		try
		{
			unsafe
			{
				uint* pBase = (uint*)data.Scan0.ToPointer();

				for( int y = 0; y < 1024; y+= 4 )
				{
					var yF = (float)y;

					var perlinY = startPos.y + yF * step.y;

					for( int x = 0; x < 2048; x+= 4 )
					{
						var xF = (float)x;

						var perlinX = startPos.x + xF * step.x;

						var perlin = new g3.Vector2f( perlinX, perlinY );

						var vPerlin = rl.Perlin.Fbm( perlin, ( p ) => 2, ( p ) => 0.5f, ( p ) => 4 );

						var vScaled = vPerlin * layer.scaleZ;

						var vScaledMoved = vScaled + layer.transZ;

						var vFull = vScaledMoved;

						var v = Math.Max( 0.0f, Math.Min( vFull, 1.0f ) );


						var byteV = v * 255.0f;

						var byteAsByte = (uint)byteV;

						var addr = pBase + y * (data.Stride / 4) + x;

						*addr = byteAsByte << 16 | byteAsByte << 8 | byteAsByte << 0;
					}
				}
			}
		}
		finally
		{
			_bmp.UnlockBits( data );
		}
	}

	protected override void OnPaint( PaintEventArgs e )
	{
		e.Graphics.DrawImageUnscaled( _bmp, 0, 0 );

		base.OnPaint( e );
	}

	protected override void OnPaintBackground( PaintEventArgs e )
	{
		//e.Graphics.DrawImageUnscaledAndClipped

		base.OnPaintBackground( e );
	}
}
