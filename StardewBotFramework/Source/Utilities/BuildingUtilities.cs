using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Extensions;
using StardewValley.GameData.Buildings;

namespace StardewBotFramework.Source.Utilities;

public static class BuildingUtilities
{
	public static bool CanBuildHere(Building building, Vector2 tileLocation)
	{
		IEnumerable<Farmer> farmers = Game1.getAllFarmers().ToList();
		for (int y = 0; y < building.tilesHigh.Value; y++)
		{
			for (int x3 = 0; x3 < building.tilesWide.Value; x3++)
			{
				BotBase.CurrentLocation.pokeTileForConstruction(new Vector2(tileLocation.X + x3, tileLocation.Y + y));
			}
		}

		foreach (BuildingPlacementTile additionalPlacementTile in building.GetAdditionalPlacementTiles())
		{
			foreach (Point areaTile2 in additionalPlacementTile.TileArea.GetPoints())
			{
				BotBase.CurrentLocation.pokeTileForConstruction(new Vector2(tileLocation.X + areaTile2.X,
					tileLocation.Y + areaTile2.Y));
			}
		}

		for (int y5 = 0; y5 < building.tilesHigh.Value; y5++)
		{
			for (int x4 = 0; x4 < building.tilesWide.Value; x4++)
			{
				Vector2 currentGlobalTilePosition2 =
					new Vector2(tileLocation.X + x4, tileLocation.Y + y5);
				if (BotBase.CurrentLocation.buildings.Contains(building) && building.occupiesTile(currentGlobalTilePosition2))
				{
					continue;
				}

				if (!BotBase.CurrentLocation.isBuildable(currentGlobalTilePosition2))
				{
					return false;
				}

				if (farmers.Any(farmer =>
					    farmer.GetBoundingBox().Intersects(new Rectangle(x4 * 64, y5 * 64, 64, 64))))
				{
					return false;
				}
			}
		}

		foreach (BuildingPlacementTile additionalPlacementTile2 in building.GetAdditionalPlacementTiles())
		{
			bool onlyNeedsToBePassable = additionalPlacementTile2.OnlyNeedsToBePassable;
			foreach (Point point in additionalPlacementTile2.TileArea.GetPoints())
			{
				int x5 = point.X;
				int y4 = point.Y;
				Vector2 currentGlobalTilePosition3 =
					new Vector2(tileLocation.X + x5, tileLocation.Y + y4);
				if (BotBase.CurrentLocation.buildings.Contains(building) && building.occupiesTile(currentGlobalTilePosition3))
				{
					continue;
				}

				if (!BotBase.CurrentLocation.isBuildable(currentGlobalTilePosition3, onlyNeedsToBePassable))
				{
					return false;
				}

				if (onlyNeedsToBePassable)
				{
					continue;
				}

				foreach (Farmer farmer2 in farmers)
				{
					if (farmer2.GetBoundingBox()
					    .Intersects(new Rectangle(x5 * 64, y4 * 64, 64, 64)))
					{
						return false;
					}
				}
			}
		}

		if (building.humanDoor.Value != new Point(-1, -1))
		{
			Vector2 doorPos = tileLocation + new Vector2(building.humanDoor.X, building.humanDoor.Y + 1);
			if ((!BotBase.CurrentLocation.buildings.Contains(building) || !building.occupiesTile(doorPos)) && !BotBase.CurrentLocation.isBuildable(doorPos) &&
			    !BotBase.CurrentLocation.isPath(doorPos))
			{
				return false;
			}
		}

		string finalCheckResult = building.isThereAnythingtoPreventConstruction(BotBase.CurrentLocation, tileLocation);
		if (finalCheckResult != null)
		{
			Game1.addHUDMessage(new HUDMessage(finalCheckResult, 3));
			return false;
		}

		return true;
	}
}