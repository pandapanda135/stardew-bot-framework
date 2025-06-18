using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding;

/// <summary>
/// A wrapper for pathfinding.
/// </summary>
public class Pathfinder
{
    /// <summary>
    /// The bot will go to the goal. This should be awaited as finding the path is asynchronous. However the character moving is not
    /// </summary>
    /// <param name="goal"><see cref="Goal"/></param>
    /// <param name="dynamic">If that goal will change position this in the future will be used for npcs.</param>
    public async Task Goto(Goal goal, bool dynamic)
    {
        AlgorithmBase.IPathing pathfinder = new AStar.Pathing();

        pathfinder.BuildCollisionMap(Game1.currentLocation);
        
        PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);
        
        Stack<PathNode> path = await pathfinder.FindPath(start,goal,Game1.currentLocation,Game1.player,100000);
        
        CharacterController.StartMoveCharacter(path, Game1.player, Game1.currentLocation,
            Game1.currentGameTime);

        while (IsMoving()) continue; // slightly jank way to get around MovingCharacter not being async
    }

    /// <summary>
    /// Will return the path to the goal.
    /// </summary>
    /// <param name="goal">The end point you want to path-find to</param>
    /// <param name="distance">the amount of times the pathfinder will run, this should be though about as each tile will increase this by one</param>
    /// <returns>A <see cref="Stack{T}"/> of <see cref="PathNode"/>, the end point will be first in dequeue and the start will be last</returns>
    public async Task<Stack<PathNode>> GetPathTo(Goal goal, int distance)
    {
        AlgorithmBase.IPathing pathfinder = new AStar.Pathing();

        pathfinder.BuildCollisionMap(Game1.currentLocation);

        PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);

        return await pathfinder.FindPath(start, goal, Game1.currentLocation, Game1.player, distance);
    }

    private bool IsMoving()
    {
        return CharacterController.IsMoving();
    }
}