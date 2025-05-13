using StardewPathfinding.Debug;
using StardewPathfinding.TileInterface;
using StardewValley;
using StardewValley.Delegates;
using StardewValley.Projectiles;
using xTile;
using Object = System.Object;

namespace StardewPathfinding.Graph;

public class Graph
{
    /// <summary>
    /// This will set up the pathfinding graph
    /// </summary>
    /// <param name="pathfindingTiles">This will be used for getting the available tiles.This should be set before Main is called</param>
    public static void Main(IPathfindingTiles pathfindingTiles)
    {
        Map map = Game1.currentLocation.Map;
        
        Logger.Info($"{map.DisplayHeight / Game1.tileSize}: {map.DisplayWidth / Game1.tileSize}, {map.DisplaySize}");

        List<Object> badTiles = pathfindingTiles.GetBadTiles();
        List<Object> availableTiles = pathfindingTiles.GetAvailableTiles();
        
        
        // need to set up queue system so we can get neighbors of a tile in this file using direction (or could do this Pathfinding)
    }
}