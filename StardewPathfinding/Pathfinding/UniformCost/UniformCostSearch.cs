using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.Graphs;
using StardewValley;

namespace StardewPathfinding.Pathfinding.UniformCost;

/// <summary>
/// This is an implementation of Uniform Cost Search (More specifically Dijkstra's algorithm).
/// This should be used when you want the even spread of Breadth First however with a cost on each tile.
/// (Also this algorithm is not well optimized so be careful with use)
/// </summary>
public class UniformCostSearch : AlgorithmBase
{
    
    public class Pathing : IPathing
    {
        private static AlgorithmBase _base = new AlgorithmBase();

        private static Graph _graph = new Graph();

        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation location,
            Character player, int limit) // TODO: There is some randomness in how the path is found / rebuilt. Also early exit a while if either the x or y are out of GameLocation height / width
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
            IPathing.PriorityFrontier = new();
            IPathing.PriorityFrontier.Enqueue(startNode, 0);
            _base.ClosedList.Add(startNode);
            
            int increase = 0;

            while (!IPathing.PriorityFrontier.IsEmpty())
            {
                bool alreadyExists = false;

                if (increase > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }

                PathNode current = IPathing.PriorityFrontier.Dequeue();
                
                if (current.X > location.Map.DisplayWidth / Game1.tileSize || current.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize || current.X < 0 || current.Y < 0)
                {
                    Logger.Info($"Blocking this tile: {current.X},{current.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                    continue;
                }

                Logger.Info($"Current tile {current.X},{current.Y}");

                if (_graph.CheckIfEnd(current, endPoint))
                {
                    Logger.Info($"Ending using CheckIfEnd function");
                    // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths

                    _base.PathToEndPoint.Push(current);
                    return _base.PathToEndPoint;
                }

                // next loop if current is already in ClosedList
                // this is dumb and can be improved
                foreach (var node in _base.ClosedList)
                {
                    if (current.X == node.X && current.Y == node.Y && startNode != current) alreadyExists = true;
                    if (alreadyExists) break;
                }
                
                if (alreadyExists) continue;
                
                _base.ClosedList.Add(current);
                foreach (var next in _graph.Neighbours(current).Where(node => !_base.ClosedList.Contains(node)))
                {
                    int newCost = current.Cost + Graph.Cost(current, next);
                    Logger.Info($"Current cost  {current.Cost}   next cost   {next.Cost}   new cost   {newCost}");
                    if (!IPathing.PriorityFrontier.Contains(next) ||newCost < next.Cost)
                    {
                        Logger.Info($"adding {next} to endpath");
                        next.Cost = newCost;
                        int priority = newCost;
                        IPathing.PriorityFrontier.Enqueue(next, priority);
                        _base.PathToEndPoint.Push(next);
                    }
                }

                increase++;
            }

            if (_base.PathToEndPoint.Count > 0)
            {
                foreach (var pathNode in _base.PathToEndPoint)
                {
                    Logger.Info($"node in end point path   {pathNode.X}   {pathNode.Y}");
                }
            }

            Logger.Info($"Uniform about to return");
            return _base.PathToEndPoint;
        }

        private void ClearVariables()
        {
            IPathing.PriorityFrontier = new();
            _base.ClosedList!.Clear();
            _base.PathToEndPoint.Clear();
            DrawFoundTiles.debugDirectionTiles.Clear();
        }
    }
}
