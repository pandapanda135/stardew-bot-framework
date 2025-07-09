using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// For both UI and non-ui interaction with the shipping bin
/// </summary>
public class ShippingBinInteraction : GrabItemMenuInteraction
{
    /// <summary>
    /// Get the location of shipping bin.
    /// </summary>
    /// <param name="location"><see cref="GameLocation"/> of where you want to query</param>
    /// <returns>Tile position of shipping bin</returns>
    public List<Point> GetShippingBinLocation(GameLocation location)
    {
        if (location.Name != "Farm")
        {
            return new();
        }

        location.TryGetMapProperty("ShippingBinLocation", out var tile);
        string[] tileExtract = tile.Split(' ');
        List<Point> tileLocations = new();
        int runs = 0;
        for (int i = 0; i < tileExtract.Length / 2; i++)
        {
            Point shippingTile = new Point(int.Parse(tileExtract[runs]), int.Parse(tileExtract[runs + 1]));
                
            tileLocations.Add(shippingTile);
            runs += 2;
        }
        
        return tileLocations;
    }

    /// <summary>
    /// Interact with shipping bin <see cref="Building"/> cannot be in ui.
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
        foreach (var item in items)
        {
            AddItem(item);
        }
    }
}