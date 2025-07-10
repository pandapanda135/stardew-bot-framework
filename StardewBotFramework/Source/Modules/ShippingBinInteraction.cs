using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Buildings;

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
    /// Get the all of the shipping bins in the location.
    /// </summary>
    /// <param name="location"><see cref="GameLocation"/> of where you want to query</param>
    /// <returns>A list of the shipping bins in the locations</returns>
    public List<ShippingBin> GetShippingBinsInLocation(GameLocation location)
    {
        List<ShippingBin> buildings = new();
        foreach (var building in location.buildings)
        {
            if (building is ShippingBin){
            {
                buildings.Add((ShippingBin)building);
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