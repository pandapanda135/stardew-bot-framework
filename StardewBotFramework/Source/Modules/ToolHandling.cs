using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewBotFramework.Source.Modules;

public class ToolHandling
{
    /// <summary>
    /// Changes direction the player sprite is facing
    /// </summary>
    /// <param name="direction">Goes 0-3 from North,East,South,West</param>
    public void ChangeFacingDirection(int direction)
    {
        BotBase.Farmer.FacingDirection = direction;
    }
    
    /// <summary>
    /// Will use currently held tool.
    /// </summary>
    /// <param name="direction">This acts as a shortcut to <see cref="ChangeFacingDirection"/>. If this is not set or not a valid value the tool will be used in the currently facing direction</param>
    public void UseTool(int direction = -1, bool currentTile = false)
    {
        if (BotBase.Farmer.UsingTool)
        {
            Logger.Error($"Already UsingTool when trying to use direction at {BotBase.Farmer.TilePoint}");
            return;
        }
        
        if (currentTile)
        {
            Logger.Info($"Using current tile");
            BotBase.Farmer.lastClick = new Vector2(BotBase.Farmer.TilePoint.X * 64,
                BotBase.Farmer.TilePoint.Y * 64);
            BotBase.Farmer.BeginUsingTool();
            return;
        }
        
        Logger.Info($"direction: {direction}");
        if (direction < 0 || direction > 4) // should account for if user uses a none valid value
        {
            Logger.Info($"Not using direction at tool");
            BotBase.Farmer.BeginUsingTool(); // Object.performToolAction
            return;
        }

        Logger.Info($"using tool at direction: {direction}");
        ChangeFacingDirection(direction);
        Graph graph = new Graph();
        Queue<Point> points = graph.GroupNeighbours(BotBase.Farmer.TilePoint, 3);
        
        BotBase.Farmer.lastClick = points.ToList()[direction].ToVector2() * 64;
        BotBase.Farmer.BeginUsingTool();
    }

    /// <summary>
    /// Water all patches in this current location, This will only work if the bot is in the farm.
    /// </summary>
    public async Task WaterAllPatches()
    {
        if (BotBase.CurrentLocation is not Farm) return;

        GroupedTiles groupedTiles = new();
        Task<List<Group>> list = groupedTiles.StartDirtCheck(Game1.currentLocation);
        Group finalGroup = new();
        foreach (var kvp in list.Result) // go through groups of dirt
        {
            foreach (var itile in kvp.GetTiles())
            {
                PlantTile tile = (itile as PlantTile)!;
                    
                HoeDirt dirt = (tile.TerrainFeature as HoeDirt)!;
                if (dirt.crop is null || dirt.isWatered()) continue;
                
                // this is because I'm a horrible programmer that's too dumb to find a better way to do this
                IEnumerable<ITile> tiles = finalGroup.GetTiles().Where(tile1 => tile1.Position == tile.Position);
                if (tiles.Any())
                {
                    continue;
                }
                
                Logger.Info($"final group point: {tile.Position}");
                finalGroup.Add(tile);
                StardewClient.debugTiles.Add(tile);
            }
        }
            
        await UseToolOnGroup(finalGroup,new WateringCan());
        Logger.Info($"Ending watering");
    }

