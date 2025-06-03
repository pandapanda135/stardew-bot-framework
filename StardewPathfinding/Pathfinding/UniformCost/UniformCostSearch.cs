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
        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation location,
            Character player, int limit) // TODO: There is some randomness in how the path is found / rebuilt. I think the randomness in the pathing is due to the cost of each tile being randomised
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
            IPathing.PriorityFrontier = new();
            IPathing.PriorityFrontier.Enqueue(startNode, 0);
            IPathing.Base.ClosedList.Add(startNode);
            
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

                if (IPathing.Graph.CheckIfEnd(current, endPoint))
                {
                    Logger.Info($"Ending using CheckIfEnd function");
                    // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths

                    IPathing.Base.PathToEndPoint.Push(current);
                    return IPathing.Base.PathToEndPoint;
                }

                // next loop if current is already in ClosedList
                foreach (PathNode node in IPathing.Base.ClosedList)
                {
                    if (current.X == node.X && current.Y == node.Y && startNode != current) alreadyExists = true;
                    if (alreadyExists) break;
                }
                
                if (alreadyExists) continue;
                
                IPathing.Base.ClosedList.Add(current);
                // Neighbour search
                foreach (var next in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                {
                    int newCost = current.Cost + Graph.Cost(current, next);
                    Logger.Info($"Current cost  {current.Cost}   next cost   {next.Cost}   new cost   {newCost}");
                    if (!IPathing.PriorityFrontier.Contains(next) || newCost < next.Cost)
                    {
                        Logger.Info($"adding {next} to endpath");
                        next.Cost = newCost;
                        int priority = newCost;
                        IPathing.PriorityFrontier.Enqueue(next, priority);
                        IPathing.Base.PathToEndPoint.Push(next);
                    }
                }

                increase++;
            }

            if (IPathing.Base.PathToEndPoint.Count > 0)
            {
                foreach (var pathNode in IPathing.Base.PathToEndPoint)
                {
                    Logger.Info($"node in end point path   {pathNode.X}   {pathNode.Y}");
                }
            }

            Logger.Info($"Uniform about to return");
            return IPathing.Base.PathToEndPoint;
        }

        private void ClearVariables()
        {
            IPathing.PriorityFrontier = new();
            IPathing.Base.ClosedList!.Clear();
            IPathing.Base.PathToEndPoint.Clear();
            DrawFoundTiles.debugDirectionTiles.Clear();
        }
    }
}
