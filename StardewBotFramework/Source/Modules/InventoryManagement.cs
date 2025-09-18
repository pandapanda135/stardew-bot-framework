using Netcode;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using Logger = StardewBotFramework.Debug.Logger;

namespace StardewBotFramework.Source.Modules;

public class InventoryManagement
{
    /// <summary>
    /// The max amount of items in the bots inventory
    /// </summary>
    public int MaxInventory => BotBase.Farmer.MaxItems;

    public IList<Item> ToolBarItems => BotBase.Farmer.Items.GetRange(0, 11); // probably works

    public Inventory Inventory => GetInventory();
    
    #region GeneralMethods

    public Inventory GetInventory()
    {
        return Game1.player.Items;
    }
    
    /// <summary>
    /// Drop item from local player inventory, will always drop full stack.
    /// </summary>
    /// <param name="item"><see cref="Item"/> to be removed.</param>
    /// <returns>true if dropped successfully else return false.</returns>
    public bool ThrowItem(Item item)
    {
        if (!Game1.player.Items.Contains(item)) return false;
            
        Game1.player.dropItem(item);
        Game1.player.removeItemFromInventory(item);
        return true;
    }

    /// <summary>
    /// Bin item in local player inventory, will always remove full stack.
    /// </summary>
    /// <param name="item"><see cref="Item"/> to be removed.</param>
    public void BinItem(Item item)
    {
        Utility.trashItem(item);
    }

    /// <summary>
    /// Will add item to position in player inventory
    /// </summary>
    /// <param name="item">Item to be moved in inventory</param>
    /// <param name="position">Position to be moved in inventory from 0-35</param>
    /// <param name="callbackMethod">The callback to invoke when an item is added to the inventory.</param>
    /// <returns>If the item was fully added to the inventory, returns <c>null</c>. If it replaced an item stack previously at that position, returns the replaced item stack. Else returns the input item with its stack reduced to the amount that couldn't be added. If it is not already in inventory will return item param</returns>
    public Item MoveItem(Item item, int position,ItemGrabMenu.behaviorOnItemSelect? callbackMethod = null) // maybe change this to int of slot not 100% sure on that due to being able to change inventory
    {
        if (!Game1.player.Items.Contains(item)) return item;

        Utility.removeItemFromInventory(GetItemIndex(GetInventory(), item),GetInventory());
        return Utility.addItemToInventory(item, position, Game1.player.Items, callbackMethod);
    }

    /// <summary>
    /// Will add item to bot's inventory if there is space for it.
    /// </summary>
    /// <param name="item">Item to be added to inventory.</param>
    /// <returns>If item was fully added to inventory will return null. Otherwise, will either return the same item that was given or item with a lowered stack</returns>
    public Item? AddItemToInventory(Item item)
    {
        if (!Game1.player.addItemToInventoryBool(item)) // if true could not add any of the item to the inventory
        {
            if (item.Stack == 0)
                return null;
            
            return item;
        }
        return item;
    }
    
    /// <summary>
    /// Shift toolbar one step
    /// </summary>
    /// <param name="right">if true will shift to the right else left</param>
    /// <param name="amount">How many times you want to shift toolbar</param>
    public void SelectInventoryRowForToolbar(bool right,int amount = 1) // row of items should probably be from 0-2 or 1-3 
    {
        for (int i = 0; i < amount; i++)
        {
            Game1.player.shiftToolbar(right);
        }
    }

    /// <summary>
    /// Change currently selected slot in Toolbar
    /// </summary>
    /// <param name="slot">index to be selected from 0-11 else it will return</param>
    public void SelectSlot(int slot)
    {
        if (slot < 0 || slot > 11)
        {
            return;
        }

        Game1.player.CurrentToolIndex = slot;
    }
    
    private int GetItemIndex(Inventory inventory,Item item)
    {
        int index = new();
        for (int i = 0; i < inventory.Count; i++)
        {
            if (!inventory.Contains(item))
            {
                return -1;
            }

            if (inventory[i] != item) continue;

            index = i;
        }

        return index;
    }
    #endregion

    #region ClothingItems

    public Item? ChangeRings(Ring? ring, bool topRing)
    {
        NetRef<Ring> ringSlot = topRing ? BotBase.Farmer.leftRing : BotBase.Farmer.rightRing;
        
        Ring previousClothing = ringSlot.Value;

        if (ring == null)
        {
            if (!BotBase.Farmer.addItemToInventoryBool(previousClothing))
            {
                BotBase.Farmer.dropItem(previousClothing);
            }
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(null, ringSlot!));
        }
        
