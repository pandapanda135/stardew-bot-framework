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
    /// <param name="includeTapper">Whether to allow changing to a tapper or not. If this is true and the tree is already tapped it will return false else if tapper exists.</param>
    /// <returns>This will return true if the item was swapped else false, this is unless you include fertilizer then this will be if the selected fertilizer can be applied.</returns>
    public static bool Swap(Point tile,bool includeFertilizer = false,bool includeTapper = false)
    {
        GameLocation location = BotBase.CurrentLocation;
        
        // bushes
        foreach (var feature in location.largeTerrainFeatures)
        {
            if (feature.getBoundingBox().Contains(tile))
            {
                Logger.Info($"Changing to large terrain feature");
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
                            if (includeTapper)
                            {
                                return !tree.tapped.Value && SwapItemHandler.EquipTapper();
                            }
                            SwapItemHandler.SwapItem(typeof(Axe),"");
                            return true;
                        case HoeDirt dirt:
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
                            SwapItemHandler.SwapItem(typeof(Pickaxe),""); // this is incase they want to destroy crops for fun
                            return true;
                        case Grass:
                            Logger.Info($"switch to grass");
                            SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Scythe");
                            return true;
                        case Bush:
                            Logger.Info($"switch to bush");
                            SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Scythe");
                            return true;
                    }
            }
        }
        
        return false;
    }

}