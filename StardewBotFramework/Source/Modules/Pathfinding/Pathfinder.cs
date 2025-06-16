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
    /// The bot will go to the goal. This should be awaited as it is asynchronous.
    /// </summary>
    /// <param name="goal"><see cref="Goal"/></param>
    /// <param name="dynamic">If that goal will change position this in the future will be used for npcs.</param>
    public async Task Goto(Goal goal, bool dynamic)
    {
        AlgorithmBase.IPathing pathfinder = new BreadthFirstSearch.Pathing();

        pathfinder.BuildCollisionMap(Game1.currentLocation);
        
        PathNode start = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.Y, null);
        
        await pathfinder.FindPath(start,goal,Game1.currentLocation,Game1.player,100000);
    }
    
    
}