using Microsoft.Xna.Framework;
using StardewValley;
using Object = System.Object;

namespace StardewPathfinding.TileInterface;

/// <summary>
/// This should be used to send the tiles that are available to the pathfinding.
/// </summary>
public interface IPathfindingTiles
{
    /// <summary>
    /// This should be used to give the path-finding algorithms the tiles they can not access.
    /// </summary>
    /// <param name="tiles">This is an optional param that can be used to add the tiles as a list. By default, this is null.</param>
    /// <returns>The tiles should be returned.</returns>
    List<Object> GetBadTiles(object? tiles = null);
    
    /// <summary>
    /// This should be used to give the path-finding algorithms the tiles they can access.
    /// </summary>
    /// <param name="tiles">This is an optional param that can be used to add the tiles as a list. By default, this is null.</param>
    /// <returns>The tiles should be returned.</returns>
    List<Object> GetAvailableTiles(object? tiles = null);
    
    /// <summary>
    /// This will be used to give the graph the locations
    /// </summary>
    /// <param name="endLocations">these are the locations that will be used</param>
    /// <returns></returns>
    List<Object> AddLocations(Point[] endLocations);
}