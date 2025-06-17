using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Objects;

namespace StardewBotFramework.Source.Modules;

public class ChestModule
{

    protected bool ChestOpen = false;
    
    /// <summary>
    /// Open a valid chest, this must be performed before being able to change the contents of the chest
    /// </summary>
    /// <param name="chest">The chest you want to open</param>
    public void OpenChest(Chest chest)
    {
        if (ChestOpen) return;
        ChestOpen = true;
        Game1.playSound(chest.fridge.Value ? "doorCreak" : "openChest");
        chest.performOpenChest();
    }
    
    /// <summary>
    /// Close a valid chest, this should be performed once the bot is done with a chest.
    /// </summary>
    /// <param name="chest">The chest you want to close</param>
    public void CloseChest(Chest chest)
    {
        if (!ChestOpen) return;
        ChestOpen = false;
        chest.frameCounter.Value = 0;
    }
    
    /// <summary>
    /// Get all items from chest, You must call OpenChest on a valid chest before running this.
    /// </summary>
    /// <param name="globalChestInventoryId">The chest's globalChestInventoryId</param>
    /// <returns>The chest's inventory</returns>
    public Inventory GetItems(string globalChestInventoryId)
    {
        if (!ChestOpen) return new Inventory();
        
        return Game1.player.team.GetOrCreateGlobalInventory(globalChestInventoryId);
    }
    
    /// <summary>
    /// Take specific item from chest, You must call OpenChest on a valid chest before running this.
    /// </summary>
    /// <param name="chest">The chest you want to take an item from</param>
    /// <param name="item">The item you want to take</param>
    /// <param name="player">the player that should get the ite,</param>
    public void TakeItemFromChest(Chest chest, Item item, Farmer player)
    {
        if (!ChestOpen) return;
        
        chest.grabItemFromChest(item,player);
    }

    /// <summary>
    /// Put an item in the chest, You must call OpenChest on a valid chest before running this.
    /// </summary>
    /// <param name="chest">The chest you want to add to</param>
    /// <param name="item">The item you want to add to the chest</param>
    public void PutItemInChest(Chest chest, Item item)
    {
        if (!ChestOpen) return;
        
        chest.addItem(item);
    }
}