using System.Collections.Concurrent;
using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding;

/// <summary>
/// A precomputed list of all blocked tiles.
/// </summary>
public class CollisionMap
{
	/// <summary>
	/// These are the blocked tiles, this should only be accessed if necessary.
	/// </summary>
    public readonly ConcurrentDictionary<(int x, int y),byte> BlockedTiles = new();

    /// <summary>
    /// Add a new tile to the list.
    /// </summary>
    public void AddBlockedTile(int x, int y) => BlockedTiles.TryAdd((x, y),byte.MinValue);

    /// <summary>
    /// Remove a tile from the list.
    /// </summary>
    public void RemoveBlockedTile(int x, int y) => BlockedTiles.TryRemove((x, y), out _);

    /// <summary>
    /// If the provided tile has collisions
    /// </summary>
    /// <returns>true if it has collisions otherwise false</returns>
    public bool IsBlocked(int x, int y) => BlockedTiles.ContainsKey((x, y));

    /// <summary>
    /// Remove all tiles from the map.
    /// </summary>
    public void Clear() => BlockedTiles.Clear();
    
    /// <summary>
    /// Query if the tile has collisions, it will not get this from the collision map but from the game. Use IsBlocked for the collision map.
    /// </summary>
    /// <returns>if true is not passable else Passable</returns>
    public static bool IsCurrentlyBlocked(GameLocation location,int x,int y)
    {
	    foreach (var door in location.interiorDoors.Doors.Where(door => door.Position == new Point(x,y)))
	    {
		    // This means if it is currently open, I don't think there is a way to know if it can be opened so just going to rely on this for now.
		    return !door.Value;
	    }
	    
	    Object? obj = location.getObjectAtTile(x, y);
        if (obj is Fence fence && fence.isGate.Value) return false;
        
        return location.isCollidingPosition(new Rectangle(x * Game1.tileSize + 1, y * Game1.tileSize + 1, 
		        62, 62), Game1.viewport, isFarmer: true, -1, glider: false, BotBase.Farmer,
	        true,false,false,true);
    }
}