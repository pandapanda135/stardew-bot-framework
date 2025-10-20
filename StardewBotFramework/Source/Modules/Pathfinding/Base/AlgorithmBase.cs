using StardewBotFramework.Debug;
using StardewBotFramework.Source.Utilities;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class AlgorithmBase
{
    // this is so multiple types of pathing can be implemented more easily
    public interface IPathing
    {
        protected static PathQueue Frontier = new();

        protected static PathPriorityQueue PriorityFrontier = new();
        
        /// <summary>
        /// This is used to keep track of the final node as we get the path from traversing the node's parent
        /// </summary>
        public static PathNode? EndNode;
        
        /// <summary>
        /// this contains Nodes the pathfinding has already reached
        /// </summary>
        public static readonly HashSet<PathNode> ClosedList = new();

        protected static readonly Graph Graph = new();

        public static CollisionMap CollisionMap = new();

        public Task<Stack<PathNode>> FindPath(PathNode startPoint, Goal goal, GameLocation location,
            int limit,bool canDestroy = false);

        public static Stack<PathNode> RebuildPath(PathNode startPoint, Goal goal,PathNode? finalNode)
        {
            if (finalNode is null || finalNode.VectorLocation != goal.VectorLocation && goal is Goal.GoalPosition)
            {
                Logger.Error($"Ending Rebuild path early, goal: {goal.VectorLocation}");
                return new Stack<PathNode>();
            }

            PathNode current = finalNode;
            Stack<PathNode> correctPath = new();

            Logger.Info($"starting while loop in rebuildPath");
            while (!current.Equals(startPoint))
            {
                correctPath.Push(current);
                if (current.Parent is null) break;
                
                Logger.Info($"{current.VectorLocation} checking parent {current.Parent.VectorLocation}");
                current = current.Parent;
            }
            
            Logger.Info($"Ending RebuildPath");
            return correctPath;
        }

        /// <summary>
        /// This will do checks to the node to check for conditions these include: if it is larger than map and if it is present in the ClosedList 
        /// </summary>
        /// <returns>will return true for if it is not in ClosedList yet else will return false for if it is larger than map or if already in ClosedList</returns>
        public static bool NodeChecks(PathNode currentNode,
            GameLocation location)
        {
            Logger.Info($"Current tile {currentNode.X},{currentNode.Y}");
            
            // look into if removing 1 from the greater than check is needed anywhere as I don't think it is right now
            if (currentNode.X > (location.Map.DisplayWidth / Game1.tileSize) ||
                currentNode.Y > (location.Map.DisplayHeight / Game1.tileSize) ||
                currentNode.X < -1 || currentNode.Y < -1)
            {
                Logger.Info($"Blocking this tile due to off map: {currentNode.X},{currentNode.Y}");
                return false;
            }

            // We don't check if it is in closed list as they should never be added to the neighbour foreach
            return true;
        }

        /// <summary>
        /// Check if is at end, shortcut for all goal type's is end function
        /// </summary>
        /// <returns>Will return true if at end or in within radius else false</returns>
        public static bool CanEnd(PathNode currentNode,Goal goal,bool cardinal = true)
        {
            return goal.CanEnd(currentNode,cardinal);
        }

        /// <summary>
        /// Build collision map this should be run before any type of pathfinding due to the dynamic nature of the world.
        /// </summary>
        /// <param name="location">the <see cref="GameLocation"/> you want the collision map of</param>
        /// <param name="maxX">Max X of the current location, you should only use this if you are looking to update a subset of a location</param>
        /// <param name="maxY">Max Y of the current location, you should only use this if you are looking to update a subset of a location</param>
        /// <param name="minX">Minimum X of the current location, this should never be below 0</param>
        /// <param name="minY">Minimum Y of the current location, this should never be below 0</param>
        /// <returns>A <see cref="Pathfinding.CollisionMap"/> however this is only useful if you want to make changes otherwise you should use IPathing.collisionMap</returns>
        CollisionMap BuildCollisionMap(GameLocation location,int maxX = -1, int maxY = -1, int minX = 0,
            int minY = 0)
        {
            if (maxX == -1) maxX = TileUtilities.MaxX;
            if (maxY == -1) maxY = TileUtilities.MaxY;
            
            Logger.Error($"size of map {maxX},{maxY}");
            
            // TODO: The walls of caves just don't get added in character controller for some reason when this is ran after already being ran in that location
            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!CollisionMap.IsCurrentlyBlocked(location, x, y)) continue;
                    
                    CollisionMap.AddBlockedTile(x,y);
                }
            }

            return CollisionMap;
        }
    }
}