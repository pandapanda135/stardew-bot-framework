using StardewPathfinding.Pathfinding;
using StardewValley;

namespace StardewBotFramework;

public interface IBot
{ 
    /// <summary>
    /// Algorithms should be added in the order they will be used in.
    /// </summary>
    public AlgorithmBase.IPathing PathfindingAlgorithm { get; set; }

    /// <summary>
    /// This will contain all the goals for the pathfinding to find.
    /// </summary>
    public Stack<PathNode> Goals {get; set;}
    
    enum Actions // these actions are not 100% finalised and most likely will be changed in the future
    {
        Movement,
        Tool,
        Weapon,
    }
    #region Pathfinding
        /// <summary>
        /// this should be used to define where you want the bot to go. 
        /// </summary>
        /// <param name="location">The current location of the player as a <see cref="GameLocation"/>.</param>
        /// <param name="character">The <see cref="Character"/> you want to move.</param>
        /// <param name="actions">The end action you want the bot to do. You can only use one per goal</param>
        /// <returns></returns>
        public void SpecifyLocations(GameLocation location, Character character, Queue<Actions> actions);

        /// <summary>
        /// This will path-find to a single goal, this will run if there is one goal in the stack.
        /// </summary>
        /// <param name="current">The current location of the player as a <see cref="PathNode"/>.</param>
        /// <param name="location">The current location of the player as a <see cref="GameLocation"/>.</param>
        /// <param name="character">The <see cref="Character"/> you want to move.</param>
        /// <param name="action">The end action you want the bot to do.</param>
        /// <returns>this will return true if it can path-find to the goal.</returns>
        protected Stack<PathNode> GetSingleGoal(PathNode current, GameLocation location, Character character, Actions action);
        
        /// <summary>
        /// This will path-find to multiple goals, this will run if there is more than one goal in the stack.
        /// </summary>
        /// <param name="current">The current location of the player as a <see cref="PathNode"/>.</param>
        /// <param name="location">The current location of the player as a <see cref="GameLocation"/>.</param>
        /// <param name="character">The <see cref="Character"/> you want to move.</param>
        /// <param name="actions">The end action you want the bot to do this will happen in the order of the Queue .</param>
        /// <returns>This will return a <see cref="Stack{T}"/> of <see cref="bool"/> in the order of <see cref="Goals"/>.</returns>
        protected Stack<bool> GetMultipleGoal(PathNode current, GameLocation location, Character character,Queue<Actions> actions);
    #endregion
}
