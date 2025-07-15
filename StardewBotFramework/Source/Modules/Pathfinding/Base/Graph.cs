using System.Text;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile;
using Object = System.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class Graph : IGraph
{
    public Queue<PathNode> Neighbours(PathNode currentNode)
    {
        Queue<PathNode> nextNodes = new();
        // get tiles in cardinal directions
        int directions = 3;
        for (int i = 0; i <= directions; i++)
        {
            int neighborX = currentNode.X + IGraph.Directions[i, 0];
            int neighborY = currentNode.Y + IGraph.Directions[i, 1];
            
            nextNodes.Enqueue(new PathNode(neighborX, neighborY, currentNode));
        }

        return nextNodes;
    }

    public Queue<Point> GroupNeighbours(Point currentTile,int directions)
    {
        Queue<Point> nextTiles = new();
        for (int i = 0; i <= directions; i++)
        {
            int neighborX = currentTile.X + IGraph.GroupDirections[i, 0];
            int neighborY = currentTile.Y + IGraph.GroupDirections[i, 1];
            
            nextTiles.Enqueue(new Point(neighborX, neighborY));
        }

        return nextTiles;
    }

    /// <summary>
    /// get if target tile is a neighbour is the start tile. This is from <see cref="GroupNeighbours"/>
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="targetTile"></param>
    /// <param name="direction">the direction the tile was in, if it is not a neighbour will return -1</param>
    /// <param name="directions">These are the directions you want to check 3 will be the cardinal directions with 7 including the tiles between them</param>
    /// <returns></returns>
    public static bool IsInNeighbours(Point startTile,Point targetTile,out int direction,int directions = 7)
    {
        for (int i = 0; i <= directions; i++)
        {
            int neighborX = startTile.X + IGraph.GroupDirections[i, 0];
            int neighborY = startTile.Y + IGraph.GroupDirections[i, 1];

            if (neighborX == targetTile.X && neighborY == targetTile.Y)
            {
                direction = i;
                return true;
            }
        }

        direction = -1;
        return false;
    }
    

    /// <summary>
    /// This calculates the next cost between the current node and the next one 
    /// </summary>
    /// <param name="current">current node</param>
    /// <param name="next">Next node</param>
    /// <returns>the cost between the current node and next as an integer </returns>
    public static int Cost(PathNode current, PathNode next)
    {
        return current.Cost - next.Cost;
    }
}