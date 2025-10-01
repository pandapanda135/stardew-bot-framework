using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Modules.Menus;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// For both UI and non-ui interaction with the shipping bin
/// </summary>
public class ShippingBinInteraction : GrabItemMenuInteraction
{
    /// <summary>
    /// Get the location of all shipping bins in the location.
    /// </summary>
    /// <param name="location"><see cref="GameLocation"/> of where you want to query</param>
    /// <returns>Left most Tile position of shipping bin</returns>
    public List<Point> GetShippingBinLocations(GameLocation location)
    {
        List<Point> tileLocations = new();
        foreach (var building in location.buildings)
        {
            if (building is ShippingBin){
            {
                Point tilePoint = new Point(building.tileX.Value, building.tileY.Value);
                tileLocations.Add(tilePoint);
            }}
        }

        return tileLocations;
    }
    /// <summary>
    /// Get all shipping bins in the location.
    /// </summary>
    /// <param name="location"><see cref="GameLocation"/> of where you want to query</param>
    /// <returns>A list of the shipping bins in the locations</returns>
    public List<ShippingBin> GetShippingBinsInLocation(GameLocation location)
    {
        List<ShippingBin> buildings = new();
        foreach (var building in location.buildings)
        {
            if (building is ShippingBin shippingBin){
            {
                buildings.Add(shippingBin);
            }}
        }

        return buildings;
    }
    
    /// <summary>
    /// Interact with shipping bin <see cref="Building"/> should not be in UI. Must be in range.
    /// </summary>
    public void ShipHeldItem(ShippingBin shippingBin)
    {
        shippingBin.leftClicked();
    }

    /// <summary>
    /// This dont work :(
    /// </summary>
    public void OpenBin(ShippingBin shippingBin)
    {
        // TODO: this does not work as need to use non-old MouseState
        shippingBin.doAction(new Vector2(shippingBin.tileX.Value, shippingBin.tileY.Value), BotBase.Farmer);
    }

    public Item? GetLastItem()
    {
        return Game1.getFarm().lastItemShipped;
    }

    public Item? GrabLastItem()
    { 
        if (_menu is null) return null;
        
        ClickableComponent cc = _menu.lastShippedHolder;
        
        Item? grabItem = GetLastItem();
        if (grabItem is null) return null;
        
        _menu.receiveLeftClick(cc.bounds.X, cc.bounds.Y);

        int emptyIndex = InventoryUtilities.GetFirstEmptySlot(BotBase.Farmer.Items);
        _menu.receiveLeftClick(_menu.inventory.inventory[emptyIndex].bounds.X, _menu.inventory.inventory[emptyIndex].bounds.Y);
        return grabItem;
    }

    /// <summary>
    /// Ship multiple items from inventory, must be in shipping bin ui
    /// </summary>
    public void ShipMultipleItems(Item[] items)
    {
        if (_menu is null) return;
        
        foreach (var item in items)
        {
            AddItem(item);
        }
    }
}