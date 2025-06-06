﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewPathfinding.Debug;
using StardewPathfinding.Pathfinding;
using StardewPathfinding.Pathfinding.AStar;
using StardewPathfinding.Pathfinding.BreadthFirst;
using StardewPathfinding.Pathfinding.GreedyBest;
using StardewPathfinding.Pathfinding.UniformCost;
using StardewValley;

namespace StardewPathfinding;

public class Main : Mod
{
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

        if (e.Button == SButton.H)
        {
            Logger.Info("User has Pressed button to start");

            BreathFirstSearchKeybind(Game1.currentCursorTile);
        }
        else if (e.Button == SButton.J)
        {
            Logger.Info("User has Pressed button to start Uniform");
    
            UniformCostSearchKeybind(Game1.currentCursorTile);
        }
        else if (e.Button == SButton.K)
        {
            Logger.Info("User has Pressed button to start Greedy");
    
            GreedyBestFirstKeyBind(Game1.currentCursorTile);
        }
        else if (e.Button == SButton.U)
        {
            Logger.Info("User has Pressed button to start Greedy");
    
            AStarKeyBind(Game1.currentCursorTile);
        }
    }

    // temp for testing
    private static AlgorithmBase.IPathing _breadthpathing = new BreadthFirstSearch.Pathing();
    private static AlgorithmBase.IPathing _uniformpathing = new UniformCostSearch.Pathing();
    private static AlgorithmBase.IPathing _greedypathing = new GreedyBestFirstSearch.Pathing();
    private static AlgorithmBase.IPathing _aStarpathing = new AStarPathfinding.Pathing();


    public static Stack<PathNode> _stackPoint = new Stack<PathNode>();
    public static Stack<PathNode> CorrectPath = new Stack<PathNode>();

    private static PathNode startPoint;
    private static PathNode endPoint;
    
    private static void BreathFirstSearchKeybind(Vector2 cursorPoint)
    {
        _stackPoint.Clear();
        CorrectPath.Clear();
        
        startPoint = new PathNode((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y,null);
        endPoint = new PathNode((int)cursorPoint.X,(int)cursorPoint.Y,null);
        
        _stackPoint = _breadthpathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _breadthpathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),new PathNode(endPoint.X,endPoint.Y,null), _stackPoint);
    }
    
    private static void UniformCostSearchKeybind(Vector2 cursorPoint)
    {
        _stackPoint.Clear();
        CorrectPath.Clear();
        
        startPoint = new PathNode((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y,null);
        endPoint = new PathNode((int)cursorPoint.X,(int)cursorPoint.Y,null);
        
        _stackPoint = _uniformpathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _uniformpathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),new PathNode(endPoint.X,endPoint.Y,null), _stackPoint);
    }
    
    private static void GreedyBestFirstKeyBind(Vector2 cursorPoint)
    {
        _stackPoint.Clear();
        CorrectPath.Clear();
        
        startPoint = new PathNode((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y,null);
        endPoint = new PathNode((int)cursorPoint.X,(int)cursorPoint.Y,null);

        CharacterController characterController = new CharacterController();
        
        _stackPoint = _greedypathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _greedypathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),new PathNode(endPoint.X,endPoint.Y,null), _stackPoint);
        
        CharacterController.StartMoveCharacter(CorrectPath,Game1.player,Game1.player.currentLocation,Game1.currentGameTime);
    }
    
    private static void AStarKeyBind(Vector2 cursorPoint)
    {
        _stackPoint.Clear();
        CorrectPath.Clear();
        
        startPoint = new PathNode((int)Game1.player.TilePoint.X, (int)Game1.player.TilePoint.Y,null);
        endPoint = new PathNode((int)cursorPoint.X,(int)cursorPoint.Y,null);
        
        _stackPoint = _aStarpathing.FindPath(startPoint, endPoint, Game1.player.currentLocation, Game1.player, 10000);

        CorrectPath = _aStarpathing.RebuildPath(new PathNode(startPoint.X,startPoint.Y,null),new PathNode(endPoint.X,endPoint.Y,null), _stackPoint);
    }
}
