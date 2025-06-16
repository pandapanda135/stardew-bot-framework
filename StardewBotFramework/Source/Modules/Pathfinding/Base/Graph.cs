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
            
            nextNodes.Enqueue(new PathNode(neighborX, neighborY, currentNode));
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