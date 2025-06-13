using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class PathNode : IComparable<PathNode>
{
    public readonly int X;

    public readonly int Y;

    public Point VectorLocation => new(X, Y);

    public PathNode? Parent;

    public int Cost;
    
    public PathNode(int x,int y,PathNode? parent,int cost = -1)
    {
        X = x;
        Y = y;
        Parent = parent;
        Cost = 1; // cost == -1 ? Random.Shared.Next(0, 6) : cost
    }

    public PathNode(Point vector, PathNode? parent, int cost = -1)
    {
        X = vector.X;
        Y = vector.Y;
        Parent = parent;
        Cost = 1; // cost == -1 ? Random.Shared.Next(0, 6) : cost
    }
    
    /// <summary>
    /// Returns if Tile at x and y has collisions
    /// </summary>
    /// <returns>if true is not passable else Passable </returns>
    public static bool IsNotPassable(int x, int y)
    {
        // TODO: Issue with crashing due to changing non-concurrent collection without exclusive access.
        return Game1.currentLocation.isCollidingPosition(new Rectangle(x * Game1.tileSize + 1, y * Game1.tileSize + 1, 62, 62), Game1.viewport, isFarmer: true, -1, glider: false, Game1.player);
    }

    public int CompareTo(PathNode? other) // why is this nullable?
    {
        if (other.VectorLocation == VectorLocation && other.Parent == Parent) return 0;

        if (other is not PathNode) return -1;

        return 1;
    }

    public static int ManhattanHeuristic(Vector2 start,Vector2 end)
    {
        Logger.Info($"X:{start.X - end.X}   Y: {start.Y - end.Y}");
        return (int)Math.Abs(start.X - end.X) + (int)Math.Abs(start.Y - end.Y);
    }
}