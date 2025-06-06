using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewPathfinding.Pathfinding;
using StardewPathfinding.Pathfinding.BreadthFirst;
using StardewValley;

namespace StardewBotFramework;

public class PathfindingBot : IBot
{
    public AlgorithmBase.IPathing PathfindingAlgorithm { get; set; }
    public Stack<PathNode> Goals { get; set; }
    
    public PathfindingBot(AlgorithmBase.IPathing pathfindingAlgorithm, Stack<PathNode> goals)
    {
        PathfindingAlgorithm = pathfindingAlgorithm;
        Goals = goals;
    }

    public void SpecifyLocations(GameLocation location, Character character,
        Queue<IBot.Actions> actions)
    {
        PathNode currentTile = new PathNode(Game1.player.TilePoint.X, Game1.player.TilePoint.X, null);

        if (Goals.Count == 1 && actions.Count == 1)
        {
            var path = GetSingleGoal(currentTile, location, character, actions.Dequeue()); 
            if (path.Count > 0)
            {
                foreach (var node in path)
                {
                    Logger.Info($"node X: {node.X}  node Y: {node.Y}");
                }
                Logger.Info($"Successfully got through single goal");
                CharacterController.StartMoveCharacter(path,Game1.player,Game1.player.currentLocation,Game1.currentGameTime);
                return;
            }
            
            Logger.Error($"Issue with getting single goal");
        }
        
        if (Goals.Count > 1 && actions.Count > 1)
        {
            Stack<bool> goalChecks = GetMultipleGoal(currentTile, location, character, actions);
            foreach (var goal in goalChecks)
            {
                if (!goal)
                {
                    Logger.Error($"Issue with getting multiple goal");
                    return;
                }
            }
            
            Logger.Info($"Successfully got through multiple goal");
        }
        
        Logger.Error($"There is an issue with SpecifyLocations. Please check how many goals and actions you have");
    } 
    
    public Stack<PathNode> GetSingleGoal(PathNode current,GameLocation location, Character character, IBot.Actions action)
    {
        PathNode endNode = Goals.Pop();
        var path = PathfindingAlgorithm.FindPath(current, endNode, location, character, 10000);
        
        return PathfindingAlgorithm.RebuildPath(current,endNode,path);
    }
    
    public Stack<bool> GetMultipleGoal(PathNode current, GameLocation location, Character character,Queue<IBot.Actions> actions)
    {
        throw new NotImplementedException();
    }
}