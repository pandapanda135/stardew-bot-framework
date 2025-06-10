using Netcode;
using StardewValley;
using StardewValley.Objects.Trinkets;

namespace StardewBotFramework;

public class InventoryManagement
{
    const int HotBarCount = 12;
    const int MaxInventorySpace = Farmer.maxInventorySpace;
    public int CurrentItemSlotCount = Game1.player.MaxItems;

    private static readonly int[] EquippableSlots = new int[]{37,38,39,40,41,42}; // temporary so I don't forget need to do more digging in how the game handles it though

    // Items in Inventory should IList<Item> (presumably 0-35)
    
    #region GeneralMethods
        /// <summary>
        /// Drop item from local player inventory, will always drop full stack.
        /// </summary>
        /// <param name="item"><see cref="Item"/> to be removed.</param>
        /// <returns>true if dropped successfully else return false.</returns>
        public static bool ThrowItem(Item item)
        {
            if (!Game1.player.Items.Contains(item)) return false;
                
            Game1.player.dropItem(item);
            return true;
        }

        /// <summary>
        /// Bin item in local player inventory, will always remove full stack.
        /// </summary>
        /// <param name="item"><see cref="Item"/> to be removed.</param>
        public static void BinItem(Item item)
        {
            Utility.trashItem(item);
        }

        /// <summary>
        /// Will add item to position in player inventory
        /// </summary>
        /// <param name="item">Item to be moved in inventory</param>
        /// <param name="position">Position to be moved in inventory from 0-35</param>
        /// <returns>If the item was fully added to the inventory, returns <c>null</c>. If it replaced an item stack previously at that position, returns the replaced item stack. Else returns the input item with its stack reduced to the amount that couldn't be added. If it is not already in inventory will return item param</returns>
        public static Item MoveItem(Item item, int position) // maybe change this to int of slot not 100% sure on that due to being able to change inventory
        {
            if (!Game1.player.Items.Contains(item)) return item;
            
            return Utility.addItemToInventory(item, position, Game1.player.Items, null);
        }

        /// <summary>
        /// Shift toolbar one step
        /// </summary>
        /// <param name="right">if true will shift to the right else left</param>
        public static void SelectInventoryRowForToolbar(bool right) // row of items should probably be from 0-2 or 1-3 
        {
            Game1.player.shiftToolbar(right);
        }

        /// <summary>
        /// Change currently selected slot in Toolbar
        /// </summary>
        /// <param name="slot">index to be selected from 0-11 else it will return</param>
        public static void SelectSlot(int slot)
        {
            if (slot < 0 || slot > 11)
            {
                return;
            }

            Game1.player.CurrentToolIndex = slot;
        }
    #endregion

    #region Trinkets

    /// <summary>
    /// Returns all currently equipped <see cref="Trinket"/> by <see cref="Farmer"/>
    /// </summary>
    /// <param name="player">player you want to know the trinkets of</param>
    /// <returns><see cref="NetList{T,TField}"/> of Trinkets</returns>
    public static NetList<Trinket, NetRef<Trinket>> GetCurrentEquippedTrinkets(Farmer player)
    {
        return player.trinketItems;
    }

    /// <summary>
    /// Equip trinket in specified slot, If existing trinket is already in that slot will move it into inventory. If inventory is full will drop old trinket. 
    /// </summary>
    /// <param name="newTrinket">Trinket to be equipped</param>
    /// <param name="slot">slot to be equipped in can get amount of slots with Game1.player.stats.Get("trinketSlots")</param>
    public static void EquipTrinket(Trinket newTrinket, int slot)
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
    public static void RemoveTrinket(Trinket trinket)
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
    public static void RemoveTrinket(int slot)
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
}