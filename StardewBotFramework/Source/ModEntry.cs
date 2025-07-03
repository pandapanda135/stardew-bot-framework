using System.Collections;
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Pathfinding;
using PathNode = StardewBotFramework.Source.Modules.Pathfinding.Base.PathNode;

namespace StardewBotFramework.Source;

public class Main : Mod
{
    public static Stack<PathNode> PathNodes = new();
    
    public override void Entry(IModHelper helper)
    {
        Logger.SetMonitor(Monitor);

        helper.Events.Input.ButtonPressed += ButtonPressed;
        helper.Events.GameLoop.UpdateTicking += CharacterController.Update;
        helper.Events.Display.Rendered += DrawFoundTiles.OnRenderTiles;
    }

    public void ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        // if (e.Button == SButton.H)
        // {
        //     Logger.Info("User has Pressed button to create bot");
        //
        //     // put creation of bot here (this will be for pathfinding)
        //
        //     Vector2 testPosition = Game1.currentCursorTile;
        //
        //     AlgorithmBase.IPathing algorithmBase = new GreedyBestFirstSearch.Pathing();
        //     Stack<PathNode> goals = new();
        //     Queue<IBot.Actions> actionsQueue = new();
        //     
        //     goals.Push(new PathNode(testPosition.ToPoint(), null));
        //     actionsQueue.Enqueue(IBot.Actions.Movement);
        //
        //     PathfindingBot testBot = new PathfindingBot(algorithmBase,goals);
        //
        //     testBot.OnBotFinished += PathfindingFinished;
        //     
        //     testBot.SpecifyLocations(Game1.currentLocation,Game1.player,actionsQueue);
        // }
    }

    // temp for testing
    
    private static void PathfindingFinished()
    {
        Logger.Info($"Pathfinding event invoked");
    }
}
