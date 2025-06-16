using Microsoft.Xna.Framework;
using StardewValley;
using xTile;
using Object = System.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class Graph : IGraph
{
    public async Task<Queue<PathNode>> Neighbours(PathNode currentNode)
    {
        Queue<PathNode> nextNodes = new();
        // get tiles in cardinal directions
        for (int i = 0; i <= 3; i++)
        {
            int neighborX = currentNode.X + IGraph.Directions[i, 0];
            int neighborY = currentNode.Y + IGraph.Directions[i, 1];
            
            // if (await IsCollidingAsync(neighborX, neighborY)) continue;
            // if (PathNode.IsNotPassable(neighborX, neighborY)) continue;
            
            nextNodes.Enqueue(new PathNode(neighborX, neighborY, currentNode));
        }

        return nextNodes;
    }

    // we do this as otherwise we get an issue with getting a non-concurrent hashmap
    async Task<bool> IsCollidingAsync(int x,int y)
    {
        var tcs = new TaskCompletionSource<bool>();
        AlgorithmBase.IPathing.PendingCollisionChecks.Enqueue(new CollisionCheck
        (
            new Rectangle(x * Game1.tileSize + 1, y * Game1.tileSize + 1, 62, 62),
            Game1.viewport,
            true,
            Game1.player,
            Game1.currentLocation,
            tcs
        ));

        return await tcs.Task; // blocks until main thread responds
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