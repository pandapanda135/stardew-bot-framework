using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;
using StardewBotFramework.Source.ObjectDestruction;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewBotFramework.Source.Utilities;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// This is for stuff you can do with tools, this also includes some helper methods for making farm-land, watering it and destroying objects
/// </summary>
public class ToolHandling
{
    public bool Running => _runAction is not null;
    private static Action<List<ITile>>? _runAction;
    private static List<ITile> _tiles = new();
    public static void Update(object? sender, UpdateTickingEventArgs e)
    {
        if (_runAction is null) return;
        
        if (CharacterController.IsMoving()) return;

        if (!_tiles.Any() && !BotBase.Farmer.UsingTool && !_destroying)
        {
            _runAction = null;
            return;
        }
        
        _runAction(_tiles);
    }
    
    /// <summary>
    /// Changes direction the player sprite is facing
    /// </summary>
    /// <param name="direction">Goes 0-3 from North,East,South,West</param>
    private void ChangeFacingDirection(int direction)
    {
        BotBase.Farmer.FacingDirection = direction;
    }
    
    /// <summary>
    /// Will use currently held tool.
    /// </summary>
    /// <param name="direction">This acts as a shortcut to <see cref="ChangeFacingDirection"/>. If this is not set or not a valid value the tool will be used in the currently facing direction</param>
    /// <param name="currentTile">This will use the tool on the current tile.</param>
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
            BotBase.Farmer.lastClick = BotBase.Farmer.Position;
            BotBase.Instance?.Helper.Input.SetCursorPosition(BotBase.Farmer.TilePoint.X,BotBase.Farmer.TilePoint.Y);
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
        Queue<Point> points = graph.GroupNeighbours(BotBase.Farmer.TilePoint, 4);
        
