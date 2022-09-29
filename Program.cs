using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rl;

//*
public record MapExp( string Category, string Name, Func<g3.Vector2f, float> Fn, ImmutableList<IMapView> Views )
{

	public MapExp AddView( IMapView view )
	{
		return this with { Views = Views.Add(view) };
	}

}
//*/

/*
public record MapExp( string Name, Map Map, ImmutableList<IMapView> Views )
{

}
*/


public record WorldFns( string Category, ImmutableList<MapExp> MapExp )
{
	public WorldFns AddOrUpdate( string name, Func<g3.Vector2f, float> fn /*, ImmutableList<Map> instances*/ )
	{
		var oldMap = MapExp.Find( (old) => old.Name == name );

		if( oldMap != null )
		{
			log.debug( $"-> Replacing {name}" );

			var newList = MapExp.Replace( oldMap, new MapExp( Category, name, fn, oldMap.Views ) );

			foreach( var view in oldMap.Views )
			{
				view.DoUpdate( fn );
			}

			return this with { MapExp = newList };
		}
		else
		{
			log.debug( $"-> Adding {name}" );

			var newList = MapExp.Add( new MapExp( Category, name, fn, ImmutableList<IMapView>.Empty ) );

			return this with { MapExp = newList };
		}
	}


	public WorldFns AddView( string name, IMapView view )
	{
		var mapExp = MapExp.Find( (m) => m.Name == name );

		if( mapExp == null )
		{
			log.warn( $"Could not find {name} in {Category}" );
			return this;
		}

		return this with { MapExp = MapExp.Replace( mapExp, mapExp with { Views = mapExp.Views.Add( view ) } ) };
	}



}



internal static class Randland
{

	static public ConcurrentDictionary<string, WorldFns> s_functions = new();

	static public void AddMapForm( IMapView mapView, TreeNode leaf )
	{
		var cat = leaf.Parent.Name;
		var name= leaf.Name;


		try
		{
			Monitor.Enter( Randland.s_functions );
			var catFound = s_functions.TryRemove(cat, out var fns);
			if( catFound )
			{
				var newFns = fns.AddView( name, mapView );

				s_functions.TryAdd(cat, newFns);
			}
			else
			{
				log.warn( $"Category {cat} not found." );
			}
		}
		finally
		{
			Monitor.Exit( Randland.s_functions );
		}
	}

	/// <summary>
	/// The main entry point for the application.
	/// </summary>
	[STAThread]
	static void Main()
	{
		log.create( $"logs/randland.log", log.Endpoints.Console | log.Endpoints.File );

		log.high( $"Starting up in {Directory.GetCurrentDirectory()}" );

		//Load the geometry dll
		var pos = new g3.Vector3f();
		var v = rl.Perlin.Noise( 0.0f );


		Application.EnableVisualStyles();
		Application.SetCompatibleTextRenderingDefault( false );

		var randlandForm = new RandlandForm();

		scr.WatchPluginDir( "./scripts/", (ass) => {

			log.debug( $"{ass.FullName} modified." );

			foreach( var t in ass.ExportedTypes)
			{
				ImmutableList<MapExp>.Builder methods = ImmutableList<MapExp>.Empty.ToBuilder();

				WorldFns worldFns;

				var found = s_functions.TryGetValue( t.Name, out worldFns );

				if( !found )
				{
					log.debug( $"Adding new world fns {t.Name}" );
					worldFns = new WorldFns( t.Name, ImmutableList<MapExp>.Empty );
				}
				else
				{
					log.debug( $"Found existing world fns {t.Name}" );
				}

				foreach( var fn in t.GetMethods() )
				{
					if( !fn.IsStatic ) continue;

					if( fn.GetParameters().Length != 1 ) continue;

					if( fn.GetParameters()[0].ParameterType != typeof( g3.Vector2f ) ) continue;

					if( fn.ReturnType != typeof( float ) ) continue;

					var newFn = fn.CreateDelegate( typeof( Func<g3.Vector2f, float> ) );

					Func<g3.Vector2f, float> fnBare = (Func<g3.Vector2f, float>)newFn;

					worldFns = worldFns.AddOrUpdate( fn.Name, fnBare );

				}

				var name = t.Name;

				s_functions.AddOrUpdate( name, worldFns, (key, old) => worldFns );
			}

			if( randlandForm.Visible )
			{
				randlandForm.Invoke( randlandForm.CreateNewNodes );
			}

		} );


		Application.Run( randlandForm );
	}
}
