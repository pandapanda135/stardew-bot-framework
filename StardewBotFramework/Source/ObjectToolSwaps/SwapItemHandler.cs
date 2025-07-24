using Netcode;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;

namespace StardewBotFramework.Source.ObjectToolSwaps;

// general item swapping implementation takes heavy inspiration from https://github.com/Caua-Oliveira/StardewMods/blob/main/AutomateToolSwap/InteractionRules
// TODO: need to make this change UI aswell as current tool look at how UseToolOnGroup to see how it could be done
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
            case Type t when t == typeof(MeleeWeapon):
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
                                    MoveToolbar(BotBase.Farmer.Items.IndexOf(item));
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
                                    MoveToolbar(BotBase.Farmer.Items.IndexOf(item));
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
                    if (item.GetType() == toolType)
                    {
                        if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                        {
                            MoveToolbar(BotBase.Farmer.Items.IndexOf(item));
                            BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                            return;
                        }
                    }
                }

                return;
        }
    }

    private static void MoveToolbar(int index)
    {
        for (int i = 0; i < (int)Math.Floor((double)index / 11); i++)
        {
            BotBase.Farmer.shiftToolbar(true);    
        }
    }
}