    /// <summary>
    /// Refill watering can in this current location.
    /// </summary>
    public async Task RefillWateringCan()
    {
        GetNearestWaterTiles groupedTiles = new();
        Task<Group> group = groupedTiles.GetWaterGroup(BotBase.Farmer.TilePoint,Game1.currentLocation);

        Group finalGroup = group.Result;
        finalGroup = GetNearestWaterTiles.Pathing.RemoveNonBorderWater(finalGroup,BotBase.CurrentLocation);
        foreach (var tile in finalGroup.GetTiles())
        {
            StardewClient.debugTiles.Add(tile);
        }
        await RefillWateringCan(finalGroup,new WateringCan());
        Logger.Info($"Ending watering");
    }
    
    
    private async Task UseToolOnGroup(Group group,Tool tool,int tileAmount = -1)
    {
        SwapItemHandler.SwapItem(typeof(WateringCan),"");
        
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        PriorityQueue<PlantTile, int> plantTileQueue = new();
        if (tileAmount == -1)
        {
            tileAmount = group.GetTiles().Count;
        }
        for (int i = 0; i < tileAmount; i++)
        {
            if (BotBase.Farmer.CurrentTool is not WateringCan wateringCan)
            {
                Logger.Error($"Does not have a watering can");
                return;
            }
            
            if (wateringCan.WaterLeft < 2)
            {
                Logger.Error($"Ran out of water");
                await RefillWateringCan();
            }

            plantTileQueue.Clear();
            foreach (var tile in group.GetTiles())
            {
                PlantTile newTile = (PlantTile)tile;
                plantTileQueue.Enqueue(newTile,newTile.Cost);
            }
            
            PlantTile plantTile = plantTileQueue.Dequeue();
            group.Remove(plantTile);
            
            // should wait until not using tool
            while (BotBase.Farmer.UsingTool)
            {
            }
            
            StardewClient.debugTiles.Remove(plantTile);

            if (plantTile.Position == BotBase.Farmer.TilePoint)
            {
                Logger.Error($"Same place in else");
                UseTool(-2,true);
                continue;
            }
            
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, plantTile.Position, out var direction, 3))
            {
                Logger.Info($"using neighbour if");
                if (direction == -1) continue;
                ChangeFacingDirection(direction);
                UseTool(direction,false);
            }
            else // pathfind to node
            {
                PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);
                
                Stack<PathNode> path = await pathing.FindPath(start,new Goal.GetToTile(plantTile.Position.X,plantTile.Position.Y),BotBase.CurrentLocation,10000);

                if (path == new Stack<PathNode>())
                {
                    Logger.Error($"Stack was empty");
                    UseTool(-2,true);
                    continue;
                }
                
                CharacterController.StartMoveCharacter(path, BotBase.Farmer, BotBase.CurrentLocation,
                    Game1.currentGameTime);

                while (CharacterController.IsMoving()) continue; // this is not async
                
                if (BotBase.Farmer.TilePoint == plantTile.Position) // will sometimes path-find to tile, this should not happen, I'm too lazy to fix this.
                {
                    UseTool(-2,true);
                    continue;
                }

                if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, plantTile.Position, out var pathDirection, 3))
                {
                    Logger.Error($"Tile was not in neighbours: {plantTile.Position}");
                    group.Add(plantTile);
                    continue;
                }
                ChangeFacingDirection(pathDirection);
                UseTool(pathDirection,false);
            }
        }
    }

    private async Task RefillWateringCan(Group group, Tool tool)
    {
        Logger.Info($"Running refill watering can: {group.GetTiles().Count}");
        SwapItemHandler.SwapItem(typeof(WateringCan),"");

        PriorityQueue<WaterTile,int> priorityQueue = new();
        foreach (var tile in group.GetTiles())
        {
            WaterTile waterTile = (tile as WaterTile)!;
            priorityQueue.Enqueue(waterTile,waterTile.Cost);
        }
        
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        int startingWater = (BotBase.Farmer.CurrentTool as WateringCan)!.WaterLeft;
        foreach (var point in group.GetTiles())
        {
            var tile = priorityQueue.Dequeue();
            WateringCan wateringCan = (BotBase.Farmer.CurrentTool as WateringCan)!;

            if (wateringCan.WaterLeft < startingWater) return;
            
            WaterTile? waterTile = GetNearestWaterTiles.Pathing.GetNeighbourWaterTile(tile, BotBase.CurrentLocation);
            if (waterTile is null) continue;
            
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, waterTile.Position, out var direction, 3))
            {
                if (direction == -1) continue;
                ChangeFacingDirection(direction);
                UseTool(direction,false);
                break;
            }
            PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);
            
            Stack<PathNode> path = await pathing.FindPath(start,new Goal.GoalPosition(tile.Position.X,tile.Position.Y),BotBase.CurrentLocation,10000);

            if (path == new Stack<PathNode>())
            {
                Logger.Error($"Stack was empty");
                UseTool(-2,true);
                break;
            }
            
            CharacterController.StartMoveCharacter(path, BotBase.Farmer, BotBase.CurrentLocation,
                Game1.currentGameTime);

            while (CharacterController.IsMoving()) continue; // this is not async
            
            if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, waterTile.Position, out var pathDirection, 3)) continue;
            ChangeFacingDirection(pathDirection);
            Logger.Error($"using bottom of useTool else");
            UseTool(pathDirection,false);
            break;
        }
    }
}