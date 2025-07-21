using Netcode;
using StardewValley;
using StardewValley.ItemTypeDefinitions;
using StardewValley.Tools;

namespace StardewBotFramework.Source.ObjectToolSwaps;

// general item swapping implementation takes heavy inspiration from https://github.com/Caua-Oliveira/StardewMods/blob/main/AutomateToolSwap/InteractionRules
public class SwapItemHandler
{
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
                            if (item.Name == "Scythe" && item.GetType() == toolType)
                            {
                                if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                                {
                                    BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                                }

                                return;
                            }
                        }

                        return;
                    case "Weapon":
                        foreach (var item in BotBase.Farmer.Items)
                        {
                            if (!item.Name.Contains("Scythe") && item.GetType() == toolType)
                            {
                                if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                                {
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
                    if (item.GetType() == toolType)
                    {
                        if (BotBase.Farmer.CurrentToolIndex != BotBase.Farmer.Items.IndexOf(item))
                        {
                            BotBase.Farmer.CurrentToolIndex = BotBase.Farmer.Items.IndexOf(item);
                            return;
                        }
                    }
                }

                return;
        }
    }
}