using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace rl;

public interface IMapView
{
	void DoUpdate( Map map );
}

public class MapBuilder
{

}

public record MapLayer( 
	Func<g3.Vector2f, float> Fn,
	float transX, float transY, float transZ, 
	float scaleX, float scaleY, float scaleZ,
	float octaves = 6
)
{
	public static MapLayer Unit   = new ( (p)=>0.5f, 0, 0, 0,    1.0f, 1.0f, 1.0f );
	public static MapLayer Perlin = new ( (p)=>0.5f, 0, 0, 0.5f, 1.0f, 1.0f, 1.0f );
}



public record Map( MapLayer Layer )
{
}

/*
public record MapHolder( Map Map, ImmutableList<IMapView> Views )
{
	void Update()
	{
		foreach( var view in Views )
		{
			view.DoUpdate( Map );
		}
	}
}
*/