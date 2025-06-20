using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding;

/// <summary>
/// A precomputed list of all blocked tiles.
/// </summary>
public class CollisionMap
{
    public HashSet<(int x, int y)> BlockedTiles = new();

    /// <summary>
    /// Add a new tile to the list.
    /// </summary>
    public void AddBlockedTile(int x, int y) => BlockedTiles.Add((x, y));

    /// <summary>
    /// Remove a tile from the list.
    /// </summary>
    public void RemoveBlockedTile(int x, int y) => BlockedTiles.Remove((x, y));

    /// <summary>
    /// If the provided tile has collisions
    /// </summary>
    /// <returns>true if it has collisions otherwise false</returns>
    public bool IsBlocked(int x, int y) => BlockedTiles.Contains((x, y));
    
    /// <summary>
    /// Query if the tile has collisions it will not get this from the collision map but from the game
    /// </summary>
    /// <returns>if true is not passable else Passable</returns>
    public static bool IsCurrentlyBlocked(int x,int y) => Game1.currentLocation.isCollidingPosition(new Rectangle(x * Game1.tileSize + 1, y * Game1.tileSize + 1, 62, 62), Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);
}