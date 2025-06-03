using Microsoft.Xna.Framework;
using StardewPathfinding.Pathfinding;

namespace StardewPathfinding.Graphs;

public interface IGraph
{
    protected static readonly sbyte[,] Directions = new sbyte[8,2]
    {
        { -1, 0 }, // west
        { 1, 0 }, // east
        { 0, 1 }, // south
        { 0, -1 }, // north
        
        // diagonal 
        {-1,-1}, // north-west
        {1,-1}, // north-east
        {-1,1}, // south-west
        {1,1}, // south-east
    };
    
    /// <summary>
    /// Get surrounding tiles of node given.
    /// </summary>
    /// <param name="currentNode">Node you want the neighbours of.</param>
    /// <returns>Queue of Neighbours in order : west,east,south,north  (assuming the Neighbours do not go into an object the player can collide with.) </returns>
    public Queue<PathNode> Neighbours(PathNode currentNode);

    /// <summary>
    /// Check if node is at end
    /// </summary>
    /// <param name="currentNode">Node you want to find out if end</param>
    /// <param name="endPoint">The end Node as a <see cref="Microsoft.Xna.Framework.Point">Point</see></param>
    /// <returns>true if at end. False if not</returns>
    public bool CheckIfEnd(PathNode currentNode, Point endPoint);
}