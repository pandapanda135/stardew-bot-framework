using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewPathfinding.Debug;
using StardewPathfinding.Pathfinding;
using StardewValley;

namespace StardewPathfinding;

public class Main : Mod
{
    public override void Entry(IModHelper helper)
    {
        Logger.SetMonitor(Monitor);

        helper.Events.Input.ButtonPressed += ButtonPressed;
        helper.Events.Display.Rendered += DrawFoundTiles.OnRenderTiles;

        helper.ConsoleCommands.Add("first", "this is breadth First Search", BreathFirstSearchTest);
    }

    public void ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (e.Button == SButton.J)
        {
            Logger.Info("User has Pressed button to start");

            // Start pathfinding stuff here
        }
    }

    private static Pathfinding.Pathfinding.IPathing _pathing = new BreadthFirstSearch.Pathing();

    public static Stack<PathNode> _stackPoint = new Stack<PathNode>();

    private static void BreathFirstSearchTest(string name, string[] args)
    {
        Point startPoint = new Point((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y);
        Point endPoint = new Point((int)Game1.player.TilePoint.X - 5, (int)Game1.player.TilePoint.Y + 5);
        
        _stackPoint = _pathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        foreach (var node in _stackPoint)
        {
            Logger.Info($"Node positions {node.X},{node.Y} : {node.id} : {node.Parent}");
        }
    }
}
    