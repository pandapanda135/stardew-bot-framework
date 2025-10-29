using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.ObjectDestruction;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Pathfinding.Algorithms;

public class AStar : AlgorithmBase
{
    public class Pathing : IPathing
    {
        #region Pathfinding

        private const int AverageTileCost = 3; // between 1-6 in PathNodes
        async Task<Stack<PathNode>> IPathing.FindPath(PathNode startPoint, Goal goal, GameLocation location,
            int limit, bool canDestroy)
        {
             // this is for it to run on a background thread to stop stutter
             Stack<PathNode> correctPath = await Task.Run(() => RunAStar(startPoint, goal, location, limit, canDestroy));
            
            ClearVariables();
            if (correctPath.Count != 0) return correctPath;
            
            Logger.Error($"Rebuild path returned empty stack");
            return new Stack<PathNode>();
        }
        
        private Stack<PathNode> RunAStar(PathNode startPoint, Goal goal, GameLocation location,int limit, bool canDestroyObjects)
        {
            ClearVariables();

            // Doesn't happen with Pathfinder, but it could if some calls this directly
            startPoint.Parent = null;
            
            int increase = 0;
            IPathing.PriorityFrontier.Enqueue(startPoint, 0);
            // may need this later so will keep for now
            // IPathing.ClosedList.Add(startPoint);
            
            Dictionary<Vector2, Object> locationObjects = new();
            
            if (canDestroyObjects)
            {
                foreach (var locationObject in location.Objects)
                {
                    foreach (var kvp in locationObject.Where(kvp => DestroyLitterObject.IsDestructible(kvp.Value)))
                    {
                        locationObjects.Add(kvp.Key,kvp.Value);
                    }
                }
            }

            // check if goal is blocked before pathfinding
            if (IPathing.CollisionMap.IsBlocked(goal.X, goal.Y))
            {
                if (goal is Goal.GoalNearby or Goal.GetToTile or Goal.GoalDynamic) // should probably check radius
                {
                }
                else
                {
                    Logger.Warning($"goal is not an available tile");
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

                if (!IPathing.NodeChecks(current, location)) continue;
                
                IPathing.ClosedList.Add(current);
                
                // This check for goal depending on the type of goal.
                if (IPathing.CanEnd(current,goal))
                {
                    Logger.Info($"breaking as current is equal to goal");
                    IPathing.EndNode = current;
                    break;
                }
                
                Logger.Info($"this is current: {current.VectorLocation}");
                // Neighbour search
                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(current);
                foreach (var next in neighbours.Where(node => 
                             IPathing.ClosedList.All(n => n.VectorLocation != node.VectorLocation) 
                             && !IPathing.CollisionMap.IsBlocked(node.X, node.Y)
                             || canDestroyObjects && locationObjects.ContainsKey(node.VectorLocation.ToVector2())))
                {
                    int newCumulative = current.GCost + next.Cost;
                    Logger.Info($"this is new cost at start: {newCumulative}   next.cost: {next.Cost}");
                    
                    if (IPathing.PriorityFrontier.Contains(next) && newCumulative >= next.GCost) continue;
                    StardewClient.DebugNode.GetOrAdd(next,byte.MinValue);
       
                    // ugly but it works
                    if (canDestroyObjects && IPathing.CollisionMap.IsBlocked(next.X,next.Y))
                    {
                        if (DestroyLitterObject.IsDestructible(locationObjects[next.VectorLocation.ToVector2()]))
                            next.Destroy = true;
                    }

                    // we don't use newCumulative as that leads to it being incredibly inefficient
                    next.GCost = current.GCost + 1;
                    // we weight heuristic to find a path quicker, this may lead to more inefficient paths though.
                    // Also multiply to make heuristic be similar to GCost. This stops issues like waving in and out of a straight line.
                    int priority = next.GCost + (goal.ManhattanHeuristic(next) * AverageTileCost);
                    Logger.Info($"A Star estimated heuristic {priority}");
                    IPathing.PriorityFrontier.Enqueue(next, priority);
                }

                increase++;
            }
            
            Logger.Info($"AStar about to return");
            return IPathing.RebuildPath(startPoint, goal,IPathing.EndNode);
        }

        #endregion
        private void ClearVariables()
        {
            IPathing.Frontier = new();
            IPathing.PriorityFrontier.Clear();
            IPathing.ClosedList.Clear();
            IPathing.EndNode = null;
        }
    }
}
