using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding;

/// <summary>
/// A precomputed list of all blocked tiles.
/// </summary>
public class CollisionMap
{
    public HashSet<(int x, int y)> _blockedTiles = new();

    /// <summary>
    /// Add a new tile to the list.
    /// </summary>
    public void AddBlockedTile(int x, int y) => _blockedTiles.Add((x, y));

    /// <summary>
    /// If the provided tile has collisions
    /// </summary>
    /// <returns>true if it has collisions otherwise false</returns>
    public bool IsBlocked(int x, int y) => _blockedTiles.Contains((x, y));
}