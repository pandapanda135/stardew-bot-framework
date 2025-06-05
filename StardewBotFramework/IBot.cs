using StardewPathfinding.Pathfinding;
using StardewValley;

namespace StardewBotFramework;

public interface IBot
{ 
    /// <summary>
    /// Algorithms should be added in the order they will be used in.
    /// </summary>
    public List<AlgorithmBase> PathfindingAlgorithm { get; set; }

    /// <summary>
    /// This will contain all the goals for the pathfinding to find.
    /// </summary>
    public Stack<PathNode> Goal {get; set;}

    /// <summary>
    /// This will path-find to a single goal, this will run if there is one goal in the stack.
    /// </summary>
    /// <param name="current">The current location of the player as a <see cref="PathNode"/>.</param>
    /// <param name="location">The current location of the player as a <see cref="GameLocation"/>.</param>
    /// <param name="character">The <see cref="Character"/> you want to move.</param>
    /// <returns>this will return true if it can path-find to the goal.</returns>
    public bool GetSingleGoal(PathNode current, GameLocation location, Character character);
    
    /// <summary>
    /// This will path-find to multiple goals, this will run if there is more than one goal in the stack.
    /// </summary>
    /// <param name="current">The current location of the player as a <see cref="PathNode"/>.</param>
    /// <param name="location">The current location of the player as a <see cref="GameLocation"/>.</param>
    /// <param name="character">The <see cref="Character"/> you want to move.</param>
    /// <returns>This will return a <see cref="Stack{T}"/> of <see cref="bool"/> in the order of <see cref="Goal"/>.</returns>
    public Stack<bool> GetMultipleGoal(PathNode current, GameLocation location, Character character);
}
