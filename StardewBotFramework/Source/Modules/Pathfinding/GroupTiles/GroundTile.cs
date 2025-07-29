using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class GroundTile : ITile
{
	/// <summary>
	/// The <see cref="TerrainFeature"/> that is on this tile if there is one, else will be null.
	/// </summary>
	public readonly TerrainFeature? TerrainFeature;

	/// <summary>
	/// The <see cref="ResourceClump"/> that is on this tile if there is one, else will be null.
	/// </summary>
	public readonly ResourceClump? ResourceClump;
	
	public readonly Object? Obj;

	/// <summary>
	/// If this is a water tile
	/// </summary>
	public readonly bool WaterTile;

	public int Cost => Heuristic(BotBase.Farmer.TilePoint);

	public GroundTile(Point pos,GameLocation location,TerrainFeature? terrainFeature = null, ResourceClump? clump = null,Object? obj = null)
	{
		X = pos.X;
		Y = pos.Y;
        
		TerrainFeature = terrainFeature;
		ResourceClump = clump;
		Obj = obj;
		int upperBound = location.waterTiles.waterTiles.GetUpperBound(0);
		int upperBoundDimension = location.waterTiles.waterTiles.GetUpperBound(1);
		if (upperBound > X && upperBoundDimension > Y)
		{
			WaterTile = location.waterTiles.waterTiles[X,Y].isWater;
		}
		else
		{
			WaterTile = false;
		}
	}
}