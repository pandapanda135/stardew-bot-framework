using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.Monsters;
using PathNode = StardewBotFramework.Source.Modules.Pathfinding.Base.PathNode;

namespace StardewBotFramework.Source.Modules.Pathfinding;

/// <summary>
/// A wrapper for pathfinding.
/// </summary>
public class Pathfinder
{
    /// <summary>
    /// The bot will go to the goal. This should be awaited as finding the path is asynchronous. However, the character moving is not.
    /// </summary>
    /// <param name="goal"><see cref="Goal"/></param>
    /// <param name="canDestroy">Will destroy objects to get to the goal</param>
    /// <param name="buildCollision">Build collision map.</param>
    public async Task Goto(Goal goal, bool canDestroy = false, bool buildCollision = true)
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();

        if (buildCollision) pathing.BuildCollisionMap(BotBase.CurrentLocation);
        
        PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);

        Stack<PathNode> path = await pathing.FindPath(start, goal, BotBase.CurrentLocation, 10000, canDestroy);

        Character? npc = null;
        if (goal is Goal.GoalDynamic dynamic) npc = dynamic.Character;

        var controller = new CharacterController(new(), BotBase.Farmer, BotBase.CurrentLocation);
        controller.StartMoveCharacter(path,npc);

        // slightly jank way to get around MovingCharacter not being async
        while (CharacterController.IsMoving())
        {
        }
    }

    /// <summary>
    /// Attack the monster sent with <see cref="Goal.GoalDynamic"/>.
    /// </summary>
    /// <param name="goal">Should be <see cref="Goal.GoalDynamic"/> with a monster being provided</param>
    /// <param name="canDestroy">can destroy if needed</param>
    /// <param name="buildCollision">Build collision map before pathfinding</param>
    public async Task AttackMonster(Goal goal, bool canDestroy = false, bool buildCollision = true)
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        if (buildCollision) pathing.BuildCollisionMap(BotBase.CurrentLocation);
        
        PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);

        Stack<PathNode> path = await pathing.FindPath(start, goal, BotBase.CurrentLocation, 10000, canDestroy);

        Character? npc = null;
        if (goal is Goal.GoalDynamic dynamic) npc = dynamic.Character; 
        var controller = new CharacterController(new(), BotBase.Farmer, BotBase.CurrentLocation);
        controller.StartMoveCharacter(path,npc as Monster,true);

        while (CharacterController.IsMoving())
        {
        }
    }

    /// <summary>
    /// Will return the path to the goal.
    /// </summary>
    /// <param name="goal">The end point you want to path-find to</param>
    /// <param name="canDestroy">Will destroy objects to get to the goal</param>
    /// <param name="distance">the amount of times the pathfinder will run, this should be though about as each tile will increase this by one</param>
    /// <param name="buildCollision">Build collision map.</param>
    /// <returns>A <see cref="Stack{T}"/> of <see cref="PathNode"/>, the end point will be first in dequeue and the start will be last</returns>
    public async Task<Stack<PathNode>> GetPathTo(Goal goal, int distance,bool canDestroy = false, bool buildCollision = true)
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        if (buildCollision) pathing.BuildCollisionMap(Game1.currentLocation);

        PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);

        Stack<PathNode> path = await pathing.FindPath(start, goal, Game1.currentLocation, distance, canDestroy);
        
        return path;
    }

    /// <summary>
    /// Query if a tile is blocked according to the collision map
    /// </summary>
    public bool IsBlocked(int x, int y)
    {
        return AlgorithmBase.IPathing.CollisionMap.IsBlocked(x,y);
    }
    
   
    private static readonly AlgorithmBase.IPathing Pathing = new AStar.Pathing();
    public void BuildCollisionMap()
    {
        Pathing.BuildCollisionMap(BotBase.CurrentLocation);
    }

    public void BuildCollisionMapInRadius(Point startTile, int radius)
    {
        Pathing.BuildCollisionMap(BotBase.CurrentLocation,
            startTile.X + radius, startTile.Y + radius,
            startTile.X - radius, startTile.Y - radius);
    }
}