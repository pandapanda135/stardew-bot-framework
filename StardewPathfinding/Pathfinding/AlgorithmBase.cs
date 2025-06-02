using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewValley;
using xTile.Tiles;

namespace StardewPathfinding.Pathfinding;

public class AlgorithmBase
{
    public Character? Character;

    public Stack<PathNode>? PathToEndPoint = new();

    public GameLocation? CurrentLocation;

    public List<Point>? EndPoints;

    // will be used to see if destroying stuff like trees is allowed in pathfinding
    public bool AllowDestruction;

    /// <summary>
    /// Also known as frontier, this contains nodes it will go to next
    /// </summary>
    public Queue<PathNode>? OpenList = new Queue<PathNode>();

    /// <summary>
    /// this contains Nodes the pathfinding has already reached
    /// </summary>
    public HashSet<PathNode>? ClosedList = new HashSet<PathNode>();

    public AlgorithmBase(Character character,GameLocation currentLocation,List<Point> endPoints)
    {
        Character = character;
        CurrentLocation = currentLocation;
        EndPoints = endPoints;
    }

    public AlgorithmBase()
    {}
    
    // ,GameLocation location, Character character

    
    // this is so multiple types of pathing can be implemented more easily
    public interface IPathing
    {
        protected static PathQueue Frontier = new();

        protected static PathPriorityQueue PriorityFrontier = new();
        
        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation location, Character character, int limit);
        
        public Stack<PathNode> RebuildPath(PathNode startPoint,PathNode endNode,Stack<PathNode> path);
    }
}