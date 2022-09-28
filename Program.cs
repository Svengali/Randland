using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace rl;

public record MapExp( string Name, Func<g3.Vector2f, float> Fn, ImmutableList<IMapView> Views )
{

}

public record WorldFns( string Category, ImmutableList<MapExp> MapExp )
{
	public WorldFns AddOrUpdate( string name, Func<g3.Vector2f, float> fn /*, ImmutableList<Map> instances*/ )
	{
		var oldMap = MapExp.Find( (old) => old.Name == name );

		if( oldMap != null )
		{
			log.debug( $"-> Replacing {name}" );

			var newList = MapExp.Replace( oldMap, new MapExp( name, fn, oldMap.Views ) );

			return this with { MapExp = newList };
		}
		else
		{
			log.debug( $"-> Adding {name}" );

			var newList = MapExp.Add( new MapExp( name, fn, ImmutableList<IMapView>.Empty ) );

			return this with { MapExp = newList };
		}
	}


}



internal static class Randland
{

	static public ConcurrentDictionary<string, WorldFns> s_functions = new();

	static public void AddMapForm( MapForm mapForm, TreeNode leaf )
	{
		var parent = leaf.Parent;

		var found = s_functions.TryGetValue( parent.Name, out var worldFns );

		if( !found )
		{
			log.warn( $"Error looking up {parent.Name}" );
			return;
		}

		var mapExp = worldFns.MapExp;

		foreach( var map in mapExp )
		{
			if( map.Name == leaf.Name )
			{
				log.debug( $"Adding form to {parent.Name}" );

				var newMap = map with { Views = map.Views.Add( mapForm ) };

				var newWorld = worldFns with { MapExp = worldFns.MapExp.Add( newMap ) };

				s_functions.AddOrUpdate( parent.Name, newWorld, (k, v) => newWorld );

				return;
			}
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

		} );


		Application.EnableVisualStyles( );
		Application.SetCompatibleTextRenderingDefault( false );
		Application.Run( new RandlandForm( ) );
	}
}
