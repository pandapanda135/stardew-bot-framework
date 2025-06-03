using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewValley;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StardewPathfinding.Pathfinding;

public class PathNode : IComparable<PathNode>
{
    public readonly int X;

    public readonly int Y;

    public PathNode? Parent;

    public int Cost;
    
    public int id;
    
    public PathNode(int x,int y,PathNode? parent,int cost = -1)
    {
        X = x;
        Y = y;
        Parent = parent;
        id = CalculateHash(x, y);
        Cost = 1;
    }
    
    public static int CalculateHash(int x,int y)
    {
        return 1000 * x + y;
    }
    
    /// <summary>
    /// Returns if Tile at x and y has collisions
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <returns>if true is not passable else Passable </returns>
    public static bool IsNotPassable(int x, int y)
    {
        return Game1.currentLocation.isCollidingPosition(new Rectangle(x * Game1.tileSize + 1, y * Game1.tileSize + 1, 62, 62), Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);
    }

    public int CompareTo(PathNode? other)
    {
        if (other.X == X && other.Y == Y && other.Parent == Parent) return 0;

        if (other is not PathNode) return -1;

        return 1;
    }

    public static int ManhattanHeuristic(Vector2 start,Vector2 end)
    {
        Logger.Info($"X:{start.X - end.X}   Y: {start.Y - end.Y}");
        return (int)Math.Abs(start.X - end.X) + (int)Math.Abs(start.Y - end.Y);
    }
}