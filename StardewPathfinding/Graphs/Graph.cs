using System.Reflection.Metadata;
using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.Pathfinding;
using StardewPathfinding.TileInterface;
using StardewValley;
using xTile;
using Object = System.Object;

namespace StardewPathfinding.Graphs;

public class Graph : IGraph
{
    
    // this may get removed later
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
    
    public virtual bool CheckIfEnd(PathNode currentNode,Point endPoint)
    {
        if (currentNode.X == endPoint.X && currentNode.Y == endPoint.Y)
        {
            return true;
        }
        return false;
    }

    public Queue<PathNode> Neighbours(PathNode currentNode)
    {
        Queue<PathNode> nextNodes = new();
        // get tiles in cardinal directions
        for (int i = 0; i <= 3; i++)
        {
            int neighborX = currentNode.X + IGraph.Directions[i, 0]; // index out of bounds error
            int neighborY = currentNode.Y + IGraph.Directions[i, 1];

            if (PathNode.IsNotPassable(neighborX, neighborY)) continue;
            
            nextNodes.Enqueue(new PathNode(neighborX, neighborY, currentNode));
            DrawFoundTiles.debugDirectionTiles.Enqueue(new PathNode(neighborX,neighborY,currentNode));
        }

        return nextNodes;
    }

    /// <summary>
    /// This calculates the next cost between the current node and the next one 
    /// </summary>
    /// <param name="current">current node</param>
    /// <param name="next">Next node</param>
    /// <returns>the cost between the current node and next as an integer </returns>
    public static int Cost(PathNode current, PathNode next)
    {
        return next.Cost;
    }
}