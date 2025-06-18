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
    /// If the provided tile has collisions
    /// </summary>
    /// <returns>true if it has collisions otherwise false</returns>
    public bool IsBlocked(int x, int y) => BlockedTiles.Contains((x, y));
}