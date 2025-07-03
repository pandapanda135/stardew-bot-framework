using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
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
    public bool IsEnd(PathNode node)
    {
        if (VectorLocation == node.VectorLocation) return true;
        
        return false;
    }

    public bool IsInEndRadius(PathNode node,int radius)
    {
        for (int x = VectorLocation.X + radius; x > VectorLocation.X - radius; x--)
        {
            for (int y = VectorLocation.Y + radius; y > VectorLocation.Y - radius; y--)
            {
                if (node.X == x && node.Y == y) // check if node is in radius
                {
                    Logger.Warning($"IsInEndRadius is returning true");
                    return true;
                }
            }
        }
        
        return false;
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
    }

    /// <summary>
    /// A position the bot should stand within a user set radius of.
    /// </summary>
    public class GoalNearby : Goal
    {
        public int Radius;

        public GoalNearby(int x, int y, int radius)
        {
            X = x;
            Y = y;
            Radius = radius;
        }
    }

    /// <summary>
    /// A position the bot should stand next to, meant for chests and items like it.
    /// </summary>
    public class GetToTile : Goal
    {
        public int Radius = 1;
        
        public GetToTile(int x,int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// A goal that moves meant for npcs, may change in the future
    /// </summary>
    public class GoalDynamic : Goal
    {
        public Character character;
        public int Radius;

        public GoalDynamic(Character _character, int range)
        {
            character = _character;
            Radius = range;
        }
    }
}