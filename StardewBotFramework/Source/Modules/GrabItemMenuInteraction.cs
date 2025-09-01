using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Inventories;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// This is for UIs that typically have a square above the inventory that you either take an item from or into e.g. the shipping bin
/// </summary>
public class GrabItemMenuInteraction
{
    protected ItemGrabMenu? _menu;
    
    public void SetUI(ItemGrabMenu menu)
    {
        _menu = menu;
    }

    public void RemoveUi()
    {
        _menu = null;
    }

    public IList<Item> GetItemsInMenu()
    {
        if (_menu is null) return new Inventory();
        return _menu.ItemsToGrabMenu.actualInventory;
    }
    
    /// <summary>
    /// Add item to this <see cref="ItemGrabMenu"/>
    /// </summary>
    /// <param name="item"></param>
    /// <returns>Will return false if this item cannot be added, else true.</returns>
    public bool AddItem(Item item)
    {
        if (_menu is null) return false;
        if (!BotBase.Farmer.Items.Contains(item)) return false;

        int itemIndex = _menu.ItemsToGrabMenu.actualInventory.IndexOf(item);
        ClickableComponent cc = _menu.inventory.inventory[itemIndex];
        _menu.receiveLeftClick(cc.bounds.X, cc.bounds.Y);
        return true;
    }

    public void TakeItem(Item item)
    {
        if (_menu is null) return;

        int index = _menu.ItemsToGrabMenu.actualInventory.IndexOf(item);
        if (index == -1) return;
        Rectangle rect = _menu.ItemsToGrabMenu.inventory[index].bounds;
        
        _menu.receiveLeftClick(rect.X + 5,rect.Y + 5);
    }
}