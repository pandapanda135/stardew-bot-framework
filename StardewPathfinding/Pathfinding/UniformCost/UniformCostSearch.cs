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
                if (increase > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }

                PathNode current = IPathing.PriorityFrontier.Dequeue();

                if (!IPathing.NodeChecks(current,startNode,endPoint, location)) continue;
                
                if (IPathing.Base.PathToEndPoint.Contains(current) && current.VectorLocation == endPoint.ToVector2()) return IPathing.Base.PathToEndPoint; // this is here as cant return in NodeChecks
                
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

            IPathing.EndDebugging();

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
