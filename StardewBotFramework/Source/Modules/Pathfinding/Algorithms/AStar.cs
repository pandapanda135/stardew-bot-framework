using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding.Algorithms;

public class AStar : AlgorithmBase
{
    public class Pathing : IPathing
    {
        #region Pathfinding
        
        async Task<Stack<PathNode>> IPathing.FindPath(PathNode startPoint, Goal goal, GameLocation location,
            Character character, int limit)
        {
            Stack<PathNode> correctPath = await Task.Run(() => RunAStar(startPoint, goal, location, character, limit));
            
            if (correctPath.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<PathNode>();
            }
            
            ClearVariables();

            return correctPath;
        }

        private Stack<PathNode> RunAStar(PathNode startPoint, Goal goal, GameLocation location,
            Character character,int limit)
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
            IPathing.PriorityFrontier = new();
            IPathing.PriorityFrontier.Enqueue(startNode, 0);
            IPathing.Base.ClosedList.Add(startNode);
            
            int increase = 0;

            // check if goal is blocked before pathfinding
            if (IPathing.collisionMap.IsBlocked(goal.X, goal.Y))
            {
                Logger.Info($"goal is not an available tile");
                return new Stack<PathNode>();
            }
            
            while (!IPathing.PriorityFrontier.IsEmpty())
            {
                if (increase > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }

                PathNode current = IPathing.PriorityFrontier.Dequeue();

                if (!IPathing.NodeChecks(current,startNode,goal, location)) continue;
                
                IPathing.Base.ClosedList.Add(current);
                
                if (IPathing.Base.PathToEndPoint.Contains(current) && goal.IsEnd(current))
                {
                    Logger.Info($"breaking as current is equal to goal");
                    break; // this is here as cant return in NodeChecks. This checks if this is goal
                }
                
                Logger.Info($"this is current: {current}");
                // Neighbour search
                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(current).Result;
                foreach (var next in neighbours.Where(node => !IPathing.Base.ClosedList.Contains(node) && !IPathing.collisionMap.IsBlocked(node.X,node.Y)))
                {
                    int newCost = current.Cost + Graph.Cost(current, next);
                    if (!IPathing.PriorityFrontier.Contains(next) || newCost < next.Cost)
                    {
                        next.Cost = newCost;
                        int priority = newCost + PathNode.ManhattanHeuristic(new Vector2(next.X, next.Y),goal.VectorLocation.ToVector2());
                        Logger.Info($"A Star estimated heuristic {priority}");
                        IPathing.PriorityFrontier.Enqueue(next, priority);
                        IPathing.Base.PathToEndPoint.Push(next);
                    }
                }

                increase++;
            }
            
            Logger.Info($"Uniform about to return");
            return IPathing.RebuildPath(startNode, goal, IPathing.Base.PathToEndPoint);
        }
        
        #endregion
        private void ClearVariables()
        {
            IPathing.PendingCollisionChecks.Clear();
            IPathing.Frontier = new();
            IPathing.Base.ClosedList.Clear();
            IPathing.Base.PathToEndPoint.Clear();
        }
    }
}
