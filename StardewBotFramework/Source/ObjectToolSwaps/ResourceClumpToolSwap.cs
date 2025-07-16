using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewBotFramework.Source.ObjectToolSwaps;

public class ResourceClumpToolSwap
{
    public static bool Swap(Point tile)
    {
        GameLocation location = BotBase.CurrentLocation;
        
        int CurrentItemCategory = BotBase.Farmer.CurrentItem.Category;
        string CurrentItemName = BotBase.Farmer.CurrentItem.Name;
        bool CurrentItemNull = BotBase.Farmer.CurrentItem == null;
        
        foreach (var clump in location.resourceClumps)
        {
            if (clump.occupiesTile(tile.X,tile.Y))
            {
                if (clump is GiantCrop)
                {
                    if (CurrentItemNull || CurrentItemName != "Tapper")
                    {
                        SwapItemHandler.SwapItem(typeof(Axe), "");
                    }

                    return true;
                }
                
                if (IsLog(clump))
                {
                    SwapItemHandler.SwapItem(typeof(Axe), "");
                    return true;
                }

                if (IsBoulder(clump))
                {
                    SwapItemHandler.SwapItem(typeof(Pickaxe), "");
                    return true;
                }

                if (IsGreenRainBush(clump))
                {
                    SwapItemHandler.SwapItem(typeof(MeleeWeapon), "Scythe");
                    return true;
                }
            }
        }

        return false;
    }

    private static bool IsLog(ResourceClump resourceClump)
    {
        return new List<int> { 600, 602 }.Contains(resourceClump.parentSheetIndex.Value);
    }

    private static bool IsBoulder(ResourceClump resourceClump)
    {
        return new List<int> { 148, 622, 672, 752, 754, 756, 758 }.Contains(resourceClump.parentSheetIndex.Value);
    }

    private static bool IsGreenRainBush(ResourceClump resourceClump)
    {
        return new List<int> { 44, 46 }.Contains(resourceClump.parentSheetIndex.Value);
    }
}