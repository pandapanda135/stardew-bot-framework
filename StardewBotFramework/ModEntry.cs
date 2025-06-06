using System.Collections;
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewPathfinding.Pathfinding;
using StardewPathfinding.Pathfinding.BreadthFirst;
using StardewPathfinding.Pathfinding.GreedyBest;
using StardewValley;
using StardewValley.Pathfinding;
using PathNode = StardewPathfinding.Pathfinding.PathNode;

namespace StardewBotFramework;

public class Main : Mod
{
    public override void Entry(IModHelper helper)
    {
        Logger.SetMonitor(Monitor);

        helper.Events.Input.ButtonPressed += ButtonPressed;
        helper.Events.GameLoop.UpdateTicking += CharacterController.Update;
    }

    public void ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (e.Button == SButton.H)
        {
            Logger.Info("User has Pressed button to create bot");

            // put creation of bot here (this will be for pathfinding)

            Vector2 testPosition = Game1.currentCursorTile;

            AlgorithmBase.IPathing algorithmBase = new GreedyBestFirstSearch.Pathing();
            Stack<PathNode> goals = new Stack<PathNode>();
            Queue<IBot.Actions> actionsQueue = new Queue<IBot.Actions>();
            
            goals.Push(new PathNode((int)testPosition.X, (int)testPosition.Y, null));
            actionsQueue.Enqueue(IBot.Actions.Movement);
            
            PathfindingBot testBot = new PathfindingBot(algorithmBase,goals);
            
            testBot.SpecifyLocations(Game1.currentLocation,Game1.player,actionsQueue);
        }
    }

    // temp for testing
}
