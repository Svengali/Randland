using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rl
{
	public static class Perlin
	{
		#region Noise functions

		public static float Noise( float x )
		{
			var X = (int)Math.Floor(x) & 0xff;
			x -= (float)Math.Floor( x );
			var u = Fade(x);
			return Lerp( u, Grad( perm[X], x ), Grad( perm[X + 1], x - 1 ) ) * 2;
		}

		public static float Noise( math.Vec2 pos )
		{
			var X = (int)Math.Floor(pos.X) & 0xff;
			var Y = (int)Math.Floor(pos.Y) & 0xff;
			pos.X -= (float)Math.Floor( pos.X );
			pos.Y -= (float)Math.Floor( pos.Y );
			var u = Fade(pos.X);
			var v = Fade(pos.Y);
			var A = (perm[X  ] + Y) & 0xff;
			var B = (perm[X+1] + Y) & 0xff;
			return Lerp( v, Lerp( u, Grad( perm[A], pos.X, pos.Y ), Grad( perm[B], pos.X - 1, pos.Y ) ),
										 Lerp( u, Grad( perm[A + 1], pos.X, pos.Y - 1 ), Grad( perm[B + 1], pos.X - 1, pos.Y - 1 ) ) );
		}

		public static float Noise( math.Vec3 pos )
		{
			var X = (int)Math.Floor(pos.X) & 0xff;
			var Y = (int)Math.Floor(pos.Y) & 0xff;
			var Z = (int)Math.Floor(pos.Z) & 0xff;
			pos.X -= (float)Math.Floor( pos.X );
			pos.Y -= (float)Math.Floor( pos.Y );
			pos.Z -= (float)Math.Floor( pos.Z );
			var u = Fade(pos.X);
			var v = Fade(pos.Y);
			var w = Fade(pos.Z);
			var A  = (perm[X+0] + Y) & 0xff;
			var B  = (perm[X+1] + Y) & 0xff;
			var AA = (perm[A+0] + Z) & 0xff;
			var BA = (perm[B+0] + Z) & 0xff;
			var AB = (perm[A+1] + Z) & 0xff;
			var BB = (perm[B+1] + Z) & 0xff;
			return Lerp( w, Lerp( v, Lerp( u, Grad( perm[AA], pos.X, pos.Y, pos.Z ), Grad( perm[BA], pos.X - 1, pos.Y, pos.Z ) ),
														 Lerp( u, Grad( perm[AB], pos.X, pos.Y - 1, pos.Z ), Grad( perm[BB], pos.X - 1, pos.Y - 1, pos.Z ) ) ),
										 Lerp( v, Lerp( u, Grad( perm[AA + 1], pos.X, pos.Y, pos.Z - 1 ), Grad( perm[BA + 1], pos.X - 1, pos.Y, pos.Z - 1 ) ),
														 Lerp( u, Grad( perm[AB + 1], pos.X, pos.Y - 1, pos.Z - 1 ), Grad( perm[BB + 1], pos.X - 1, pos.Y - 1, pos.Z - 1 ) ) ) );
		}

		#endregion

		#region fBm functions

		public static float Fbm( float x, Func<float, float> fnMult, Func<float, float> fnWeight, Func<float, float> fnOctave )
		{
			var f = 0.0f;
			var w = 0.5f;

			float allOctaves = fnOctave( x );

			float fullOctaves = (float)Math.Floor( allOctaves );

			float mult = fnMult( x );
			float weight = fnWeight( x );

			for( var i = 0; i < fullOctaves; i++ )
			{
				f += w * Noise( x );
				x *= mult;
				w *= weight;
			}

			float finalOctave = allOctaves - fullOctaves;

			if( finalOctave > 0 )
			{
				f += w * Noise( x ) * finalOctave;
			}

			return f;
		}

		public static float Fbm( math.Vec3 pos, Func<math.Vec3, float> fnMult, Func<math.Vec3, float> fnWeight, Func<math.Vec3, float> fnOctave )
		{
			var f = 0.0f;
			var w = 0.5f;

			float allOctaves = fnOctave( pos );

			float fullOctaves = (float)Math.Floor( allOctaves );

			float mult = fnMult( pos );
			float weight = fnWeight( pos );

			for( var i = 0; i < fullOctaves; i++ )
			{
				f += w * Noise( pos );
				pos *= mult;
				w *= weight;
			}

			float finalOctave = allOctaves - fullOctaves;

			if( finalOctave > 0 )
			{
				f += w * Noise( pos ) * finalOctave;
			}

			return f;
		}


		public static float Fbm( math.Vec2 pos, Func<math.Vec2, float> fnMult, Func<math.Vec2, float> fnWeight, Func<math.Vec2, float> fnOctave )
		{
			var f = 0.0f;
			var w = 0.5f;

			float allOctaves = fnOctave( pos );

			float fullOctaves = (float)Math.Floor( allOctaves );

			float mult = fnMult( pos );
			float weight = fnWeight( pos );

			for( var i = 0; i < fullOctaves; i++ )
			{
				f += w * Noise( pos );
				pos *= mult;
				w *= weight;
			}

			float finalOctave = allOctaves - fullOctaves;

			if( finalOctave > 0 )
			{
				f += w * Noise( pos ) * finalOctave;
			}

			return f;
		}


		#endregion

		#region Private functions

		static float Fade( float t )
		{
			return t * t * t * (t * (t * 6 - 15) + 10);
		}

		static float Lerp( float t, float a, float b )
		{
			return a + t * (b - a);
		}

		static float Grad( int hash, float x )
		{
			return (hash & 1) == 0 ? x : -x;
		}

		static float Grad( int hash, float x, float y )
		{
			return ((hash & 1) == 0 ? x : -x) + ((hash & 2) == 0 ? y : -y);
		}

		static float Grad( int hash, float x, float y, float z )
		{
			var h = hash & 15;
			var u = h < 8 ? x : y;
			var v = h < 4 ? y : (h == 12 || h == 14 ? x : z);
			return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
		}

		static int[] perm = {
				151,160,137,91,90,15,
				131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,8,99,37,240,21,10,23,
				190, 6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,35,11,32,57,177,33,
				88,237,149,56,87,174,20,125,136,171,168, 68,175,74,165,71,134,139,48,27,166,
				77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,55,46,245,40,244,
				102,143,54, 65,25,63,161, 1,216,80,73,209,76,132,187,208, 89,18,169,200,196,
				135,130,116,188,159,86,164,100,109,198,173,186, 3,64,52,217,226,250,124,123,
				5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,189,28,42,
				223,183,170,213,119,248,152, 2,44,154,163, 70,221,153,101,155,167, 43,172,9,
				129,22,39,253, 19,98,108,110,79,113,224,232,178,185, 112,104,218,246,97,228,
				251,34,242,193,238,210,144,12,191,179,162,241, 81,51,145,235,249,14,239,107,
				49,192,214, 31,181,199,106,157,184, 84,204,176,115,121,50,45,127, 4,150,254,
				138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
				151
		};

		#endregion
	}
}
