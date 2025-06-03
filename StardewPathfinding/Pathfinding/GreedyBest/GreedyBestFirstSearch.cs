using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewValley;

namespace StardewPathfinding.Pathfinding.GreedyBest;

public class GreedyBestFirstSearch : AlgorithmBase
{
    public class Pathing : IPathing
    {
        public Stack<PathNode> FindPath(Point startPoint, Point endPoint, GameLocation location,
            Character player, int limit)
        {
            ClearVariables();
            
            PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
            
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

                // go onto next node if current is outside of map
                if (current.X > (location.Map.DisplayWidth / Game1.tileSize) - 1 || current.Y > (Game1.currentLocation.Map.DisplayHeight / Game1.tileSize) - 1 || current.X < 0 || current.Y < 0)
                {
                    Logger.Info($"Blocking this tile: {current.X},{current.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                    continue;
                }
                
                Logger.Info($"Current tile {current.X},{current.Y}");

                if (IPathing.Graph.CheckIfEnd(current, endPoint))
                {
                    Logger.Info($"Ending using CheckIfEnd function");
                    // _base.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths
                    
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

                foreach (var next in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                {
                    int priority = PathNode.ManhattanHeuristic(new Vector2(next.X, next.Y),endPoint.ToVector2());
                    Logger.Info($"heuristic: {priority}");
                    IPathing.PriorityFrontier.Enqueue(next,priority);
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

            Logger.Info($"Greedy about to return");
            return IPathing.Base.PathToEndPoint;
        }

        private void ClearVariables()
        {
            IPathing.PriorityFrontier.Clear();
            IPathing.Base.ClosedList!.Clear();
            IPathing.Base.PathToEndPoint.Clear();
            DrawFoundTiles.debugDirectionTiles.Clear();
        }
    }
}