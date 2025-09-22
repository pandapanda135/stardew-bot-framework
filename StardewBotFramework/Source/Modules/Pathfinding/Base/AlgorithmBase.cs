using System.Collections.Immutable;
using StardewBotFramework.Debug;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class AlgorithmBase
{
    public Character? Character;
    
    public GameLocation? CurrentLocation;
    
    // will be used to see if destroying stuff like trees is allowed in pathfinding
    public bool AllowDestruction;

    public AlgorithmBase(Character character,GameLocation currentLocation)
    {
        Character = character;
        CurrentLocation = currentLocation;
    }

    public AlgorithmBase()
    {}
    
    // this is so multiple types of pathing can be implemented more easily
    public interface IPathing
    {
        protected static PathQueue Frontier = new();

        protected static PathPriorityQueue PriorityFrontier = new();
        
        protected static readonly AlgorithmBase Base = new();

        public static readonly Stack<PathNode> PathToEndPoint = new();
        
        /// <summary>
        /// this contains Nodes the pathfinding has already reached
        /// </summary>
        public static readonly HashSet<PathNode> ClosedList = new();

        protected static readonly Graph Graph = new();

        public static CollisionMap collisionMap = new();

        public Task<Stack<PathNode>> FindPath(PathNode startPoint, Goal goal, GameLocation location,
            int limit,bool canDestroy = false);

        public static Stack<PathNode> RebuildPath(PathNode startPoint, Goal goal, Stack<PathNode> path)
        {
            if (!path.TryPeek(out var pathEndPoint) || pathEndPoint.VectorLocation != goal.VectorLocation && goal is Goal.GoalPosition)
            {
                Logger.Error($"Ending Rebuild path early, end path: {path.Count} goal: {goal.VectorLocation}");
                return new Stack<PathNode>();
            }

            PathNode current = path.Pop();
            Stack<PathNode> correctPath = new();

            Logger.Info($"starting while loop in rebuildPath");
            while (!current.Equals(startPoint))
            {
                correctPath.Push(current);
                if (current.Parent is null) break;
                
                Logger.Info($"{current.VectorLocation} checking parent {current.Parent.VectorLocation}");
                current = current.Parent!;
            }

            Logger.Info($"Ending RebuildPath");
            return correctPath;
        }

        /// <summary>
        /// This will do checks to the node to check for conditions these include: if it is at the end, if it is larger than map 
        /// </summary>
        /// <returns>will return true for either is end or if it is not in ClosedList yet else will return false for if it is larger than map or if already in ClosedList</returns>
        public static bool NodeChecks(PathNode currentNode, PathNode startNode, Goal goal,
            GameLocation location)
        {
            Logger.Info($"Current tile {currentNode.X},{currentNode.Y}");

            if (goal.IsEnd(currentNode) || IPathing.CanEnd(currentNode,goal)) // put GoalReached here (I think)
            {
                Logger.Info($"Ending using CheckIfEnd function");
                // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths

                PathToEndPoint.Push(currentNode);
                return true;
            }

            // We reduce by 1 to avoid pathfinding going along the side of the map
            if (currentNode.X > location.Map.DisplayWidth / Game1.tileSize - 1 ||
                currentNode.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 ||
                currentNode.X < -1 || currentNode.Y < -1)
            {
                Logger.Info(
                    $"Blocking this tile: {currentNode.X},{currentNode.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                return false;
            }

            // check if node == current and node is not start if none return true else false
            return !Equals(
                ClosedList.Where(node => node.X == currentNode.X && node.Y == currentNode.Y && !node.Equals(startNode)),
                ImmutableList<PathNode>.Empty);
        }

        /// <summary>
        /// Check if is at end, shortcut for all goal type's is end function
        /// </summary>
        /// <returns>Will return true if at end or in within radius else false</returns>
        public static bool CanEnd(PathNode currentNode,Goal goal,bool cardinal = true)
        {
            switch (goal)
            {
                case Goal.GoalPosition:
                    if (goal.IsEnd(currentNode))
                    {
                        return true;
                    }
                    break;
                case Goal.GoalNearby goalNearby: //TODO: maybe make cardinal optional as it being false is better for movement but doesn't work for using items
                    if (goalNearby.IsInEndRadius(currentNode, goalNearby.Radius,cardinal))
                    {
                        return true;
                    }
                    break;
                case Goal.GoalDynamic goalDynamic:
                    if (goalDynamic.IsInEndRadius(currentNode, goalDynamic.Radius,cardinal))
                    {
                        return true;
                    }
                    break;
                case Goal.GetToTile getToTile:
                    if (getToTile.IsInEndRadius(currentNode, getToTile.Radius,cardinal))
                    {
                        return true;
                    }
                    break;
                default:
                    if (goal.IsEnd(currentNode))
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        /// <summary>
        /// Build collision map this should be run before any type of pathfinding due to the dynamic nature of the world.
        /// </summary>
        /// <param name="location">the <see cref="GameLocation"/> you want the collision map of</param>
        /// <param name="maxX">Max X of the current location, you should only use if you are looking for a subset of a location</param>
        /// <param name="maxY">Max Y of the current location, you should only use if you are looking for a subset of a location</param>
        /// <param name="minX">Minimum X of the current location, this should never be below 0</param>
        /// <param name="minY">Minimum Y of the current location, this should never be below 0</param>
        /// <returns>A <see cref="CollisionMap"/> however this is only useful if you want to make changes otherwise you should use IPathing.collisionMap</returns>
        CollisionMap BuildCollisionMap(GameLocation location,int maxX = -1, int maxY = -1, int minX = 0,
            int minY = 0)
        {
            if (maxX == -1) maxX = location.Map.DisplayWidth / Game1.tileSize;
            if (maxY == -1) maxY = location.Map.DisplayHeight / Game1.tileSize; 

            Logger.Error($"size of map {maxX},{maxY}");
            var map = new CollisionMap();

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    if (!CollisionMap.IsCurrentlyBlocked(x,y)) continue;
                    map.BlockedTiles.Add((x,y));
                }
            }

            collisionMap = map;
            return map;
        }
    }
}