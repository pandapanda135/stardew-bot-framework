using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.Objects;

namespace StardewBotFramework.Source.Modules.Pathfinding.Algorithms;

public class BreadthFirstSearch : AlgorithmBase
{
     public class Pathing : IPathing
     {
         #region PathFinding
         
         // make this run asynchronously so we can await it when using bot as don't want running actions when pathfinding or just make character controller async (as don't want moving while dropping items as an example)
         async Task IPathing.FindPath(PathNode startPoint, Goal goal, GameLocation location,
             Character character,
             int limit)
         {
             await Task.Run(() =>
             {
                 Logger.Info("started function");
                 ClearVariables();
                 int increase = 0;

                 Logger.Info("started try of breadth first search");
                 PathNode startNode = new PathNode(startPoint.X, startPoint.Y, null);
                 IPathing.Frontier = new PathQueue();
                 IPathing.Frontier.Enqueue(startNode);
                 IPathing.Base.ClosedList.Add(startNode);
                 Logger.Info("before while starts");
                 while (!IPathing.Frontier.IsEmpty())
                 {
                     if (increase > limit)
                     {
                         Logger.Error($"Breaking due to limit");
                         break;
                     }
                     PathNode current = IPathing.Frontier.Dequeue(); // issue with going through same tile multiple times (This might actually be fine according to some stuff I've read I'll keep it here though)

                     if (!IPathing.NodeChecks(current,startNode,goal, location)) continue;
                
                     IPathing.Base.ClosedList.Add(current);

                     if (IPathing.Base.PathToEndPoint.Contains(current)) break; // this is here as cant return in NodeChecks
                 
                     foreach (var node in IPathing.Graph.Neighbours(current).Where(node => !IPathing.Base.ClosedList.Contains(node)))
                     {
                         IPathing.Frontier.Enqueue(node);
                         IPathing.Base.PathToEndPoint.Push(current);
                     }
                 
                     increase++;
                 }

                 Logger.Info($"breadth first about to return");

                 Stack<PathNode> correctPath = IPathing.RebuildPath(startNode, goal, IPathing.Base.PathToEndPoint);
                 
                 CharacterController.StartMoveCharacter(correctPath,Game1.player,Game1.currentLocation,Game1.currentGameTime);
                 
                 ClearVariables();
                 
                 return correctPath;
             });
         }
         
         #endregion
         private void ClearVariables()
         {
             IPathing.Frontier = new();
             IPathing.Base.ClosedList.Clear();
             IPathing.Base.PathToEndPoint.Clear();
         }
     }
}