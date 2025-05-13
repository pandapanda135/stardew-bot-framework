using Microsoft.Xna.Framework;
using StardewPathfinding.TileInterface;
using StardewValley;
using Object = System.Object;

namespace StardewPathfinding.Pathfinding;

public class BreadthFirstSearch : Pathfinding
{
    static Pathfinding pathfinding  = new Pathfinding();

    public static IPathfindingTiles pathfindingTiles;

    public static List<Object> Frontier;
    
    // class's custom Pathfinding will be implemented here
//     public class Pathing : IPathing
//     {
//         Stack<Point> IPathing.FindPath(Point startPoint, Point endPoint, GameLocation location, Character character,
//             int limit)
//         {
//             Frontier = pathfindingTiles.GetBadTiles();
//             pathfinding.OpenList.Add(startPoint);
//
//             while (Frontier.Count != 0)
//             {
//                 Point current = Frontier.
//             }
//         }
//     }
}