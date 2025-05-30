using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.Graphs;
using StardewValley;

namespace StardewPathfinding.Pathfinding.UniformCost;

public class UniformCostSearch : AlgorithmBase
{
    
    public class Pathing : IPathing
    {

        static AlgorithmBase _base = new AlgorithmBase();

        private static Graph _graph = new Graph();

        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation currentLocation,
            Character player, int limit)
        {
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
            IPathing._priorityFrontier.Enqueue(startNode, 0);

            int increase = 0;

            _base.ClosedList.Add(startNode);

            bool alreadyExists = false;

            while (!IPathing._priorityFrontier.IsEmpty())
            {
                alreadyExists = false;

                if (increase > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }

                PathNode current = IPathing._priorityFrontier.Dequeue();

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
                    Vector2 currentVector = new Vector2(current.X, current.Y);
                    Vector2 nextVector = new Vector2(node.X, node.Y);
                    if (currentVector == nextVector && startNode != current) alreadyExists = true;
                }

                if (alreadyExists) continue;

                _base.ClosedList.Add(current);

                foreach (var next in _graph.Neighbours(current).Where(node => !_base.ClosedList.Contains(node)))
                {
                    int newCost = current.Cost + Graph.Cost(current, next);
                    Logger.Info($"Current cost  {current.Cost}   next cost   {next.Cost}   new cost   {newCost}");
                    if (!IPathing._priorityFrontier.Contains(next) || newCost < next.Cost)
                    {
                        Logger.Info($"adding {next} to endpath");
                        next.Cost = newCost;
                        int priority = newCost;
                        IPathing._priorityFrontier.Enqueue(next, priority);
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

        public Stack<PathNode> RebuildPath(PathNode startPoint, PathNode endPoint, Stack<PathNode> path)
        {
            PathNode current = endPoint;

            Stack<PathNode> correctPath = new();

            while (current != startPoint)
            {
                Logger.Info($"new current  {current.id}");
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

        private void ClearVariables()
        {
            IPathing._priorityFrontier.Clear();
            _base.ClosedList!.Clear();
            _base.PathToEndPoint.Clear();
            DrawFoundTiles.debugDirectionTiles.Clear();
        }
    }
}
