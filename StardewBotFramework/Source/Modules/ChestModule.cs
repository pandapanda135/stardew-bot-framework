using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;

namespace StardewBotFramework.Source.Modules;

public class ChestModule
{
    private IInventory CurrentChestInventory;
    
    private static bool ChestOpen => Game1.activeClickableMenu is ItemGrabMenu;
    
    /// <summary>
    /// Open a valid chest, this must be performed before being able to change the contents of the chest
    /// </summary>
    /// <param name="chest">The chest you want to open</param>
    /// <returns>Will return an <see cref="IInventory"/> of the current items in the chest</returns>
    public IInventory? OpenChest(Chest chest)
    {
        if (ChestOpen) return null;

        CurrentChestInventory = chest.GetItemsForPlayer();

        chest.ShowMenu();

        Game1.playSound(chest.fridge.Value ? "doorCreak" : "openChest");    

        // gift boxes cause crashes if performOpenChest runs
        if (chest.giftbox.Value || chest.giftboxIsStarterGift.Value) return CurrentChestInventory;
        
        chest.performOpenChest();
        
        return CurrentChestInventory;
    }

    public IInventory? SetChest(Chest chest)
    {
        CurrentChestInventory = chest.GetItemsForPlayer();
        return CurrentChestInventory;
    }
    
    /// <summary>
    /// Close a valid chest, this should be performed once the bot is done with a chest.
    /// </summary>
    /// <param name="chest">The chest you want to close</param>
    public void CloseChest(Chest? chest = null)
    {
        Logger.Warning("Running CloseChest");
        Game1.activeClickableMenu = null;
        // chest.frameCounter.Value = 0;
    }
    
    /// <summary>
    /// Get all items from chest, You must call OpenChest on a valid chest before running this.
    /// </summary>
    /// <param name="globalChestInventoryId">The chest's globalChestInventoryId</param>
    /// <returns>The chest's inventory</returns>
    public IInventory GetItems(string globalChestInventoryId)
    {
        if (!ChestOpen) return new Inventory();
        
        return Game1.player.team.GetOrCreateGlobalInventory(globalChestInventoryId);
    }
    
    /// <summary>
    /// Get all items from chest, You must call OpenChest on a valid chest before running this.
    /// </summary>
    /// <param name="chest">The chest you want the contents of.</param>
    /// <returns>The chest's inventory</returns>
    public IInventory GetItems(Chest chest)
    {
        if (!ChestOpen) return new Inventory();

        return chest.GetItemsForPlayer();
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
        
        // chest.grabItemFromInventory(item,player); // get from player inventory
        if (!CurrentChestInventory.Contains(item)) return;
        
        chest.grabItemFromChest(item,player);
        player.addItemToInventory(item);
    }

    /// <summary>
    /// Put an item in the chest, You must call OpenChest on a valid chest before running this.
    /// </summary>
    /// <param name="chest">The chest you want to add to</param>
    /// <param name="item">The item you want to add to the chest</param>
    /// <param name="player">The player to get the item from</param>
    public void PutItemInChest(Chest chest, Item item,Farmer player)
    {
        if (!ChestOpen) return;
        
        if (!player.Items.Contains(item)) return;
        
        chest.grabItemFromInventory(item,Game1.player);
        // chest.addItem(item);
    }
}