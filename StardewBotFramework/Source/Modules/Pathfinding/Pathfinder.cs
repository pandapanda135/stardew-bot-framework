using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Pathfinding;
using PathNode = StardewBotFramework.Source.Modules.Pathfinding.Base.PathNode;

namespace StardewBotFramework.Source.Modules.Pathfinding;

/// <summary>
/// A wrapper for pathfinding.
/// </summary>
public class Pathfinder
{
    private static readonly AlgorithmBase.IPathing Pathing = new AStar.Pathing();
    /// <summary>
    /// The bot will go to the goal. This should be awaited as finding the path is asynchronous. However the character moving is not
    /// </summary>
    /// <param name="goal"><see cref="Goal"/></param>
    /// <param name="canDestroy">Will destroy objects to get to the goal</param>
    /// <param name="dynamic">If that goal will change position this in the future will be used for npcs.</param>
    public async Task Goto(Goal goal, bool dynamic, bool canDestroy = false)
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        
        pathing.BuildCollisionMap(Game1.currentLocation);
        
        PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);
        
        Stack<PathNode> path = await pathing.FindPath(start,goal,Game1.currentLocation,10000,canDestroy);
        
        CharacterController.StartMoveCharacter(path,Game1.currentGameTime);

        while (CharacterController.IsMoving()) continue; // slightly jank way to get around MovingCharacter not being async
    }

    /// <summary>
    /// Will return the path to the goal.
    /// </summary>
    /// <param name="goal">The end point you want to path-find to</param>
    /// <param name="canDestroy">Will destroy objects to get to the goal</param>
    /// <param name="distance">the amount of times the pathfinder will run, this should be though about as each tile will increase this by one</param>
    /// <returns>A <see cref="Stack{T}"/> of <see cref="PathNode"/>, the end point will be first in dequeue and the start will be last</returns>
    public async Task<Stack<PathNode>> GetPathTo(Goal goal, int distance,bool canDestroy = false)
    {
        Pathing.BuildCollisionMap(Game1.currentLocation);

        PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);

        return await Pathing.FindPath(start, goal, Game1.currentLocation, distance,canDestroy);
    }

    public bool IsBlocked(int x, int y)
    {
        return AlgorithmBase.IPathing.collisionMap.IsBlocked(x,y);
    }

    public void BuildCollisionMap()
    {
        Pathing.BuildCollisionMap(BotBase.CurrentLocation);
    }
}