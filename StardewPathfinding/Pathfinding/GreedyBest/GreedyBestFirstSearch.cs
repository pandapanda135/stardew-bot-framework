using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewValley;

namespace StardewPathfinding.Pathfinding.GreedyBest;

public class GreedyBestFirstSearch : AlgorithmBase
{
    public class Pathing : IPathing
    {
        public Stack<PathNode> FindPath(PathNode startPoint, PathNode endPoint, GameLocation location,
            Character player, int limit)
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
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
                
                if (IPathing.Base.PathToEndPoint.Contains(current)) return IPathing.Base.PathToEndPoint; // this is here as cant return in NodeChecks
                
                IPathing.Base.ClosedList.Add(current);

                foreach (var next in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                {
                    int priority = PathNode.ManhattanHeuristic(new Vector2(next.X, next.Y),endPoint.VectorLocation);
                    Logger.Info($"heuristic: {priority}");
                    IPathing.PriorityFrontier.Enqueue(next,priority);
                }

                increase++;
            }

            IPathing.EndDebugging();
            
            Logger.Info($"Greedy about to return");
            return IPathing.Base.PathToEndPoint;
        }

        private void ClearVariables()
        {
            IPathing.PriorityFrontier.Clear();
            IPathing.Base.ClosedList.Clear();
            IPathing.Base.PathToEndPoint.Clear();
            DrawFoundTiles.debugDirectionTiles.Clear();
        }
    }
}