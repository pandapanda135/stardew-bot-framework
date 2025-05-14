using Microsoft.Xna.Framework;
using StardewPathfinding.TileInterface;
using StardewValley;
using StardewValley.Menus;
using Object = System.Object;

namespace StardewPathfinding.Pathfinding;

public class BreadthFirstSearch : Pathfinding
{
    static Pathfinding pathfinding  = new Pathfinding();

    public static IPathfindingTiles pathfindingTiles;

    public static PathQueue Frontier;
    
    // class's custom Pathfinding will be implemented here
     public class Pathing : IPathing
     {
         Stack<Point> IPathing.FindPath(Point startPoint, Point endPoint, GameLocation location, Character character,
             int limit)
         {
             Frontier = new PathQueue(); // use instead of _openlist because easier
             Frontier.Enqueue(new PathNode(startPoint.X,startPoint.Y,null));
             pathfinding.ClosedList.Append(new PathNode(startPoint.X, startPoint.Y, null));
             
             while (!Frontier.IsEmpty())
             {
                 PathNode current = Frontier.Dequeue();

                 foreach (var nextNode in pathfinding.OpenList)
                 {
                     if (!pathfinding.ClosedList.Contains(nextNode))
                     {
                         Frontier.Enqueue(nextNode);
                         pathfinding.ClosedList.Append(nextNode);
                     }
                 }
             }

             return pathfinding.PathToEndPoint;
         }
     }
}