        Logger.Info($"current point: {points.ToList()[direction]}");
        switch (direction) // N,E,S,W
        {
            case 0:
                BotBase.Farmer.lastClick = new Vector2(BotBase.Farmer.lastClick.X,
                    BotBase.Farmer.lastClick.Y - 1 * 64);
                BotBase.Farmer.BeginUsingTool();
                break;
            case 1:
                BotBase.Farmer.lastClick = new Vector2(BotBase.Farmer.lastClick.X + 1 * 64,
                    BotBase.Farmer.lastClick.Y);
                BotBase.Farmer.BeginUsingTool();
                break;
            case 2:
                BotBase.Farmer.lastClick = new Vector2(BotBase.Farmer.lastClick.X,
                    BotBase.Farmer.lastClick.Y + 1 * 64);
                BotBase.Farmer.BeginUsingTool();
                break;
            case 3:
                BotBase.Farmer.lastClick = new Vector2(BotBase.Farmer.lastClick.X - 1 * 64,
                    BotBase.Farmer.lastClick.Y);
                BotBase.Farmer.BeginUsingTool();
                break;
            default:
                Logger.Error($"{direction} is not in switch statement");
                break;
        }
    }

    #region WateringPlants

    /// <summary>
    /// Water the tiles in the rectangle
    /// </summary>
    /// <param name="rectangle">the rectangle's positions should be provided in pixel value not tiles. it will check if the tile contains the tile so you may need to add an extra tile to the height/width.</param>
    /// <param name="canDestroy"></param>
    public async Task WaterSelectPatches(Rectangle rectangle, bool canDestroy = false)
    {
        Group finalGroup = new();
        GroupedTiles groupedTiles = new();
        List<Group> list = groupedTiles.StartDirtCheck(BotBase.CurrentLocation).Result;
        
        foreach (var kvp in list) // go through groups of dirt
        {
            foreach (var itile in kvp.GetTiles())
            {
                Logger.Warning($"Running foreach: {rectangle}  {itile.Position}");
                if (!rectangle.Contains(itile.Position.ToVector2() * 64)) continue;
                // if (itile.Position != new Point(x, y)) continue;
                PlantTile tile = (itile as PlantTile)!;
            
                HoeDirt dirt = (tile.TerrainFeature as HoeDirt)!;
                if (dirt.crop is null || dirt.isWatered()) continue;

                if (finalGroup.GetTiles().Any(tile1 => tile1.Position == tile.Position))
                {
                    continue;
                }
        
                Logger.Info($"final group point: {tile.Position}");
                finalGroup.Add(tile);
                StardewClient.DebugTiles.GetOrAdd(tile,byte.MinValue);
            }
        }

        await UseToolOnGroup(finalGroup,new WateringCan(),-1,canDestroy);
    }
    
    /// <summary>
    /// Water all patches in this current location, This will only work if the bot is in the farm.
    /// </summary>
    public async Task WaterAllPatches(bool canDestroy = false)
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
                StardewClient.DebugTiles.GetOrAdd(tile,byte.MinValue);
            }
        }
        
        await UseToolOnGroup(finalGroup,new WateringCan(),-1,canDestroy);
        Logger.Info($"Ending watering");
    }
    
    private async Task UseToolOnGroup(Group group,Tool tool,int tileAmount = -1,bool canDestroy = false)
    {
        SwapItemHandler.SwapItem(tool.GetType(),"");
        
        PriorityQueue<PlantTile, int> plantTileQueue = new();
        if (tileAmount == -1)
        {
            tileAmount = group.GetTiles().Count;
        }
        
        if (BotBase.Farmer.CurrentTool is not WateringCan wateringCan)
        {
            Logger.Error($"Does not have a watering can");
            return;
        }
        Logger.Info($"tile amount: {tileAmount}");
        for (int i = 0; i < tileAmount; i++)
        {
            if (wateringCan.WaterLeft < 5)
            {
                Logger.Error($"Ran out of water");
                await RefillWateringCan();
            }

            plantTileQueue.Clear();
            // get new cost
            foreach (var tile in group.GetTiles())
            {
                PlantTile newTile = (PlantTile)tile;
                plantTileQueue.Enqueue(newTile,newTile.Cost);
            }
            
            PlantTile plantTile = plantTileQueue.Dequeue();
            group.Remove(plantTile);
            
            StardewClient.DebugTiles.Remove(plantTile,out _);

            if (plantTile.Position == BotBase.Farmer.TilePoint)
            {
                Logger.Error($"Same place in else");
                UseTool(-2,true);
                continue;
            }
            
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, plantTile.Position, out var direction, 4))
            {
                Logger.Info($"using neighbour if");
                if (direction == -1) continue;
                ChangeFacingDirection(direction);
                UseTool(direction);
            }
            else // pathfind to node
            {
                await TaskDispatcher.SwitchToMainThread();
                AlgorithmBase.IPathing pathing = new AStar.Pathing();
                pathing.BuildCollisionMap(BotBase.CurrentLocation);

                bool result = PathfindingHelper.Goto(new Goal.GoalPosition
                    (plantTile.Position.X, plantTile.Position.Y), canDestroy, false).Result;
                
                if (!result)
                {
                    Logger.Error($"Stack was empty");
                    UseTool(-2,true);
                    continue;
                }
                
                if (BotBase.Farmer.TilePoint == plantTile.Position) // will sometimes path-find to tile, this should not happen, I'm too lazy to fix this.
                {
                    UseTool(-2,true);
                    continue;
                }

                if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, plantTile.Position, out var pathDirection, 4))
                {
                    Logger.Error($"Tile was not in neighbours: {plantTile.Position}");
                    group.Add(plantTile);
                    continue;
                }
                ChangeFacingDirection(pathDirection);
                UseTool(pathDirection);
            }
        }
    }

    #endregion

    #region RefillWateringCan

    /// <summary>
    /// Refill watering can in this current location.
    /// </summary>
    public async Task RefillWateringCan(bool canDestroy = false)
    {
        GetNearestWaterTiles groupedTiles = new();
        Task<Group> group = groupedTiles.GetWaterGroup(BotBase.Farmer.TilePoint,BotBase.CurrentLocation);

        Group finalGroup = group.Result;
        finalGroup = GetNearestWaterTiles.Pathing.RemoveNonBorderWater(finalGroup,BotBase.CurrentLocation);
        foreach (var tile in finalGroup.GetTiles())
        {
            StardewClient.DebugTiles.GetOrAdd(tile,byte.MinValue);
        }
        await RefillWateringCan(finalGroup,canDestroy);
        Logger.Info($"Ending watering");
    }

    private async Task RefillWateringCan(Group group, bool canDestroy = false)
    {
        Logger.Info($"Running refill watering can: {group.GetTiles().Count}");
        SwapItemHandler.SwapItem(typeof(WateringCan), "");

        PriorityQueue<WaterTile, int> priorityQueue = new();
        foreach (var iTile in group.GetTiles())
        {
            if (iTile is not WaterTile water) continue;
            priorityQueue.Enqueue(water, water.Cost);
        }

        await TaskDispatcher.SwitchToMainThread();
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        
        if (BotBase.Farmer.CurrentTool is not WateringCan wateringCan) return;
        int startingWater = wateringCan.WaterLeft;
        var tile = priorityQueue.Dequeue();

        if (wateringCan.WaterLeft < startingWater) return;

        WaterTile? waterTile = GetNearestWaterTiles.Pathing.GetNeighbourWaterTile(tile, BotBase.CurrentLocation);
        if (waterTile is null) return;

        if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, waterTile.Position, out var direction, 4))
        {
            if (direction == -1) return;
            ChangeFacingDirection(direction);
            UseTool(direction);
            return;
        }

        bool foundPath = await PathfindingHelper.Goto(new Goal.GoalPosition(tile.Position.X, tile.Position.Y),
            canDestroy, false);
        await TaskDispatcher.SwitchToMainThread();
        
        if (!foundPath)
        {
            Logger.Error($"Stack was empty");
            UseTool(-2,true);
            return;
        }

        if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, waterTile.Position, out var pathDirection, 4)) return;
        
        ChangeFacingDirection(pathDirection);
        Logger.Error($"using bottom of useTool else");
        UseTool(pathDirection);
    }

    #endregion

    #region MakeFarmLand

    /// <summary>
    /// Get list of points for <see cref="MakeFarmLand"/>. This will be the dimensions of the farm
    /// </summary>
    /// <param name="rectangle">This is a rectangle that covers the tiles you want to get. This should be in pixel position.</param>
    /// <returns>List of <see cref="Point"/></returns>
    public List<GroundTile> CreateFarmLandTiles(Rectangle rectangle)
    {
        List<GroundTile> tiles = new();

        GameLocation location = BotBase.CurrentLocation;

        for (int x = 0; x < location.Map.DisplayWidth / 64; x++)
        {
            for (int y = 0; y < location.Map.DisplayHeight / 64; y++)
            {
                Vector2 tile = new Vector2(x, y);
                if (!rectangle.Contains(tile * 64)) continue;
                
                TerrainFeature? terrainFeature = null;
                ResourceClump? resourceClump = null;
                
                if (location.terrainFeatures.ContainsKey(tile))
                {
                    terrainFeature = location.terrainFeatures[tile];
                }
                List<ResourceClump> list = location.resourceClumps.ToList()
                    .FindAll(clump => clump.Tile == tile);
                if (list.Count > 0)
                {
                    resourceClump = list[0];
                }
                tiles.Add(new GroundTile(tile.ToPoint(),location,terrainFeature,resourceClump));   
            }
        }

        return tiles;
    }

    /// <param name="tiles">For this to work correctly this should be a list of <see cref="GroundTile"/></param>
    public void HoeFarmLand(List<ITile> tiles)
    {
        _tiles = tiles;
        _runAction = MakeFarmLand;
    }
    
    /// <summary>
    /// Make a portion of farm land based on the provided tiles, this will destroy any objects in the way.
    /// </summary>
    /// <param name="tiles">The tiles to turn into dirt, you can get these from <see cref="CreateFarmLandTiles"/></param>
    private async void MakeFarmLand(List<ITile> tiles)
    {
        // go back to update
        if (BotBase.Farmer.UsingTool) return;
        
        SwapItemHandler.SwapItem(typeof(Hoe),"");
        
        PriorityQueue<GroundTile, int> groundTileQueue = new();
        foreach (var tile in tiles)
        {
            if (tile is not GroundTile newTile) continue;
            groundTileQueue.Enqueue(newTile, newTile.Cost);
        }
        Logger.Info($"post queue");

        if (groundTileQueue.Count == 0) return;
        
        GroundTile groundTile = groundTileQueue.Dequeue();
        tiles.Remove(groundTile);
        
        if (groundTile.WaterTile) return;
        if (groundTile.TerrainFeature is HoeDirt) return;

        // everything after this is on a separate thread. Shouldn't be any issues with switch to main thread don't want to risk though.
        try
        {
            bool result = await SwapItemAndDestroy(groundTile.Position);
            await TaskDispatcher.SwitchToMainThread();

            Logger.Info($"result of object in way: {result}");
            if (!SwapItemHandler.SwapItem(typeof(Hoe), ""))
            {
                Logger.Error($"Cannot make farmland as there is no hoe in the bot's inventory.");
                return;
            }

            if (BotBase.Farmer.TilePoint == groundTile.Position)
            {
                Logger.Info($"first current tile");
                UseTool(-1, true);
                Logger.Info($"after use tool");
                return;
            }

            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, groundTile.Position, out var direction, 4))
            {
                if (direction == -1)
                {
                    return;
                }

                Logger.Info($"is in neighbours");
                ChangeFacingDirection(direction);
                UseTool(direction);
            }
            else
            {
                bool pathFound = await PathfindingHelper.Goto(new Goal.GetToTile(groundTile.X, groundTile.Y), false, false);
                await TaskDispatcher.SwitchToMainThread();

                // will sometimes path-find to tile, this should not happen, I'm too lazy to fix this.
                if (BotBase.Farmer.TilePoint == groundTile.Position)
                {
                    UseTool(-1, true);
                    return;
                }

                if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, groundTile.Position, out var pathDirection, 4))
                {
                    Logger.Info($"last use tool");
                    ChangeFacingDirection(pathDirection);
                    UseTool(pathDirection);
                
                }
                else
                {
                    if (!pathFound)
                    {
                        Logger.Error($"Stack was empty");
                        UseTool(-1, true);
                        return;
                    }
                
                    Logger.Error($"Tile was not in neighbours: {groundTile}");
                    tiles.Add(groundTile);
                }
            }
        }
        catch (Exception e)
        {
            Logger.Error($"{e}");
        }
    }

    #endregion

    #region RemoveObject

    /// <summary>
    /// Path-find and remove object at tile, current tool needs to be changed before this is called.
    /// </summary>
    /// <param name="tile">The tile of the object you want to destroy</param>
    public async Task RemoveObject(Point tile)
    {
        await SwapItemAndDestroy(tile);
        await PathDestroyObject(tile);
    }
    
    private async Task PathDestroyObject(Point tile) //TODO: can have weird pathing as it does not account for updated collision map. I think I removed due to crashing or something?
    {
        await TaskDispatcher.SwitchToMainThread();
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);

        Logger.Info($"starting removeObject public");

        if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, tile, out var direction, 4))
        {
            Logger.Info($"object is a neighbour");
            ChangeFacingDirection(direction);
            DestroyObjectType(tile);
        }
        else
        {
            bool pathFound = await PathfindingHelper.Goto(new Goal.GetToTile(tile.X, tile.Y),true, false);
            // cant put this here as some of the objects use a while loop that will pause the main game thread.  
            // await TaskDispatcher.SwitchToMainThread();
            
            if (!pathFound)
            {
                Logger.Error($"Stack was empty");
                DestroyObjectType(tile);
            }
            
            if (BotBase.Farmer.TilePoint == tile) // will sometimes path-find to tile, this should not happen, I'm too lazy to fix this.
            {
                DestroyObjectType(tile);
            }

            if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, tile, out var pathDirection, 4))
            {
                Logger.Error($"Tile was not in neighbours: {tile}");
                return;
            }
            ChangeFacingDirection(pathDirection);
            DestroyObjectType(tile);
        }
        AlgorithmBase.IPathing.CollisionMap.RemoveBlockedTile(tile.X,tile.Y); // should get around not being able to rebuild collision map
    }
    
    /// <summary>
    /// Get radius of startPoint as a square.
    /// </summary>
    /// <param name="startPoint">The Point to get tiles in radius of.</param>
    /// <param name="radius">radius you want to extend to</param>
    public List<GroundTile> RemoveObjectsInRadius(Point startPoint,int radius)
    {
        List<GroundTile> tiles = new();
        GameLocation location = BotBase.CurrentLocation;
        Point endPoint = new(startPoint.X + radius,startPoint.Y + radius);
        startPoint.X -= radius;
        startPoint.Y -= radius;
        for (int x = startPoint.X; x < endPoint.X; x++)
        {
            for (int y = startPoint.Y; y < endPoint.Y + radius; y++)
            {
                TerrainFeature? terrainFeature = null;
                ResourceClump? resourceClump = null;
                Object? obj = null;
                if (location.terrainFeatures.ContainsKey(new Vector2(x, y)))
                {
                    terrainFeature = location.terrainFeatures[new Vector2(x, y)];
                }
                List<ResourceClump> list = location.resourceClumps.ToList()
                    .FindAll(clump => clump.Tile == new Vector2(x,y));
                if (list.Count > 0)
                {
                    resourceClump = list[0];
                }

                foreach (var objDict in location.Objects)
                {
                    if (objDict.ContainsKey(new Vector2(x, y)))
                    {
                        obj = objDict[new Vector2(x, y)];
                    }
                }
                Logger.Info($"Adding tile at: {new Point(x,y)}");
                tiles.Add(new GroundTile(new Point(x,y),location,terrainFeature,resourceClump,obj));
            }
        }

        return tiles;
    }
    /// <summary>
    /// Remove objects that are in the provided dimensions
    /// </summary>
    /// <param name="rectangle">This is a rectangle that contains the tiles you want to remove the objects in.</param>
    public List<GroundTile> RemoveObjectsInDimension(Rectangle rectangle)
    {
        GameLocation location = BotBase.CurrentLocation;
        List<GroundTile> tiles = new();
        
        for (int x = 0; x < location.Map.DisplayWidth / 64; x++)
        {
            for (int y = 0; y < location.Map.DisplayHeight / 64; y++)
            {
                TerrainFeature? terrainFeature = null;
                ResourceClump? resourceClump = null;
                Object? obj = null;

                Vector2 tile = new Vector2(x, y);
                Logger.Info($"tile: {tile}   rect: {rectangle}   contains {rectangle.Contains(tile * 64)}");
                if (!rectangle.Contains(tile * 64)) continue;
                if (location.terrainFeatures.ContainsKey(new Vector2(x, y)))
                {
                    terrainFeature = location.terrainFeatures[new Vector2(x, y)];
                }
                List<ResourceClump> list = BotBase.CurrentLocation.resourceClumps.ToList()
                    .FindAll(clump => clump.Tile == new Vector2(x,y));
                if (list.Count > 0)
                {
                    resourceClump = list[0];
                }
                foreach (var objDict in location.Objects)
                {
                    if (objDict.ContainsKey(new Vector2(x, y)))
                    {
                        obj = objDict[new Vector2(x, y)];
                    }
                }
                Logger.Info($"Adding tile at: {new Point(x,y)}");
                tiles.Add(new GroundTile(new Point(x,y),location,terrainFeature,resourceClump,obj));
            }
        }

        return tiles;
    }

    /// <param name="tiles">These are tiles you should get from <see cref="RemoveObjectsInDimension"/> or <see cref="RemoveObjectsInRadius"/></param>
    public void RemoveObjects(List<ITile> tiles)
    {
        _runAction = RemoveObjectsInTiles;
        _tiles = tiles;
    }

    private async void RemoveObjectsInTiles(List<ITile> tiles)
    {
        Logger.Info($"remove object");
        if (BotBase.Farmer.UsingTool) return;
        Logger.Info($"post using tool");
     
        PriorityQueue<GroundTile, int> groundTileQueue = new();
        foreach (var tile in tiles)
        {
            if (tile is not GroundTile gt) continue;
            groundTileQueue.Enqueue(gt,gt.Cost);
        }

        if (groundTileQueue.Count == 0) return;
        GroundTile groundTile = groundTileQueue.Dequeue();
        tiles.Remove(groundTile);
        
        if (groundTile.WaterTile || groundTile.TerrainFeature is HoeDirt) return;
        if (groundTile.TerrainFeature is null && groundTile.ResourceClump is null && groundTile.Obj is null) return;
        
        Logger.Info($"running object in way at: {groundTile.Position}");
        try
        {
            bool result = await SwapItemAndDestroy(groundTile.Position);
            Logger.Info($"result of swap item and destroy: {result}");
        }
        catch (Exception e)
        {
            Logger.Error($"{e}");
        }
    }

    /// <summary>
    /// Destroy the object at the provided tile, this allows for destroying objects
    /// based on rules set in the destroy object type classes.
    /// </summary>
    /// <param name="tile"></param>
    /// <returns>return true, if you can destroy something at the tile else false</returns>
    private static bool DestroyObjectType(Point tile)
    {
        GameLocation location = BotBase.CurrentLocation;
        tile = (tile.ToVector2() * 64).ToPoint();
        Logger.Info($"running get object type");
        var containFeatures =
            location.terrainFeatures.Values.Where(terrFeat => terrFeat.getBoundingBox().Contains(tile)).ToList();
        List<LargeTerrainFeature> largeFeatures = location.largeTerrainFeatures
            .Where(feature => feature.getBoundingBox().Contains(tile)).ToList();
        
        if (containFeatures.Count > 0 || largeFeatures.Count > 0)
        {
            var feature = containFeatures.Count > 0 ? containFeatures[0] : largeFeatures[0];
            Logger.Info($"destroying: {feature}");
            DestroyTerrainFeature.Destroy(feature);
            return true;
        }

        List<ResourceClump> clumps = location.resourceClumps.ToList()
            .FindAll(clump => clump.getBoundingBox().Contains(tile));
        if (clumps.Any())
        {
            Logger.Info($"destroying: {clumps[0].textureName}");
            DestroyResourceClump.Destroy(clumps[0]);
            return true;
        }

        foreach (var objDict in location.Objects)
        {
            var objs = objDict.Where(kvp => kvp.Value.GetBoundingBox().Contains(tile)).ToList();
            if (!objs.Any()) return false;
            Logger.Info($"destroying: {objs[0].Value.DisplayName}");
            DestroyLitterObject.Destroy(objs[0].Value);
            return true;
        }

        return false;
    }

    #endregion

    #region PlaceObject

    /// <summary>
    /// This is to place the current held item, defined by <see cref="Farmer.ActiveObject"/>,
    /// there is no validation on if the item can be placed by this function. This will reduce the item's stack by one though
    /// </summary>
    /// <param name="tile">The tile that you want to place it on, this will be converted by the function to be a pixel coordinate</param>
    /// <returns>Returns whether the object should be or was added to the location.</returns>
    public bool PlaceCurrentItem(Point tile)
    {
        Point pixelTile = (tile.ToVector2() * 64).ToPoint();
        if (BotBase.Farmer.ActiveObject.placementAction(BotBase.CurrentLocation, pixelTile.X, pixelTile.Y,
                BotBase.Farmer))
        {
            BotBase.Farmer.reduceActiveItemByOne();
            return true;
        }
        
        return false;
    }

    // This mainly exists because of character controller recalculate path if object in next tile
    private bool PlaceCurrentAndModifyMap(Point tile)
    {
        bool isPassable = BotBase.Farmer.ActiveObject.isPassable();
        if (PlaceCurrentItem(tile))
        {
            if (!isPassable) AlgorithmBase.IPathing.CollisionMap.AddBlockedTile(tile.X,tile.Y);
            return true;
        }

        return false;
    }

    private async Task<bool> PlaceObject(PlaceTile tile, bool checkPass = true)
    {
        Logger.Info($"starting place object");
        if (tile.ItemToPlace is null) return false;
        
        SwapItemHandler.SwapObject(tile.ItemToPlace);
        Logger.Info($"object: {BotBase.Farmer.ActiveObject.Name}");
        
        if (!BotBase.CurrentLocation.CanItemBePlacedHere(tile.Position.ToVector2(),tile.ItemToPlace.isPassable()))
        {
            return false;
        }
        
        // TODO: check top of later fixes for reason for this
        if (checkPass && !tile.ItemToPlace.isPassable()) return false;
        
        if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, tile.Position, out var direction, 4))
        {
            Logger.Info($"object is a neighbour");
            ChangeFacingDirection(direction);
            return PlaceCurrentAndModifyMap(tile.Position);
        }

        bool result = await PathfindingHelper.Goto(new Goal.GetToTile(tile.X, tile.Y), false, false);
        
        if (!result)
        {
            Logger.Error($"Stack was empty");
        }
        
        // I don't know if we need this here but, I want to make sure.
        if (tile.TerrainFeature is HoeDirt dirt && tile.ItemToPlace.Category == -74 && !dirt.canPlantThisSeedHere(tile.ItemToPlace.ItemId)) return false;
        
        // will sometimes path-find to tile.
        if (BotBase.Farmer.TilePoint == tile.Position)
        {
            return tile.TerrainFeature is HoeDirt && PlaceCurrentAndModifyMap(tile.Position);
        }

        if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, tile.Position, out var pathDirection, 4))
        {
            Logger.Error($"Tile was not in neighbours: {tile}");
            return false;
        }

        ChangeFacingDirection(pathDirection);
        return PlaceCurrentAndModifyMap(tile.Position);
    }

    /// <summary>
    /// Place a <see cref="Object"/> in a radius from the start Tile
    /// </summary>
    /// <param name="startTile"></param>
    /// <param name="itemToPlace">An instance of the item you want to place, you should probably get this from the bot's inventory.</param>
    /// <param name="radius"></param>
    public List<ITile> PlaceObjectsInRadius(Point startTile,Object itemToPlace,int radius)
    {
        List<PlaceTile> tiles = new();
        GameLocation location = BotBase.CurrentLocation;
        Point endPoint = new(startTile.X + radius,startTile.Y + radius);
        startTile.X -= radius;
        startTile.Y -= radius;
        for (int x = startTile.X; x < endPoint.X; x++)
        {
            for (int y = startTile.Y; y < endPoint.Y + radius; y++)
            {
                TerrainFeature? terrainFeature = null;
                ResourceClump? resourceClump = null;
                Object? obj = null;
                if (location.terrainFeatures.ContainsKey(new Vector2(x, y)))
                {
                    terrainFeature = location.terrainFeatures[new Vector2(x, y)];
                }
                List<ResourceClump> list = location.resourceClumps.ToList()
                    .FindAll(clump => clump.Tile == new Vector2(x,y));
                if (list.Count > 0)
                {
                    resourceClump = list[0];
                }

                foreach (var objDict in location.Objects)
                {
                    if (objDict.ContainsKey(new Vector2(x, y)))
                    {
                        obj = objDict[new Vector2(x, y)];
                    }
                }
                
                if (!itemToPlace.canBePlacedHere(BotBase.CurrentLocation,new(x,y))) continue;
                Logger.Info($"Adding tile at: {new Point(x,y)} {itemToPlace.Name}");
                tiles.Add(new PlaceTile(new Point(x,y),location,terrainFeature,resourceClump,obj,itemToPlace));
            }
        }

        return new List<ITile>(tiles);
    }

    /// <summary>
    /// This can be used to place objects at the provided tiles.
    /// </summary>
    /// <param name="tiles">These should be <see cref="PlaceTile"/> you can get these from <see cref="PlaceObjectsInRadius"/>.
    /// If you are making your own tiles, you should make sure the item can be placed at the position.</param>
    /// <param name="checkPass">This will check if the object you are placing is passable.</param>
    public void PlaceObjects(List<ITile> tiles, bool checkPass = true)
    {
        _checkPass = checkPass;
        _tiles = tiles;
        _runAction = PlaceObjectsAtTiles;
    }

    private bool _checkPass;
    /// <summary>
    /// Place the object at the provided tiles.
    /// </summary>
    /// <param name="tiles">Will place the object in <see cref="PlaceTile.ItemToPlace"/> if there is none or that tile
    /// is blocked, that tile will be skipped.
    /// </param>
    private async void PlaceObjectsAtTiles(List<ITile> tiles)
    {
        if (BotBase.Farmer.UsingTool) return;

        try
        {
            await TaskDispatcher.SwitchToMainThread();
            AlgorithmBase.IPathing pathing = new AStar.Pathing();
            pathing.BuildCollisionMap(BotBase.CurrentLocation);
        }
        catch (Exception e)
        {
            Logger.Error($"{e}");
            return;
        }
        
        PriorityQueue<PlaceTile, int> groundTileQueue = new();

        // reorder priority queue on new cost
        foreach (var tile in tiles.Where(tile => tile is PlaceTile { WaterTile: false, ItemToPlace: not null }))
        {
            if (tile is not PlaceTile pt) continue;
            groundTileQueue.Enqueue(pt, pt.Cost);
        }

        if (groundTileQueue.Count == 0) return;
        PlaceTile placeTile = groundTileQueue.Dequeue();
        tiles.Remove(placeTile);

        if (placeTile.ItemToPlace is null) return;
        
        // if the tile is already blocked
        if (placeTile.ResourceClump is not null || placeTile.Obj is not null) return;
        
        // if the item to place is seed or fertilizer and the tile does not support the seed or fertilizer return
        // these are separated to stop it from being as ugly
        if (placeTile.ItemToPlace.Category == -74 && (placeTile.TerrainFeature is not HoeDirt || (placeTile.TerrainFeature is HoeDirt dirt 
                && !dirt.canPlantThisSeedHere(Crop.ResolveSeedId(placeTile.ItemToPlace.ItemId,BotBase.CurrentLocation)))))
        {
            Logger.Info($"skipping: {placeTile.Position}");
            return;
        }

        if (placeTile.ItemToPlace.Category == -19 && (placeTile.TerrainFeature is not HoeDirt ||
            (placeTile.TerrainFeature is HoeDirt d &&!d.CanApplyFertilizer(placeTile.ItemToPlace.ItemId))))
        {
            Logger.Info($"skipping: {placeTile.Position}");
            return;
        }
        
        Logger.Info($"placing at {placeTile.Position}");
        try
        {
            bool result = await PlaceObject(placeTile,_checkPass);
            Logger.Info($"result of object in way: {result}    false is bad :(");
        }
        catch (Exception e)
        {
            Logger.Error($"{e}");
        }
    }

    #endregion

    private static bool _destroying;
    /// <summary>
    /// change item and path-find then destroy object that is on specified tile.
    /// </summary>
    /// <returns></returns>
    private async Task<bool> SwapItemAndDestroy(Point tile,bool destroy = true)
    {
        if (destroy) _destroying = true;
        if (TerrainFeatureToolSwap.Swap(tile)) // we also handle bushes here
        {
            Logger.Info($"terrain feature swap");
            if (!destroy) return true;
            
            await PathDestroyObject(tile);
            _destroying = false;
            return true;
        }

        if (ResourceClumpToolSwap.Swap(tile))
        {
            Logger.Info($"resource set");
            if (!destroy) return true;
            
            await PathDestroyObject(tile);
            _destroying = false;
            return true;
        }

        if (LitterObjectToolSwap.Swap(tile))
        {
            Logger.Info($"object set");
            if (!destroy) return true;
            
            await PathDestroyObject(tile);
            _destroying = false;
            return true;
        }
        
        return false;
    }
}