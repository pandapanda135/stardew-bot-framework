using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewPathfinding.Debug;
using StardewPathfinding.Pathfinding;
using StardewPathfinding.Pathfinding.BreadthFirst;
using StardewPathfinding.Pathfinding.UniformCost;
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

            BreathFirstSearchKeybind(Game1.currentCursorTile);
        }
        else if (e.Button == SButton.K)
        {
            Logger.Info("User has Pressed button to start Uniform");
    
            UniformCostSearchKeybind(Game1.currentCursorTile);
        }
    }

    private static Pathfinding.AlgorithmBase.IPathing _breadthpathing = new BreadthFirstSearch.Pathing();
    private static Pathfinding.AlgorithmBase.IPathing _Uniformpathing = new UniformCostSearch.Pathing();

    public static Stack<PathNode> _stackPoint = new Stack<PathNode>();
    public static Stack<PathNode> CorrectPath = new Stack<PathNode>();

    private static void BreathFirstSearchTest(string name, string[] args)
    {
        Point startPoint = new Point((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y);
        Point endPoint = new Point((int)Game1.player.TilePoint.X - 10, (int)Game1.player.TilePoint.Y + 20);
        
        _stackPoint = _breadthpathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _breadthpathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),_stackPoint.Pop(), _stackPoint);
        
        foreach (var node in CorrectPath)
        {
            Logger.Info($"Node positions {node.X},{node.Y} : {node.id} : {node.Parent}   first Stackpoint");
        }
    }
    
    private static void BreathFirstSearchKeybind(Vector2 cursorPoint)
    {
        _stackPoint.Clear();
        CorrectPath.Clear();
        
        Point startPoint = new Point((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y);
        Point endPoint = new Point((int)cursorPoint.X, (int)cursorPoint.Y);
        
        _stackPoint = _breadthpathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _breadthpathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),_stackPoint.Pop(), _stackPoint);
    }
    
    private static void UniformCostSearchKeybind(Vector2 cursorPoint)
    {
        _stackPoint.Clear();
        CorrectPath.Clear();
        
        Point startPoint = new Point((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y);
        Point endPoint = new Point((int)cursorPoint.X, (int)cursorPoint.Y);
        
        _stackPoint = _Uniformpathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _Uniformpathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),_stackPoint.Pop(), _stackPoint);
    }
}
    