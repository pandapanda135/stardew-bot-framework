using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.Graphs;
using StardewValley;

namespace StardewPathfinding.Pathfinding.GreedyBest;

public class GreedyBestFirstSearch : AlgorithmBase
{
    public class Pathing : IPathing
    {
        private static AlgorithmBase _base = new AlgorithmBase();

        private static Graph _graph = new Graph();

        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation location,
            Character player, int limit)
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
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
                foreach (var node in _base.ClosedList)
                {
                    if (current.X == node.X && current.Y == node.Y && startNode != current) alreadyExists = true;
                    if (alreadyExists) break;
                }
                
                if (alreadyExists) continue;

                _base.ClosedList.Add(current);

                foreach (var next in _graph.Neighbours(current).Where(node => !_base.ClosedList.Contains(node)))
                {
                    int priority = PathNode.ManhattanHeuristic(new Vector2(current.X, current.Y),
                        new Vector2(next.X, next.Y));
                    IPathing.PriorityFrontier.Enqueue(next,priority);
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