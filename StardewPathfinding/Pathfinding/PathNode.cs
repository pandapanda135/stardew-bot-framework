namespace StardewPathfinding.Pathfinding;

public class PathNode
{
    public readonly  int X;

    public readonly int Y;

    public PathNode? Parent;

    public int id;

    public PathNode(int x,int y,PathNode? parent)
    {
        X = x;
        Y = y;
        Parent = parent;
        id = CalculateHash(x, y);
    }
    
    public static int CalculateHash(int x,int y)
    {
        return 1000 * x + y;
    }
}