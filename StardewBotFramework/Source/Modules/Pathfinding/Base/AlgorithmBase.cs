using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using StardewBotFramework.Debug;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace StardewBotFramework.Source.Modules.Pathfinding.Base;

public class AlgorithmBase
{
    public Character? Character;

    public Stack<PathNode> PathToEndPoint = new();

    private List<PathNode> _goals;
    
    public GameLocation? CurrentLocation;
    
    // will be used to see if destroying stuff like trees is allowed in pathfinding
    public bool AllowDestruction;
    
    /// <summary>
    /// this contains Nodes the pathfinding has already reached
    /// </summary>
    public HashSet<PathNode> ClosedList = new();

    public AlgorithmBase(Character character,GameLocation currentLocation,List<PathNode> goals)
    {
        Character = character;
        CurrentLocation = currentLocation;
        _goals = goals;
    }

    public AlgorithmBase()
    {}
    
    // ,GameLocation location, Character character
    
    // this is so multiple types of pathing can be implemented more easily
    public interface IPathing
    {
        protected static PathQueue Frontier = new();

        protected static PathPriorityQueue PriorityFrontier = new();

        protected static Stack<PathNode> TemporaryStack = new();

        public static ConcurrentQueue<CollisionMap> PendingCollisionChecks = new();

        protected static readonly AlgorithmBase Base = new();

        protected static readonly Graph Graph = new();

        protected static CollisionMap collisionMap = new();

        public Task<Stack<PathNode>> FindPath(PathNode startPoint, Goal goal, GameLocation location,
            Character character, int limit);

        public static Stack<PathNode> RebuildPath(PathNode startPoint, Goal goal, Stack<PathNode> path)
        {
            if (!path.TryPeek(out var pathEndPoint) || pathEndPoint.VectorLocation != goal.VectorLocation)
            {
                if (pathEndPoint is null)
                {
                    Logger.Error($"Ending Rebuild path early end path: {path.Count} goal: {goal.VectorLocation}");
                    return new Stack<PathNode>();
                }
                
                return new Stack<PathNode>();
            }

            PathNode current = path.Pop();

            Stack<PathNode> correctPath = new();

            Logger.Info($"starting while loop in rebuildPath");
            while (!current.Equals(startPoint))
            {
                correctPath.Push(current);
                if (current.Parent is not null)
                {
                    Logger.Info($"{current.VectorLocation} checking parent");
                    current = current.Parent!;
                    continue;
                }

                break;
            }

            Logger.Info($"Ending RebuildPath");
            return correctPath;
        }

        public static bool NodeChecks(PathNode currentNode, PathNode startNode, Goal goal,
            GameLocation location)
        {
            Logger.Info($"Current tile {currentNode.X},{currentNode.Y}");

            if (goal.IsEnd(currentNode)) // put GoalReached here (I think)
            {
                Logger.Info($"Ending using CheckIfEnd function");
                // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths

                Base.PathToEndPoint.Push(currentNode);
                return true;
            }

            // We reduce by 1 to avoid pathfinding going along the side of the map
            if (currentNode.X > location.Map.DisplayWidth / Game1.tileSize - 1 ||
                currentNode.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 ||
                currentNode.X < 0 || currentNode.Y < 0)
            {
                Logger.Info(
                    $"Blocking this tile: {currentNode.X},{currentNode.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                return false;
            }

            // check if node == current and node is not start if none return true else false
            return !Equals(
                Base.ClosedList.Where(node => node.X == currentNode.X && node.Y == currentNode.Y && !node.Equals(startNode)),
                ImmutableList<PathNode>.Empty);
        }

        /// <summary>
        /// Build precomputed collision map this should be run before any type of pathfinding due to the dynamic nature of the world
        /// </summary>
        /// <param name="location">the <see cref="GameLocation"/> you want the collision map of</param>
        /// <param name="maxX">Max X of the current location, you should only use if you are looking for a subset of a location</param>
        /// <param name="maxY">Max Y of the current location, you should only use if you are looking for a subset of a location</param>
        /// <param name="minX">Minimum X of the current location, this should never be below 0</param>
        /// <param name="minY">Minimum Y of the current location, this should never be below 0</param>
        /// <returns>A <see cref="CollisionMap"/> however this is only useful if you want to make changes otherwise you should use IPathing.collisionMap</returns>
        CollisionMap BuildCollisionMap(GameLocation location,int maxX = 0, int maxY = 0, int minX = 0,
            int minY = 0)
        {
            // remove one as to not go around map
            maxX = location.Map.DisplayWidth / Game1.tileSize - 1;
            maxY = location.Map.DisplayHeight / Game1.tileSize - 1; 

            Logger.Error($"size of map {maxX},{maxY}");
            var map = new CollisionMap();

            for (int x = minX; x <= maxX; x++)
            {
                for (int y = minY; y <= maxY; y++)
                {
                    Rectangle rect = new Rectangle(x * 64, y * 64, 64, 64);
                    if (!Game1.currentLocation.isCollidingPosition(rect, Game1.viewport, true, 0, false, Game1.player))
                        continue;
                    Logger.Error($"adding {x},{y} to blockedTiles");
                    map._blockedTiles.Add((x,y));
                }
            }

            collisionMap = map;
            return map;
        }
    }
}