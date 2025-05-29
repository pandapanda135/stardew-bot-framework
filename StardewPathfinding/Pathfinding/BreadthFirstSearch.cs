using System.Collections;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.Serialization;
using System.Transactions;
using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.TileInterface;
using StardewValley;
using StardewValley.Pathfinding;

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
                 PathNode endPointNode = new PathNode(endPoint.X, endPoint.Y, null);
                 _frontier = new PathQueue(); // use instead of _openlist because easier
                 _frontier.Enqueue(startPointNode);
                 _pathfinding.ClosedList.Add(startPointNode);

                 bool alreadyExists = false;
                 
                 Logger.Info("before while starts");
                 while (!_frontier.IsEmpty())
                 {
                     alreadyExists = false;
                     
                     if (increase > limit)
                     {
                         Logger.Error($"Breaking due to limit");
                         break;
                     }
                     PathNode current = _frontier.Dequeue(); // TODO: issue with going through same tile multiple times

                     Logger.Info($"Current tile {current.X},{current.Y}");

                     if (CheckIfEnd(current, endPoint))
                     {
                         Logger.Info($"Ending using CheckIfEnd function");
                         // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths
                         
                         _pathfinding.PathToEndPoint.Push(current);
                         return _pathfinding.PathToEndPoint;
                     }
                     
                     // next loop if current is already in ClosedList
                     foreach (var node in _pathfinding.ClosedList)
                     {
                         Vector2 currentVector = new Vector2(current.X, current.Y);
                         Vector2 nextVector = new Vector2(node.X, node.Y);
                         if (currentVector == nextVector && startPointNode != current ) alreadyExists = true;
                     }

                     if (alreadyExists) continue;
                     
                     _pathfinding.ClosedList.Add(current);
                     // this is dumb but it works
                     foreach (var node in _pathfinding.Neighbours(current).Where(node => !_pathfinding.ClosedList.Contains(node)))
                     {
                         if (_pathfinding.ClosedList.Contains(node))
                         {
                             continue;
                         }
                         _frontier.Enqueue(node);
                         _pathfinding.PathToEndPoint.Push(current);
                     }
                     
                     increase++;
                 }
                 Logger.Debug($"This is the end point {_pathfinding.PathToEndPoint.Count}");
                 if (_pathfinding.PathToEndPoint.Count > 0)
                 {
                     foreach (var pathNode in _pathfinding.PathToEndPoint)
                     {
                         Logger.Info($"node in end point path   {pathNode.X}   {pathNode.Y}");
                     }
                 }
                 Logger.Info($"breadth first about to return");
                 return _pathfinding.PathToEndPoint;
             }
             catch (Exception e)
             {
                 Logger.Error($"Error with breadth first search with error:  {e}");
                 return _pathfinding.PathToEndPoint;
             }
         }

         public Stack<PathNode> RebuildPath(PathNode startPoint,PathNode endPoint,Stack<PathNode> path)
         {
             PathNode current = endPoint;

             Stack<PathNode> correctPath = new();

             PathNode? previousNodeParent = current.Parent;
             
             Logger.Info($"previous node parent {current.id}   {current.Parent}");

             while (current != startPoint)
             {
                 Logger.Info($"new current  {current.id}");
                 correctPath.Push(current);
                 if (current.Parent is not null)
                 {
                     current = current.Parent!;
                     continue;
                 }
                 
                 break;
             }
             
             return correctPath;
         }
     }
}