using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.TileInterface;
using StardewValley;
using StardewPathfinding.Graphs;
using StardewPathfinding.Pathfinding;

namespace StardewPathfinding.Pathfinding.BreadthFirst;

public class BreadthFirstSearch : AlgorithmBase
{
    static AlgorithmBase _base  = new AlgorithmBase();

    private static Graph _graph = new Graph();
    
    // class's custom Pathfinding will be implemented here
     public class Pathing : IPathing
     {
         Stack<PathNode> IPathing.FindPath(Point startPoint, Point endPoint, GameLocation location, Character character,
             int limit)
         {
             Logger.Info("started function");
             ClearVariables();
             int increase = 0;
             
             Logger.Info("started try of breadth first search");
             PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
             IPathing.Frontier = new PathQueue(); // use instead of _openlist because easier
             IPathing.Frontier.Enqueue(startNode);
             _base.ClosedList.Add(startNode);

             bool alreadyExists = false;
             
             Logger.Info("before while starts");
             while (!IPathing.Frontier.IsEmpty())
             {
                 alreadyExists = false;
                 
                 if (increase > limit)
                 {
                     Logger.Error($"Breaking due to limit");
                     break;
                 }
                 PathNode current = IPathing.Frontier.Dequeue(); // issue with going through same tile multiple times (This might actually be fine according to some stuff I've read I'll keep it here though)

                 Logger.Info($"Current tile {current.X},{current.Y}");

                 if (_graph.CheckIfEnd(current, endPoint))
                 {
                     Logger.Info($"Ending using CheckIfEnd function");
                     // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths
                     
                     _base.PathToEndPoint.Push(current);
                     return _base.PathToEndPoint;
                 }
                 
                 // next loop if current is already in ClosedList
                 foreach (var node in _base.ClosedList)
                 {
                     Vector2 currentVector = new Vector2(current.X, current.Y);
                     Vector2 nextVector = new Vector2(node.X, node.Y);
                     if (currentVector == nextVector && startNode != current) alreadyExists = true;
                 }

                 if (alreadyExists) continue;
                 
                 _base.ClosedList.Add(current);
                 // this is dumb but it works
                 foreach (var node in _graph.Neighbours(current).Where(node => !_base.ClosedList.Contains(node)))
                 {
                     IPathing.Frontier.Enqueue(node);
                     _base.PathToEndPoint.Push(current);
                 }
                 
                 increase++;
             }
             if (_base.PathToEndPoint.Count > 0)
             {
                 foreach (var pathNode in _base.PathToEndPoint)
                 {
                     Logger.Info($"node in end point path   {pathNode.X}   {pathNode.Y}");
                 }
             }
             Logger.Info($"breadth first about to return");
             return _base.PathToEndPoint;
         }

         public Stack<PathNode> RebuildPath(PathNode startPoint,PathNode endPoint,Stack<PathNode> path)
         {
             PathNode current = endPoint;

             Stack<PathNode> correctPath = new();
             
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

         private void ClearVariables()
         {
             IPathing.Frontier = new();
             _base.ClosedList!.Clear();
             _base.PathToEndPoint.Clear();
             DrawFoundTiles.debugDirectionTiles.Clear();
         }
     }
}