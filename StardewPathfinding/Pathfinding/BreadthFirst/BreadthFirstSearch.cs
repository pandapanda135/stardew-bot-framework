using System.Collections;
using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewPathfinding.TileInterface;
using StardewValley;
using StardewPathfinding.Graphs;
using StardewPathfinding.Pathfinding;

namespace StardewPathfinding.Pathfinding.BreadthFirst;

public class BreadthFirstSearch : AlgorithmBase
{
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
             IPathing.Base.ClosedList.Add(startNode);

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
                 
                 if (current.X > location.Map.DisplayWidth / Game1.tileSize || current.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize || current.X < 0 || current.Y < 0)
                 {
                     Logger.Info($"Blocking this tile: {current.X},{current.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                     continue;
                 }

                 Logger.Info($"Current tile {current.X},{current.Y}");

                 if (IPathing.Graph.CheckIfEnd(current, endPoint))
                 {
                     Logger.Info($"Ending using CheckIfEnd function");
                     // _pathfinding.PathToEndPoint.Reverse(); // this is done as otherwise get ugly paths
                     
                     IPathing.Base.PathToEndPoint.Push(current);
                     return IPathing.Base.PathToEndPoint;
                 }
                 
                 // next loop if current is already in ClosedList
                 foreach (PathNode node in IPathing.Base.ClosedList)
                 {
                     Vector2 currentVector = new Vector2(current.X, current.Y);
                     Vector2 nextVector = new Vector2(node.X, node.Y);
                     if (currentVector == nextVector && startNode != current) alreadyExists = true;
                 }

                 if (alreadyExists) continue;
                 
                 IPathing.Base.ClosedList.Add(current);
                 // this is dumb but it works
                 foreach (var node in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                 {
                     IPathing.Frontier.Enqueue(node);
                     IPathing.Base.PathToEndPoint.Push(current);
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
             Logger.Info($"breadth first about to return");
             return IPathing.Base.PathToEndPoint;
         }

         private void ClearVariables()
         {
             IPathing.Frontier = new();
             IPathing.Base.ClosedList!.Clear();
             IPathing.Base.PathToEndPoint.Clear();
             DrawFoundTiles.debugDirectionTiles.Clear();
         }
     }
}