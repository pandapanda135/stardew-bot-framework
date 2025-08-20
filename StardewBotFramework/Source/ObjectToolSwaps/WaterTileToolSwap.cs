using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Locations;
using StardewValley.Tools;

namespace StardewBotFramework.Source.ObjectToolSwaps;

public class WaterTileToolSwap
{
    private static GameLocation location = BotBase.CurrentLocation;

    public static bool Swap(Point tile)
    {
        int CurrentItemCategory = BotBase.Farmer.CurrentItem.Category;
        bool CurrentItemNull = BotBase.Farmer.CurrentItem == null;

        if (!location.isWaterTile(tile.X, tile.Y))
        {
            return false;
        }

        if (IsWaterTrough(tile))
        {
            SwapItemHandler.SwapItem(typeof(WateringCan), "");
            return true;
        }
        if (IsPetBowlOrStable(tile))
        {
            SwapItemHandler.SwapItem(typeof(WateringCan), "");
            return true;
        }
        if (IsPanSpot(tile))
        {
            SwapItemHandler.SwapItem(typeof(Pan), "");
            return true;
        }
        if ((IsWaterSource(tile) || IsWater(tile)) && ShouldUseWateringCan)
        {
            if (BotBase.Farmer.CurrentItem is not FishingRod)
                SwapItemHandler.SwapItem(typeof(WateringCan), "");
            return true;
        }
        if (IsWater(tile))
        {
            if (BotBase.Farmer.CurrentItem is not WateringCan)
                SwapItemHandler.SwapItem(typeof(FishingRod), "");
            return true;
        }
        return false;
    }

    private static bool IsWaterTrough(Point tile)
    {
        foreach (var dict in BotBase.CurrentLocation.Objects)
        {
            foreach (var kvp in dict)
            {
                if (kvp.Value.Name == "Trough" && kvp.Key.ToPoint() == tile)
                {
                    return true;
                }
            }
        }

        return true;
    }

    private static readonly bool ShouldUseWateringCan = location is Farm || location is VolcanoDungeon ||
                                        location.InIslandContext() || location.isGreenhouse.Value;
    private static bool IsPetBowlOrStable(Point tile)
    {
        var building = location.getBuildingAt(tile.ToVector2());
        return building != null && (building.GetType() == typeof(PetBowl) ||
                                    building.GetType() == typeof(Stable));
    }
    private static bool IsPanSpot(Point tile)
    {
        var orePanRect = new Rectangle(location.orePanPoint.X * 64 - 64, location.orePanPoint.Y * 64 - 64, 256, 256);
        return orePanRect.Contains(tile.X * 64, tile.Y * 64) &&
               Utility.distance(BotBase.Farmer.StandingPixel.X, orePanRect.Center.X, BotBase.Farmer.StandingPixel.Y, orePanRect.Center.Y) <= 192f;
    }
    private static bool IsWater(Point tile)
    {
        return location.doesTileHaveProperty(tile.X, tile.Y, "Water", "Back") != null &&
               !(BotBase.Farmer.CurrentTool is WateringCan || BotBase.Farmer.CurrentTool is Pan);
    }
    private static bool IsWaterSource(Point tile)
    {
        return location.doesTileHaveProperty(tile.X, tile.Y, "WaterSource", "Back") != null;
    }
}