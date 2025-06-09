using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using StardewPathfinding.Debug;
using StardewPathfinding.Graphs;
using StardewValley;

namespace StardewPathfinding.Pathfinding.AStar;

public class AStarPathfinding : AlgorithmBase
{
    public class Pathing : IPathing
    {
        Stack<PathNode> AlgorithmBase.IPathing.FindPath(PathNode startPoint, PathNode endPoint, GameLocation location,
            Character character, int limit)
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
                
                if (IPathing.Base.PathToEndPoint.Contains(current) && current.VectorLocation == endPoint.VectorLocation) return IPathing.Base.PathToEndPoint; // this is here as cant return in NodeChecks
                
                Logger.Info($"this is current: {current}");
                
                IPathing.Base.ClosedList.Add(current);
                // Neighbour search
                foreach (var next in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                {
                    int newCost = current.Cost + Graph.Cost(current, next);
                    if (!IPathing.PriorityFrontier.Contains(next) || newCost < next.Cost)
                    {
                        next.Cost = newCost;
                        int priority = newCost + PathNode.ManhattanHeuristic(new Vector2(next.X, next.Y),endPoint.VectorLocation);
                        Logger.Info($"A Star estimated heuristic {priority}");
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

        public Stack<PathNode> FindMultipleGoals(PathNode startNode, List<PathNode> goals, GameLocation location, Character character, int limit)
        {
            throw new NotImplementedException();
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