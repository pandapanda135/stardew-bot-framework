using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

// possibly move these all to separate files in a separate directory?
public class Goal
{
    public int X;
    public int Y;

    public Point VectorLocation => new(X, Y);

    /// <summary>
    /// Calculate heuristic from node provided to goal 
    /// </summary>
    /// <param name="node"><see cref="PathNode"/></param>
    /// <returns>estimated distance to end</returns>
    public int ManhattanHeuristic(PathNode node)
    {
        return Math.Abs(X - node.X) + Math.Abs(Y - node.Y);
    }

    /// <summary>
    /// Check if node is at end
    /// </summary>
    /// <param name="node">The node you want to now if it is end</param>
    /// <returns>true if end otherwise false </returns>
    private bool IsEnd(PathNode node)
    {
        return VectorLocation == node.VectorLocation;
    }

    private bool IsInEndRadius(PathNode node, int radius, bool cardinal = false)
    {
        return IsInEndRadius(node.VectorLocation, radius, cardinal);
    }
    
    private bool IsInEndRadius(Point node,int radius,bool cardinal = false)
    {
        if (cardinal && !(node.X != X && node.Y == Y ||
                         node.X == X && node.Y != Y))
        {
            return false;
        }
        
        return Math.Abs(node.X - X) <= radius && Math.Abs(node.Y - Y) <= radius;
    }

    public virtual bool CanEnd(PathNode node, bool cardinal)
    {
        return VectorLocation == node.VectorLocation;
    }
    /// <summary>
    /// A specific position the bot should stand on.
    /// </summary>
    public class GoalPosition : Goal
    {
        public GoalPosition(int x,int y)
        {
            X = x;
            Y = y;
        }

        public override bool CanEnd(PathNode node, bool cardinal) => IsEnd(node);
    }

    /// <summary>
    /// A position the bot should stand within a user set radius of.
    /// </summary>
    public class GoalNearby : Goal
    {
        private readonly int _radius;

        public GoalNearby(int x, int y, int radius)
        {
            X = x;
            Y = y;
            _radius = radius;
        }

        public override bool CanEnd(PathNode node, bool cardinal) => IsInEndRadius(node, _radius, cardinal);
    }

    /// <summary>
    /// A position the bot should stand next to, meant for chests and items like it.
    /// </summary>
    public class GetToTile : Goal
    {
        private readonly int _radius = 1;
        
        public GetToTile(int x,int y)
        {
            X = x;
            Y = y;
        }

        public override bool CanEnd(PathNode node, bool cardinal) => IsInEndRadius(node, _radius, cardinal);
    }

    /// <summary>
    /// A goal that moves meant for npcs, may change in the future
    /// </summary>
    public class GoalDynamic : Goal
    {
        public readonly Character Character;
        private readonly int _radius;

        public GoalDynamic(Character character, int range)
        {
            X = character.TilePoint.X;
            Y = character.TilePoint.Y;
            Character = character;
            _radius = range;
        }

        public override bool CanEnd(PathNode node, bool cardinal) => IsInEndRadius(Character.TilePoint, _radius, cardinal);
    }
}