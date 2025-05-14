namespace StardewPathfinding.Pathfinding;

public class PathNode
{
    public readonly  int X;

    public readonly int Y;

    public int? Parent; // use id for parent?

    public int id;

    public PathNode(int x,int y,int? parent)
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