using System.Collections.Immutable;
using StardewBotFramework.Debug;
using StardewValley;

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
        
        protected static readonly AlgorithmBase Base = new();

        protected static readonly Graph Graph = new();
        
        public Task FindPath(PathNode startPoint, Goal goal, GameLocation location,
            Character character, int limit);

        public static Stack<PathNode> RebuildPath(PathNode startPoint, Goal goal, Stack<PathNode> path)
        {
            if (!path.TryPeek(out var pathEndPoint) || pathEndPoint.VectorLocation != goal.VectorLocation)
            {
                Logger.Error($"Ending Rebuild path early end path: {path.Count}   goal: {goal.VectorLocation}");
                return new Stack<PathNode>();
            }

            PathNode current = path.Pop();

            Stack<PathNode> correctPath = new();

            while (current != startPoint)
            {
                correctPath.Push(current);
                if (current.Parent is not null)
                {
                    current = current.Parent!;
                    continue;
                }
                
                break;
            }

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
            if (currentNode.X > location.Map.DisplayWidth / Game1.tileSize - 1 || currentNode.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 || currentNode.X < 0 || currentNode.Y < 0)
            {
                Logger.Info($"Blocking this tile: {currentNode.X},{currentNode.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                return false;
            }

            // check if node == current and node is not start if none return true else false
            return !Equals(Base.ClosedList.Where(node => node.X == currentNode.X && node.Y == currentNode.Y && node != startNode), ImmutableList<PathNode>.Empty);
        }
        
    }
}