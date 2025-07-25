using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using xTile.Dimensions;

namespace StardewBotFramework.Source.ObjectToolSwaps;

public class TerrainFeatureToolSwap
{
    
    /// <summary>
    /// This will change the currently equipped item depending on what <see cref="TerrainFeature"/> is on the specified tile.
    /// </summary>
    /// <param name="tile">The tile to check.</param>
    /// <param name="includeFertilizer">Whether to allow changing to fertilizer or not.</param>
    /// <param name="includeTapper">Whether to allow changing to a tapper or not</param>
    /// <returns>This will return true if the item was swapped else false, this is unless you include fertilizer then this will be if the selected fertilizer can be applied.</returns>
    public static bool Swap(Point tile,bool includeFertilizer = false,bool includeTapper = false) // TODO: maybe change this to return the type of item it was swapped to and if not swapped null?
    {
        GameLocation location = BotBase.CurrentLocation;
        
        int currentItemCategory = BotBase.Farmer.CurrentItem.Category;
        bool currentItemNull = BotBase.Farmer.CurrentItem == null;

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
                if (feature.Value.Tile != tile.ToVector2()) continue;
                Logger.Info($"feature: {feature.Value.GetType()}");
                Logger.Info($"location: {feature.Value.Tile}");
                
                switch (feature.Value)
                    {
                        case Tree tree:
                            Logger.Info($"switch to axe");
                            if (!tree.tapped.Value && includeTapper) return SwapItemHandler.EquipTapper();
                            SwapItemHandler.SwapItem(typeof(Axe),"");
                            return true;
                        case HoeDirt dirt: // should probably include fertilizer
                            Logger.Info($"switch to dirt");
                            if (!dirt.isWatered())
                            {
                                SwapItemHandler.SwapItem(typeof(WateringCan),"");
                                return true;
                            }
                            if (includeFertilizer && !dirt.HasFertilizer())
                            {
                                return SwapItemHandler.EquipFertilizer(dirt);
                            }
                            SwapItemHandler.SwapItem(typeof(Pickaxe),""); // this is for destroying crops
                            return true;
                        case Grass:
                            Logger.Info($"switch to grass");
                            // SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Scythe");
                            return true;
                    }
            }
        }
        
        return false;
    }

}