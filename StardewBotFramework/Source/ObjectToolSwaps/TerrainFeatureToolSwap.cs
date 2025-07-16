using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace StardewBotFramework.Source.ObjectToolSwaps;

public class TerrainFeatureToolSwap
{
    public static bool Swap(Point tile)
    {
        GameLocation location = BotBase.CurrentLocation;
        
        int CurrentItemCategory = BotBase.Farmer.CurrentItem.Category;
        bool CurrentItemNull = BotBase.Farmer.CurrentItem == null;

        // bushes
        foreach (var feature in location.largeTerrainFeatures)
        {
            if (feature.getBoundingBox().Contains(tile))
            {
                SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Scythe");
                return true;
            }
        }

        foreach (var terrainFeatureDict in location.terrainFeatures)
        {
            foreach (var feature in terrainFeatureDict)
            {
                if (!feature.Value.getBoundingBox().Contains(tile))
                {
                    return false;
                }

                switch (feature.Value)
                    {
                        case Tree:
                            SwapItemHandler.SwapItem(typeof(Axe),"");
                            return true;
                        case HoeDirt: // should probably include fertilizer
                            HoeDirt dirt = (feature.Value as HoeDirt)!;
                            if (!dirt.isWatered())
                            {
                                SwapItemHandler.SwapItem(typeof(WateringCan),"");
                                return true;
                            }
                            SwapItemHandler.SwapItem(typeof(Pickaxe),""); // this is for destroying crops
                            return true;
                        case Grass:
                            SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Scythe");
                            return true;
                    }
            }
        }
        
        return false;
    }

}