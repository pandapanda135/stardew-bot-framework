using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewPathfinding.Pathfinding;

public class Pathfinding
{
    public Character Character;

    public Stack<Point> PathToEndPoint;

    public GameLocation CurrentLocation;

    public List<Point> EndPoints;

    // will be used to see if destroying stuff like trees is allowed in pathfinding
    public bool AllowDestruction;

    public Queue<PathNode> OpenList;

    public HashSet<PathNode> ClosedList;
    
    private static readonly sbyte[,] Directions = new sbyte[8,2]
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

    public bool CheckIfEnd(PathNode currentnode,PathNode endPoint,GameLocation location, Character character)
    {
        if (currentnode.X == endPoint.X && currentnode.Y == endPoint.Y)
        {
            return true;
        }
        return false;
    }

    public List<PathNode> Neighbors(PathNode current)
    {
        List<PathNode> neighbors = new List<PathNode>();
        for (int i = 0; i <= 4; i++)
        {
            int neighborX = current.X + Directions[current.X, 0];
            int neighborY = current.X + Directions[current.Y, 1];

            neighbors.Add(new PathNode(neighborX, neighborY, current.id));
        }

        return neighbors;
    }
    
    // this is so multiple types of pathing can be implemented more easily
    protected interface IPathing
    {
        public Stack<Point> FindPath(Point startPoint, Point endPoint, GameLocation location, Character character, int limit);
    }
}