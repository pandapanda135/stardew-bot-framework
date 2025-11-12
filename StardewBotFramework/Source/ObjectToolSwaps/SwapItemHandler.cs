using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.ObjectToolSwaps;

// Thanks to https://www.nexusmods.com/stardewvalley/mods/21050?tab=description as the SwapItem method comes from them,
// you can see what they allow for their mod to be used for in the "Permissions and credits" tab. 

public static class SwapItemHandler
{
    /// <summary>
    /// This allows for swapping the currently selected item, this will also change the toolbar
    /// </summary>
    /// <param name="toolType">The <see cref="Type"/> of the tool.</param>
    /// <param name="meleeWeapon">This is for melee weapons, If you want to select the scythe pass "Scythe" else "Weapon"</param>
    public static bool SwapItem(Type toolType,string meleeWeapon)
    {
        switch (toolType)
        {
            case { } t when t == typeof(MeleeWeapon):
                switch (meleeWeapon)
                {
                    case "Scythe":
                        foreach (var item in BotBase.Farmer.Items)
                        {
                            if (item is not Tool tool || !tool.isScythe() || item.GetType() != toolType) continue;

                            ChangeIndex(item);
                            break;
                        }
                        
                        break;
                    case "Weapon":
                        foreach (var item in BotBase.Farmer.Items)
                        {
                            if (item is not Tool tool || tool.isScythe() || item.GetType() != toolType) continue;

                            ChangeIndex(item);
                            break;
                        }

                        break;
                }

                break;
            default: // non melee weapon items
                foreach (var item in BotBase.Farmer.Items)
                {
                    if (item is null || item.GetType() != toolType) continue;

                    ChangeIndex(item);
                    break;
                }

                break;
        }

        return BotBase.Farmer.CurrentTool.GetType() == toolType;
    }

    private static void ChangeIndex(Item obj)
    {
        int index = BotBase.Farmer.Items.IndexOf(obj);
        if (BotBase.Farmer.CurrentToolIndex == index) return;
            
        ChangeToolbar(index);
        BotBase.Farmer.CurrentToolIndex = index;
    }

    public static bool EquipFirstEmptySlot()
    {
        if (BotBase.Farmer.Items.Count < BotBase.Farmer.MaxItems)
        {
            BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.Count + 1;
            return true;
        }

        for (int i = 0; i < BotBase.Farmer.Items.Count; i++)
        {
            if (BotBase.Farmer.Items[i] is not null) continue;

            BotBase.Farmer.CurrentToolIndex = i;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Change the currently selected object to whatever you specify. 
    /// </summary>
    public static void SwapObject(Object obj)
    {
        foreach (var item in BotBase.Farmer.Items)
        {
            if (item is null || item != obj) continue;
            
            ChangeIndex(item);
            return;
        }
    }
    
    public static void SwapItem(Item item)
    {
        SwapObject((Object)item);
    }
    
    private static readonly List<string> FertilizerItemIds = new() { "368", "369", "370", "371" };
    /// <summary>
    /// Equip fertilizer
    /// </summary>
    /// <param name="dirt"><see cref="HoeDirt"/></param>
    /// <returns>True if fertilizer can be used, false if cannot</returns>
    public static bool EquipFertilizer(HoeDirt dirt)
    {
        foreach (var item in BotBase.Farmer.Items)
        {
            if (FertilizerItemIds.Contains(item.ItemId))
            {
                if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                {
                    ChangeToolbar(BotBase.Farmer.Items.IndexOf(item));
                    if (dirt.CheckApplyFertilizerRules(item.ItemId) != HoeDirtFertilizerApplyStatus.Okay)
                    {
                        return false;
                    }
                    BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                    return true;
                }
            }
        }

        return false;
    }

    public static bool EquipTapper()
    {
        foreach (var item in BotBase.Farmer.Items)
        {
            if (item is null) continue;
            if (!item.Name.Contains("Tapper")) continue;
            if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
            {
                ChangeToolbar(BotBase.Farmer.Items.IndexOf(item));
                BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                return true;
            }
        }

        return false;
    }
    
    private static void ChangeToolbar(int index)
    {
        for (int i = 0; i < (int)Math.Floor((double)index / 11); i++)
        {
            BotBase.Farmer.shiftToolbar(true);    
        }
    }
}