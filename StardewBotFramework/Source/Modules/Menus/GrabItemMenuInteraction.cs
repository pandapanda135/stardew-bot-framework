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

    public void ChangeColour(int selection)
    {
        if (!Menu.CanHaveColorPicker() || !Menu.colorPickerToggleButton.visible) return;

        ClickableComponent cc = Menu.discreteColorPickerCC[selection];
        LeftClick(cc);
    }
}