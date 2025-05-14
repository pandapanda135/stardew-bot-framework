using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.TileInterface;
using StardewValley;

namespace StardewPathfinding.Pathfinding;

public class BreadthFirstSearch : Pathfinding
{
    static Pathfinding _pathfinding  = new Pathfinding();

    public static IPathfindingTiles pathfindingTiles;

    private static PathQueue _frontier = new PathQueue();
    
    // class's custom Pathfinding will be implemented here
     public class Pathing : IPathing
     {
         Stack<PathNode> IPathing.FindPath(Point startPoint, Point endPoint, GameLocation location, Character character,
             int limit)
         {
             Logger.Info("started function");
             _frontier.Clear();

             int increase = 0;
             
             try
             {
                 Logger.Info("started try of breadth first search");
                 PathNode startPointNode = new PathNode(startPoint.X, startPoint.Y, null);
                 _frontier = new PathQueue(); // use instead of _openlist because easier
                 _frontier.Enqueue(startPointNode);
                 _pathfinding.ClosedList.Add(startPointNode);
                
                 Logger.Info("before while starts");
                 while (!_frontier.IsEmpty())
                 {
                     if (increase > limit)
                     {
                         break;
                     }
                     PathNode current = _frontier.Dequeue();

                     _pathfinding.ClosedList.Add(current);
                 
                     for (int i = 0; i <= 4; i++)
                     {
                         int neighborX = current.X + Directions[i, 0]; // index out of bounds error
                         int neighborY = current.X + Directions[i, 1];

                         _pathfinding.OpenList.Enqueue(new PathNode(neighborX,neighborY,current.id));
                     }

                     _pathfinding.OpenList.Reverse(); // this is done as otherwise get ugly paths
                     
                     foreach (var nextNode in _pathfinding.OpenList)
                     {
                         if (!_pathfinding.ClosedList.Contains(nextNode))
                         {
                             _frontier.Enqueue(nextNode);
                             _pathfinding.ClosedList.Add(nextNode);
                        
                             // this is here temp just to get return out
                             _pathfinding.PathToEndPoint.Push(nextNode);

                         }
                     }

                     increase++;
                 }
                 Logger.Debug($"This is the end point {_pathfinding.PathToEndPoint}");
                 return _pathfinding.PathToEndPoint;
             }
             catch (Exception e)
             {
                 Logger.Error($"Error with breadth first search with error:  {e}");
                 return _pathfinding.PathToEndPoint;
             }
         }
     }
}