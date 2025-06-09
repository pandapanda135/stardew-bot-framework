using Microsoft.Xna.Framework;
using StardewPathfinding.Debug;
using StardewValley;

namespace StardewPathfinding.Pathfinding.BreadthFirst;

public class BreadthFirstSearch : AlgorithmBase
{
    // class's custom Pathfinding will be implemented here
     public class Pathing : IPathing
     {
         public event EventHandler? ReachedGoal;

         
         #region PathFinding

         
         Stack<PathNode> IPathing.FindPath(PathNode startPoint, PathNode endPoint, GameLocation location,
             Character character,
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
             
             ReachedGoal += OnReachedGoal;
             
             Logger.Info("before while starts");
             while (!IPathing.Frontier.IsEmpty())
             {
                 if (increase > limit)
                 {
                     Logger.Error($"Breaking due to limit");
                     break;
                 }
                 PathNode current = IPathing.Frontier.Dequeue(); // issue with going through same tile multiple times (This might actually be fine according to some stuff I've read I'll keep it here though)

                 if (!IPathing.NodeChecks(current,startNode,endPoint, location)) continue;
                
                 IPathing.Base.ClosedList.Add(current);
                 
                 if (IPathing.Base.PathToEndPoint.Contains(current)) return IPathing.Base.PathToEndPoint; // this is here as cant return in NodeChecks
                 
                 // this is dumb but it works
                 foreach (var node in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                 {
                     IPathing.Frontier.Enqueue(node);
                     IPathing.Base.PathToEndPoint.Push(current);
                 }
                 
                 increase++;
             }
             
             IPathing.EndDebugging();

             Logger.Info($"breadth first about to return");
             if (IPathing.Base.PathToEndPoint.Count < 1)
             {
                 ReachedGoal.Invoke(this,EventArgs.Empty);
             }
             return IPathing.Base.PathToEndPoint;
         }

         public Stack<PathNode> FindMultipleGoals(PathNode startNode, List<PathNode> goals, GameLocation location, Character character, int limit)
         {
             Logger.Info("started function");
             ClearVariables();
             int increase = 0;
             
             Logger.Info("started try of breadth first search");
             IPathing.Frontier = new PathQueue();
             IPathing.Frontier.Enqueue(startNode);
             IPathing.Base.ClosedList.Add(startNode);

             IPathing.Base.Goals = goals;
             
             ReachedGoal += OnReachedGoal;
             
             Logger.Info("before while starts");
             while (!IPathing.Frontier.IsEmpty())
             {
                 if (increase > limit)
                 {
                     Logger.Error($"Breaking due to limit");
                     break;
                 }

                 if (goals.Count == 0) return IPathing.Base.MultipleEndPaths;
                 
                 PathNode current = IPathing.Frontier.Dequeue(); // issue with going through same tile multiple times (This might actually be fine according to some stuff I've read I'll keep it here though)
                 
                 if (!IPathing.MultipleEndNodeChecks(current, startNode, location)) continue; // TODO: get the different paths to goals into their own stacks
                 
                 IPathing.Base.ClosedList.Add(current);
                 
                 if (IPathing.Base.MultipleEndPaths.Contains(current)) return IPathing.Base.MultipleEndPaths; // this is here as cant return in NodeChecks

                 foreach (var node in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                 {
                     IPathing.Frontier.Enqueue(node);
                     IPathing.Base.MultipleEndPaths.Push(current);
                 }
                 
                 increase++;
             }
             
             IPathing.EndDebugging();

             Logger.Info($"breadth first about to return");
             // if (IPathing.Base.MultipleEndPaths.Count < 1)
             // {
             //     ReachedGoal.Invoke(this,EventArgs.Empty);
             // }
             return IPathing.Base.MultipleEndPaths;
         }
             
         #endregion
         private void OnReachedGoal(object? sender, EventArgs e)
         {
             throw new NotImplementedException();
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