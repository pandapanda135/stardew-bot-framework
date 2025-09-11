using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public interface IGraph
{
    protected static readonly sbyte[,] Directions = new sbyte[,]
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
    /// This is so the index is the same as <see cref="Farmer.FacingDirection"/>
    /// </summary>
    protected static readonly sbyte[,] GroupDirections = new sbyte[,]
    {
        { 0, -1 }, // north
        { 1, 0 }, // east
        { 0, 1 }, // south
        { -1, 0 }, // west
        
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
}