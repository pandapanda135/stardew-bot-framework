using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.ObjectDestruction;
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
            
            ClearVariables();
            if (correctPath.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<PathNode>();
            }
            return correctPath;
        }

        private Stack<PathNode> RunAStar(PathNode startPoint, Goal goal, GameLocation location,int limit, bool canDestroyObjects)
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            int increase = 0;

            IPathing.PriorityFrontier.Enqueue(startNode, 0);
            IPathing.ClosedList.Add(startNode);
            
            SerializableDictionary<Vector2, Object> locationObjects = new();
            if (canDestroyObjects)
            {
                foreach (var locationObject in location.Objects)
                {
                    foreach (var kvp in locationObject)
                    {
                        Object obj = kvp.Value;
                        if (DestroyLitterObject.IsDestructible(obj))
                        {
                            locationObjects.Add(kvp.Key,kvp.Value);
                        }
                    }
                }
            }

            // check if goal is blocked before pathfinding
            if (IPathing.collisionMap.IsBlocked(goal.X, goal.Y))
            {
                if (goal is Goal.GoalNearby or Goal.GetToTile) // should probably check radius
                {
                }
                else
                {
                    Logger.Info($"goal is not an available tile");
                    return new Stack<PathNode>();    
                }
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
                
                Logger.Info($"this is current: {current.VectorLocation}");
                // Neighbour search
                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(current);
                foreach (var next in neighbours.Where(node => !IPathing.ClosedList.Contains(node) && !IPathing.collisionMap.IsBlocked(node.X, node.Y) 
                            || canDestroyObjects && locationObjects.ContainsKey(node.VectorLocation.ToVector2())))
                {
                    int newCost = current.Cost - Graph.Cost(current, next);
                    Logger.Error($"this is new cost at start: {newCost}   next.cost: {next.Cost}    contains: {IPathing.PriorityFrontier.Contains(next)}");
                    if (IPathing.PriorityFrontier.Contains(next) && newCost < next.Cost) continue;
                    StardewClient.debugNode.Add(next);
                    
                    // ugly but it works
                    if (canDestroyObjects && IPathing.collisionMap.IsBlocked(next.X,next.Y))
                    {
                        if (DestroyLitterObject.IsDestructible(locationObjects[next.VectorLocation.ToVector2()]))
                            next.Destroy = true;
                        // if (IPathing.DestructibleObjects.Contains(locationObjects[next.VectorLocation.ToVector2()].Name)) next.Destroy = true;
                    }

                    newCost += next.Cost;
                    Logger.Error($"new cost after cost: {newCost}");
                    int priority = newCost + PathNode.ManhattanHeuristic(next.VectorLocation,goal.VectorLocation);
                    Logger.Info($"A Star estimated heuristic {priority}");
                    IPathing.PriorityFrontier.Enqueue(next, priority);
                    IPathing.PathToEndPoint.Push(next);
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