        if (ringSlot.Value != null && BotBase.Farmer.Items.Contains(ring))
        {
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(ring, ringSlot));
        }

        return null;
    }

    public Item? ChangeClothing(bool shirt,Clothing? clothing)
    {
        NetRef<Clothing> clothingSlot = shirt ? BotBase.Farmer.shirtItem : BotBase.Farmer.pantsItem;

        Clothing previousClothing = clothingSlot.Value;

        if (clothing == null)
        {
            if (!BotBase.Farmer.addItemToInventoryBool(previousClothing))
            {
                BotBase.Farmer.dropItem(previousClothing);
            }
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(null, clothingSlot!));
        }
        
        if (clothingSlot.Value != null && BotBase.Farmer.Items.Contains(clothing))
        {
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(clothing, clothingSlot!));
        }

        return null;
    }

    public Item? ChangeHat(Hat? hat)
    {
        NetRef<Hat> hatSlot = BotBase.Farmer.hat;
        
        Hat previousClothing = hatSlot.Value;

        if (hat == null)
        {
            if (!BotBase.Farmer.addItemToInventoryBool(previousClothing))
            {
                BotBase.Farmer.dropItem(previousClothing);
            }
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(null, hatSlot!));
        }

        if (hatSlot.Value != null && BotBase.Farmer.Items.Contains(hat))
        {
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(hat, hatSlot!));
        }

        return null;
    }

    public Item? ChangeBoots(Boots? boots)
    {
        NetRef<Boots> bootsSlot = BotBase.Farmer.boots;
        
        Boots previousClothing = bootsSlot.Value;

        if (boots == null)
        {
            if (!BotBase.Farmer.addItemToInventoryBool(previousClothing))
            {
                BotBase.Farmer.dropItem(previousClothing);
            }
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(null, bootsSlot!));
        }

        if (bootsSlot.Value != null && BotBase.Farmer.Items.Contains(boots))
        {
            return Utility.PerformSpecialItemGrabReplacement(BotBase.Farmer.Equip(boots, bootsSlot!));
        }

        return null;
    }

    public Dictionary<string,Item> GetEquippedClothing()
    {
        Dictionary<string,Item> clothing = new()
        {
            // only the best code
            {"Boots", BotBase.Farmer.boots.Value},
            { "Hat",BotBase.Farmer.hat.Value },
            { "Shirt" ,BotBase.Farmer.shirtItem.Value},
            { "Pants",BotBase.Farmer.pantsItem.Value},
            { "Right ring",BotBase.Farmer.rightRing.Value},
            { "Left ring",BotBase.Farmer.leftRing.Value}
        };
        return clothing;
    }

    #endregion
    
    #region Trinkets

    /// <summary>
    /// Returns all currently equipped <see cref="Trinket"/> by <see cref="Farmer"/>
    /// </summary>
    /// <param name="player">player you want to know the trinkets of</param>
    /// <returns><see cref="NetList{T,TField}"/> of Trinkets</returns>
    public NetList<Trinket, NetRef<Trinket>> GetCurrentEquippedTrinkets(Farmer player)
    {
        return player.trinketItems;
    }

    /// <summary>
    /// Equip trinket in specified slot, If existing trinket is already in that slot will move it into inventory. If inventory is full will drop old trinket. 
    /// </summary>
    /// <param name="newTrinket">Trinket to be equipped</param>
    /// <param name="slot">slot to be equipped in can get amount of slots with Game1.player.stats.Get("trinketSlots")</param>
    public void EquipTrinket(Trinket newTrinket, int slot)
    {
        Trinket oldTrinket = Game1.player.trinketItems[slot]; 
        
        Game1.player.trinketItems[slot] = newTrinket;

        // if (Game1.player.trinketItems.Count > Game1.player.stats.Get("trinketSlots"))

        if (oldTrinket == null && Game1.player.trinketItems.Count > Game1.player.stats.Get("trinketSlots")) return;

        if (Utility.canItemBeAddedToThisInventoryList(oldTrinket, Game1.player.Items))
            Game1.player.addItemToInventory(oldTrinket);

        Game1.player.dropItem(oldTrinket);
    }

    /// <summary>
    /// Remove existing trinket for player inventory.
    /// </summary>
    /// <param name="trinket">trinket to be removed.</param>
    public void RemoveTrinket(Trinket trinket)
    {
        if (!Game1.player.trinketItems.Contains(trinket)) return;

        int trinketIndex = Game1.player.trinketItems.IndexOf(trinket);
        
        Trinket oldTrinket = Game1.player.trinketItems[trinketIndex]; 
        
        Game1.player.trinketItems.RemoveAt(trinketIndex);

        if (Utility.canItemBeAddedToThisInventoryList(oldTrinket, Game1.player.Items))
        {
            Game1.player.addItemToInventory(oldTrinket);
            return;
        }
        Game1.player.dropItem(oldTrinket);
    }

    
    /// <summary>
    /// Remove existing trinket for player inventory at specified slot.
    /// </summary>
    /// <param name="slot">The slot you want the trinket to be removed from</param>
    public void RemoveTrinket(int slot)
    {
        if (slot > Game1.player.stats.Get("trinketSlots") || Game1.player.trinketItems[slot] == null) return;
        
        Trinket trinket = Game1.player.trinketItems[slot];
        
        Game1.player.trinketItems.RemoveAt(slot);
        
        if (Utility.canItemBeAddedToThisInventoryList(trinket, Game1.player.Items))
        {
            Game1.player.addItemToInventory(trinket);
            return;
        }
        Game1.player.dropItem(trinket);
    }
    
    #endregion

    #region Crafting

    /// <summary>
    /// Get all unlocked <see cref="CraftingRecipe"/> of this player
    /// </summary>
    /// <returns>A List of <see cref="CraftingRecipe"/></returns>
    public List<CraftingRecipe> PossibleCrafts()
    {
        List<CraftingRecipe> unlockedRecipes = new();
        foreach (SerializableDictionary<string,int> craftingRecipe in Game1.player.craftingRecipes)
        {
            foreach (KeyValuePair<string,int> kvp in craftingRecipe) // value of this is how many have been made this is not used though
            {
                CraftingRecipe recipe = new CraftingRecipe(kvp.Key); // give name of item
                if (!unlockedRecipes.Contains(recipe))
                {
                    Logger.Info(recipe.name);
                    unlockedRecipes.Add(recipe);
                }
            }
        }
        
        return unlockedRecipes;
    }

    /// <summary>
    /// Craft specified item if you have the items
    /// </summary>
    /// <param name="item">The item to craft</param>
    public void CraftItem(CraftingRecipe item)
    {
        Item crafted = item.createItem();

        item.consumeIngredients(null);
        
        // replicate game
        Game1.playSound("coin");
        
        Logger.Info($"currentItem: {Game1.player.CurrentItem} current tool index: {Game1.player.CurrentToolIndex}");
        
        if (Game1.player.CurrentItem == null) MoveItem(crafted, Game1.player.CurrentToolIndex);
        
        AddItemToInventory(crafted);
    }
    #endregion

    #region Attach

    /// <summary>
    /// Attach an item to another.
    /// </summary>
    /// <param name="attachItem">Item to attach.</param>
    /// <param name="item">Item to attach to.</param>
    public void AttachItem(Item attachItem, Item item)
    {
        if (!Inventory.Contains(attachItem) || !Inventory.Contains(item))
        {
            return;
        }

        if (Game1.activeClickableMenu is not GameMenu menu) return;
        if (menu.GetCurrentPage() is not InventoryPage page) return;
        
        int itemIndex = Inventory.IndexOf(item);
        ClickableComponent cc = page.inventory.inventory[itemIndex];
        page.inventory.rightClick(cc.bounds.X, cc.bounds.Y, attachItem);
        Inventory.Remove(attachItem);
    }

    public void RemoveAttached(Item item)
    {
        if (!Inventory.Contains(item))
        {
            return;
        }
        
        if (Game1.activeClickableMenu is not GameMenu menu) return;
        if (menu.GetCurrentPage() is not InventoryPage page) return;

        int itemIndex = Inventory.IndexOf(item);
        ClickableComponent cc = page.inventory.inventory[itemIndex];
        Item attach = page.inventory.rightClick(cc.bounds.X, cc.bounds.Y, null,false,true);
        page.inventory.rightClick(cc.bounds.X, cc.bounds.Y, null);
        for (int i = 0; i < Inventory.Count; i++) // get first open slot
        {
            if (Inventory[i] is not null) continue;
            
            page.inventory.leftClick(page.inventory.inventory[i].bounds.X,page.inventory.inventory[i].bounds.Y,attach);
            return;
        }
        Game1.player.dropItem(attach);
    }

    #endregion
}
