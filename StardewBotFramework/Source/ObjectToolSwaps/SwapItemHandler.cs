using System.Runtime.CompilerServices;
using Netcode;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.ObjectToolSwaps;

public class SwapItemHandler
{
    /// <summary>
    /// This allows for swapping the currently selected item, this will also change the toolbar
    /// </summary>
    /// <param name="toolType">The <see cref="Type"/> of the tool.</param>
    /// <param name="meleeWeapon">This is for melee weapons, If you want to select the scythe pass "Scythe" else "Weapon"</param>
    public static void SwapItem(Type toolType,string meleeWeapon)
    {
        switch (toolType)
        {
            case { } t when t == typeof(MeleeWeapon):
                switch (meleeWeapon)
                {
                    case "Scythe":
                        foreach (var item in BotBase.Farmer.Items)
                        {
                            if (item is null) continue;
                            if (item.Name == "Scythe" && item.GetType() == toolType)
                            {
                                if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                                {
                                    ChangeToolbar(BotBase.Farmer.Items.IndexOf(item));
                                    BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                                }

                                return;
                            }
                        }

                        return;
                    case "Weapon":
                        foreach (var item in BotBase.Farmer.Items)
                        {
                            if (item is null) continue;
                            if (!item.Name.Contains("Scythe") && item.GetType() == toolType)
                            {
                                if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                                {
                                    ChangeToolbar(BotBase.Farmer.Items.IndexOf(item));
                                    BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                                    return;
                                }
                            }
                        }

                        return;
                }

                break;
            default: // non melee weapon items
                foreach (var item in BotBase.Farmer.Items)
                {
                    if (item is null) continue;
                    if (item.GetType() != toolType) continue;
                    
                    if (BotBase.Farmer.CurrentToolIndex == BotBase.Farmer.Items.IndexOf(item)) continue;
                    
                    ChangeToolbar(BotBase.Farmer.Items.IndexOf(item));
                    BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                    return;
                }

                return;
        }
    }

    public static void SwapObject(Object obj)
    {
        foreach (var item in BotBase.Farmer.Items)
        {
            if (item is null) continue;
            if (item != obj) continue;
            if (BotBase.Farmer.CurrentToolIndex == BotBase.Farmer.Items.IndexOf(item)) return;
            
            ChangeToolbar(BotBase.Farmer.Items.IndexOf(item));
            BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);

            return;
        }
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