using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

/// <summary>
/// This is for UIs that typically have a square above the inventory that you either take an item from or into e.g. the shipping bin
/// </summary>
public class GrabItemMenuInteraction : MenuHandler
{
    public ItemGrabMenu Menu
    {
        get => _menu as ItemGrabMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _menu = value;
    }
    
    public void SetUI(ItemGrabMenu menu) => Menu = menu;
    
    public IList<Item> GetItemsInMenu() => Menu.ItemsToGrabMenu.actualInventory;
    
    /// <summary>
    /// Add item to this <see cref="ItemGrabMenu"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Will return false if this item cannot be added, else true.</returns>
    public bool AddItem(Item item)
    {
        if (!BotBase.Farmer.Items.Contains(item)) return false;

        int itemIndex = Menu.ItemsToGrabMenu.actualInventory.IndexOf(item);
        ClickableComponent cc = Menu.inventory.inventory[itemIndex];
        LeftClick(cc);
        return true;
    }

    public void TakeItem(Item item)
    {
        int index = Menu.ItemsToGrabMenu.actualInventory.IndexOf(item);
        if (index == -1) return;
        Rectangle rect = Menu.ItemsToGrabMenu.inventory[index].bounds;
        
        LeftClick(rect.X + 5,rect.Y + 5);
    }

    // public void AddItemAmount(Item item, int amount)
    // {
    //     if (!BotBase.Farmer.Items.Contains(item)) return;
    //
    //     int itemIndex = Menu.inventory.actualInventory.IndexOf(item);
    //     ClickableComponent cc = Menu.inventory.inventory[itemIndex];
    //
    //     ClickAmount(cc,amount);
    // }
    
    public void TakeItemAmount(Item item, int amount)
    {
        if (!BotBase.Farmer.Items.Contains(item)) return;

        int itemIndex = Menu.ItemsToGrabMenu.actualInventory.IndexOf(item);
        ClickableComponent cc = Menu.ItemsToGrabMenu.inventory[itemIndex];

        ClickAmount(cc,amount);
    }

    private void ClickAmount(ClickableComponent cc, int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            RightClick(cc);
        }
    }

    /// <summary>
    /// Click on the first empty slot in a menu.
    /// </summary>
    public void ClickFirstOpen(InventoryMenu menu)
    {
        foreach (var cc in menu.inventory.Where(cc => cc.item is null))
        {
            LeftClick(cc);
            break;
        }
    }

    public void ChangeColour(int selection)
    {
        if (!Menu.CanHaveColorPicker() || !Menu.colorPickerToggleButton.visible) return;

        ClickableComponent cc = Menu.discreteColorPickerCC[selection];
        LeftClick(cc);
    }

    /// <summary>
    /// Remove the amount from an item and return that new item. 
    /// </summary>
    /// <returns>Returns the amount of the item specified</returns>
    public Item? GetItemAmount(List<Item?> inventory,Item slotItem, int stack)
    {
        int slotIndex = inventory.IndexOf(slotItem);

        var newItem = inventory[slotIndex]?.getOne();
        if (newItem is null) return null;
        newItem.Stack = stack;
        Item? consumeItem = slotItem.ConsumeStack(newItem.Stack);
        if (consumeItem is null)
            ItemGrabBehaviour(inventory[slotIndex]);
        else
            inventory[slotIndex] = consumeItem;

        return newItem;
    }
    
    public void AddItemAmount(Item slotItem, int stack)
    {
        int slotIndex = Menu.inventory.actualInventory.IndexOf(slotItem);
        
        var newItem = Menu.inventory.actualInventory[slotIndex].getOne();
        newItem.Stack = stack;
        Menu.inventory.actualInventory[slotIndex] = slotItem.ConsumeStack(newItem.Stack);
        
        ItemSelectBehaviour(newItem);
    }
    
    public void RemoveItemAmount(Item slotItem, int stack)
    {
        int slotIndex = Menu.ItemsToGrabMenu.actualInventory.IndexOf(slotItem);
        
        var newItem = Menu.ItemsToGrabMenu.actualInventory[slotIndex].getOne();
        newItem.Stack = stack;
        Menu.ItemsToGrabMenu.actualInventory[slotIndex] = slotItem.ConsumeStack(newItem.Stack);
        
        ItemGrabBehaviour(newItem);
    }

    /// <summary>
    /// This is when an item is taken from the bot's inventory
    /// </summary>
    public void ItemSelectBehaviour(Item item)
    {
        Menu.behaviorFunction(item, BotBase.Farmer);
    }

    /// <summary>
    /// This is when an item is added to the bot's inventory
    /// </summary>
    public void ItemGrabBehaviour(Item item)
    {
        Menu.behaviorOnItemGrab(item, BotBase.Farmer);
    }
}