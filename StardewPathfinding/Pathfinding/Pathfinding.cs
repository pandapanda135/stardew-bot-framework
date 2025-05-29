using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewValley;

namespace StardewPathfinding.Pathfinding;

public class Pathfinding
{
    public Character? Character;

    public Stack<PathNode>? PathToEndPoint = new Stack<PathNode>();

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
    
    public Pathfinding(Character character,GameLocation currentLocation,List<Point> endPoints)
    {
        Character = character;
        CurrentLocation = currentLocation;
        EndPoints = endPoints;
    }

    public Pathfinding()
    {}
    
    // ,GameLocation location, Character character
    public static bool CheckIfEnd(PathNode currentNode,Point endPoint)
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
            int neighborX = currentNode.X + Directions[i, 0]; // index out of bounds error
            int neighborY = currentNode.Y + Directions[i, 1];

            nextNodes.Enqueue(new PathNode(neighborX, neighborY, currentNode));
            DrawFoundTiles.debugDirectionTiles.Enqueue(new PathNode(neighborX,neighborY,currentNode));
        }

        return nextNodes;
    }
    
    // this is so multiple types of pathing can be implemented more easily
    public interface IPathing
    {
        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation location, Character character, int limit);
        
        public Stack<PathNode> RebuildPath(PathNode startPoint,PathNode endNode,Stack<PathNode> path);
    }
}