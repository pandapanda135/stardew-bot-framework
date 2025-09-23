using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class PlaceTile : GroundTile
{
	public PlaceTile(Point pos, GameLocation location, TerrainFeature? terrainFeature = null, ResourceClump? clump = null, Object? obj = null, Object? itemToPlace = null) : base(pos, location, terrainFeature, clump, obj)
	{
		ItemToPlace = itemToPlace;
	}

	/// <summary>
	/// Item to be placed at this position
	/// </summary>
	public Object? ItemToPlace;
}