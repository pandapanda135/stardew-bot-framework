using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class WaterTile : ITile
{
    public GameLocation Location;

    /// <summary>
    /// The <see cref="WaterTiles.WaterTileData"/> of this tile, this will be null if it is not a water tile.
    /// </summary>
    public WaterTiles.WaterTileData? WaterTileData;
    
    /// <summary>
    /// Will be false if this water is from a building
    /// </summary>
    public bool NaturalWater;

    public int Cost = 0;
    
    public WaterTile(Point position,GameLocation location)
    {
        X = position.X;
        Y = position.Y;

        Location = location;
        NaturalWater = location.isOpenWater(X,Y); // This should remove water from buildings (like wells or fishponds)
        WaterTileData = location.waterTiles.waterTiles[X, Y];
        Cost = Heuristic(BotBase.Farmer.TilePoint);
    }
}