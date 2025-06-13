using StardewValley;
using StardewValley.Inventories;
using StardewValley.Objects;

namespace StardewBotFramework.Source.Modules;

public class ChestModule
{
    public Inventory GetItems(string GlobalChestInventoryId)
    {
        return Game1.player.team.GetOrCreateGlobalInventory(GlobalChestInventoryId);
    }

    public void TakeItemFromChest(Chest chest, Item item, Farmer player)
    {
        chest.grabItemFromChest(item,player);
    }

    public void PutItemInChest(Chest chest, Item item)
    {
        chest.addItem(item);
    }
}