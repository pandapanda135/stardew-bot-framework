using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.Network;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding.Algorithms;

public class AStar : AlgorithmBase
{
    public class Pathing : IPathing
    {
        #region Pathfinding
        
        async Task<Stack<PathNode>> IPathing.FindPath(PathNode startPoint, Goal goal, GameLocation location,
            int limit, bool canDestroy)
        {
            Stack<PathNode> correctPath = await Task.Run(() => RunAStar(startPoint, goal, location, limit,canDestroy)); // this is for it to run on a background thread
            
            if (correctPath.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<PathNode>();
            }
            
            ClearVariables();

            return correctPath;
        }

        private Stack<PathNode> RunAStar(PathNode startPoint, Goal goal, GameLocation location,int limit, bool canDestroy)
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
            IPathing.PriorityFrontier = new();
            IPathing.PriorityFrontier.Enqueue(startNode, 0);
            IPathing.ClosedList.Add(startNode);
            
            int increase = 0;
            
            OverlaidDictionary locationObjectsDict = location.objects;
            SerializableDictionary<Vector2, Object> locationObjects = new();
            if (canDestroy)
            {
                foreach (var locationObject in locationObjectsDict)
                {
                    foreach (var kvp in locationObject)
                    {
                        locationObjects.Add(kvp.Key,kvp.Value);
                    }
                }
            }

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
                
                IPathing.ClosedList.Add(current);
                
                if (IPathing.CanEnd(current,goal))
                {
                    Logger.Info($"breaking as current is equal to goal");
                    // IPathing.PathToEndPoint.Push(current);
                    break; // this is here as cant return in NodeChecks. This checks if this is goal
                }
                
                Logger.Info($"this is current: {current}");
                // Neighbour search
                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(current);
                foreach (var next in neighbours.Where(node => !IPathing.ClosedList.Contains(node) && !IPathing.collisionMap.IsBlocked(node.X, node.Y) 
                                                              || canDestroy && locationObjects.ContainsKey(node.VectorLocation.ToVector2())))
                {
                    int newCost = current.Cost + Graph.Cost(current, next);
                    if (!IPathing.PriorityFrontier.Contains(next) || newCost < next.Cost)
                    {
                        // ugly but it works
                        if (canDestroy && IPathing.collisionMap.IsBlocked(next.X,next.Y))
                        {
                            if (IPathing.DestructibleObjects.Contains(locationObjects[next.VectorLocation.ToVector2()].Name)) next.Destroy = true;
                        }
                        
                        next.Cost = newCost;
                        int priority = newCost + PathNode.ManhattanHeuristic(next.VectorLocation.ToVector2(),goal.VectorLocation.ToVector2());
                        Logger.Info($"A Star estimated heuristic {priority}");
                        IPathing.PriorityFrontier.Enqueue(next, priority);
                        IPathing.PathToEndPoint.Push(next);
                    }
                }

                increase++;
            }
            
            Logger.Info($"AStar about to return");
            return IPathing.RebuildPath(startNode, goal, IPathing.PathToEndPoint);
        }

        #endregion
        private void ClearVariables()
        {
            IPathing.Frontier = new();
            IPathing.PriorityFrontier.Clear();
            IPathing.ClosedList.Clear();
            IPathing.PathToEndPoint.Clear();
        }
    }
}
