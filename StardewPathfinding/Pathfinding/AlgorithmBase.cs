using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.Graphs;
using StardewValley;
using StardewValley.Quests;
using xTile.Tiles;

namespace StardewPathfinding.Pathfinding;

public class AlgorithmBase
{
    public Character? Character;

    public Stack<PathNode> PathToEndPoint = new();

    public Stack<PathNode> MultipleEndPaths = new();

    public GameLocation? CurrentLocation;

    public List<PathNode>? Goals;

    // will be used to see if destroying stuff like trees is allowed in pathfinding
    public bool AllowDestruction;
    
    /// <summary>
    /// this contains Nodes the pathfinding has already reached
    /// </summary>
    public HashSet<PathNode> ClosedList = new HashSet<PathNode>();

    public AlgorithmBase(Character character,GameLocation currentLocation,List<PathNode> goals)
    {
        Character = character;
        CurrentLocation = currentLocation;
        Goals = goals;
    }

    public AlgorithmBase()
    {}
    
    // ,GameLocation location, Character character
    
    // this is so multiple types of pathing can be implemented more easily
    public interface IPathing
    {
        // public event EventHandler ReachedGoal; // TODO: Implement Later 
        
        protected static PathQueue Frontier = new();

        protected static PathPriorityQueue PriorityFrontier = new();
        
        protected static Stack<PathNode> TemporaryStack = new();
        
        protected static readonly AlgorithmBase Base = new();

        protected static readonly Graph Graph = new();
        
        public Stack<PathNode> FindPath(PathNode startPoint, PathNode endPoint, GameLocation location,
            Character character, int limit);

        public Stack<PathNode> FindMultipleGoals(PathNode startNode, List<PathNode> goals, GameLocation location,
            Character character, int limit);
        
        public Stack<PathNode> RebuildPath(PathNode startPoint, PathNode endPoint, Stack<PathNode> path)
        {
            if (!path.TryPeek(out var endPointPath) || endPointPath.VectorLocation != endPoint.VectorLocation)
            {
                Logger.Info("Ending Rebuild path early");
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

        public Stack<PathNode> RebuildMultiplePaths(PathNode startPoint, List<PathNode> goals, Stack<PathNode> path)
        {
            Stack<PathNode> correctPath = new();

            foreach (var goal in goals)
            {
                // if (!path.TryPeek(out var endPointPath) || endPointPath.VectorLocation != goal.VectorLocation)
                // {
                //     Logger.Info($"Ending Rebuild path early due to {endPointPath.VectorLocation} , {endPointPath.Parent}");
                //     return new Stack<PathNode>();
                // }

                PathNode current = path.Pop();
                
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
            }
            
            return correctPath;
        }
        
        public static bool NodeChecks(PathNode currentNode, PathNode startNode, PathNode endPoint,
            GameLocation location)
        {
            Logger.Info($"Current tile {currentNode.X},{currentNode.Y}");
            
            if (Graph.CheckIfEnd(currentNode, endPoint))
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
            
            bool alreadyExists = false;
            // next loop if current is already in ClosedList
            foreach (PathNode node in Base.ClosedList)
            {
                if (currentNode.X == node.X && currentNode.Y == node.Y && startNode != node) alreadyExists = true;
                if (alreadyExists) break;
            }
            
            if (alreadyExists) return false;

            return true;
        }

        public static bool MultipleEndNodeChecks(PathNode currentNode, PathNode startNode,
            GameLocation location)
        {
            foreach (var endNode in Base.Goals!)
            {
                if (Graph.CheckIfEnd(currentNode,endNode))
                {
                    Base.MultipleEndPaths.Push(currentNode);
                    Base.Goals.Remove(endNode);
                    return true;
                }
            }

            if (currentNode.X > location.Map.DisplayWidth / Game1.tileSize - 1 || currentNode.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 || currentNode.X < 0 || currentNode.Y < 0)
            {
                Logger.Info($"Blocking this tile: {currentNode.X},{currentNode.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                return false;
            }
            
            bool alreadyExists = false;
            // next loop if current is already in ClosedList
            foreach (PathNode node in Base.ClosedList)
            {
                if (currentNode.X == node.X && currentNode.Y == node.Y && startNode != node) alreadyExists = true;
                if (alreadyExists) break;
            }
            
            if (alreadyExists) return false;
            
            return true;
        }
        
        public static void EndDebugging()
        {
            if (Base.PathToEndPoint.Count > 0)
            {
                foreach (var pathNode in Base.PathToEndPoint)
                {
                    Logger.Info($"node in end point path   {pathNode.X}   {pathNode.Y}");
                }
            }
        }
    }
}