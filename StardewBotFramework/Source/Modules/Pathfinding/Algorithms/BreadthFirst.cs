using System.Collections;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.Objects;

namespace StardewBotFramework.Source.Modules.Pathfinding.Algorithms;

public class BreadthFirstSearch : AlgorithmBase
{
    public class Pathing : IPathing
    {
        #region PathFinding

        async Task<Stack<PathNode>> IPathing.FindPath(PathNode startPoint, Goal goal, GameLocation location, int limit, bool canDestroy)
        {
            Stack<PathNode> correctPath = await Task.Run(() => RunBreadthFirst(startPoint, goal, location, limit));
            
            if (correctPath.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<PathNode>();
            }
            
            ClearVariables();

            return correctPath;
        }

        private Stack<PathNode> RunBreadthFirst(PathNode startPoint, Goal goal, GameLocation location, int limit)
        {
            Logger.Info("started RunBreadthFirst");
            ClearVariables();

            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
            IPathing.Frontier = new();
            IPathing.Frontier.Enqueue(startNode);
            IPathing.ClosedList.Add(startNode);
            
            int increase = 0;
            
            // check if goal is blocked before pathfinding
            if (IPathing.collisionMap.IsBlocked(goal.X, goal.Y))
            {
                Logger.Info($"goal is not an available tile");
                return new Stack<PathNode>();
            }
            
            while (!IPathing.Frontier.IsEmpty())
            {
                if (increase > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }

                PathNode current = IPathing.Frontier.Dequeue(); // issue with going through same tile multiple times (This might actually be fine according to some stuff I've read I'll keep it here though)

                if (!IPathing.NodeChecks(current, startNode, goal, location)) continue;

                IPathing.ClosedList.Add(current);

                if (goal.IsEnd(current))
                {
                    Logger.Info($"breaking as current is equal to goal");
                    break; // this is here as cant return in NodeChecks. This checks if this is goal
                }

                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(current);
                foreach (var node in neighbours.Where(node => !IPathing.ClosedList.Contains(node) && !IPathing.collisionMap.IsBlocked(node.X,node.Y)))
                {
                    Logger.Info($"in foreach this is node: {node.X},{node.Y}");
                    IPathing.Frontier.Enqueue(node);
                    IPathing.PathToEndPoint.Push(current);
                }

                increase++;
            }
            
            Logger.Info($"breadth first about to return");

            return IPathing.RebuildPath(startNode, goal, IPathing.PathToEndPoint);
        }
        
    #endregion
    
         private void ClearVariables()
         {
             IPathing.Frontier = new();
             IPathing.ClosedList.Clear();
             IPathing.PathToEndPoint.Clear();
         }
     }
}