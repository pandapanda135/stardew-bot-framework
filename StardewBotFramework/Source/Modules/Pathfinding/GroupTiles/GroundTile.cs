using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class GroundTile : ITile
{
	/// <summary>
	/// The <see cref="HoeDirt"/> this plant is associated with.
	/// </summary>
	public TerrainFeature? TerrainFeature;

	public ResourceClump? ResourceClump;

	public bool WaterTile;

	public int Cost => Heuristic(BotBase.Farmer.TilePoint);

	public GroundTile(Point pos,GameLocation location,TerrainFeature? terrainFeature = null, ResourceClump? clump = null)
	{
		X = pos.X;
		Y = pos.Y;
        
		TerrainFeature = terrainFeature;
		ResourceClump = clump;
		WaterTile = location.waterTiles.waterTiles[X,Y].isWater;
	}
}