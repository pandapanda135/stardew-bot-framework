using System.Net;
using System.Xml.Resolvers;
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;
using StardewBotFramework.Source.ObjectDestruction;
using StardewBotFramework.Source.ObjectToolSwaps;
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
        // BotBase.Farmer.lastClick = points.ToList()[direction].ToVector2() * 64;
        // BotBase.Farmer.BeginUsingTool();
    }

    #region WateringPlants

    public async Task WaterSelectPatches(int leftX,int topY,int rightX,int bottomY)
    {
        Group finalGroup = new();
        GroupedTiles groupedTiles = new();
        Task<List<Group>> list = groupedTiles.StartDirtCheck(BotBase.CurrentLocation);
        
        for (int x = leftX; x < rightX; x++)
        {
            for (int y = topY; y < bottomY; y++)
            {
                foreach (var kvp in list.Result) // go through groups of dirt
                {
                    foreach (var itile in kvp.GetTiles())
                    {
                        if (itile.Position != new Point(x, y)) continue;
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
            }
        }

        await UseToolOnGroup(finalGroup,new WateringCan());
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
        SwapItemHandler.SwapItem(tool.GetType(),"");
        
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        PriorityQueue<PlantTile, int> plantTileQueue = new();
        if (tileAmount == -1)
        {
            tileAmount = group.GetTiles().Count;
        }
        Logger.Info($"tile amount: {tileAmount}");
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
            
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, plantTile.Position, out var direction, 4))
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
                
                CharacterController.StartMoveCharacter(path);
                while (CharacterController.IsMoving()) continue; // this is not async
                
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
            
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, waterTile.Position, out var direction, 4))
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
            
            CharacterController.StartMoveCharacter(path);

            while (CharacterController.IsMoving()) continue; // this is not async
            
            if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, waterTile.Position, out var pathDirection, 4)) continue;
            ChangeFacingDirection(pathDirection);
            Logger.Error($"using bottom of useTool else");
            UseTool(pathDirection,false);
            break;
        }
    }

    #endregion

    #region MakeFarmLand

    /// <summary>
    /// Get list of points for <see cref="MakeFarmLand"/>. This will be the dimensions of the farm
    /// </summary>
    /// <param name="startX">X of the farm land you want to make, this should be the top left of a rectangle.</param>
    /// <param name="startY">Y of the farm land you want to make, this should be the top left of a rectangle.</param>
    /// <param name="endX">the end X of the farm land you want to make, this should be the bottom right of a rectangle.</param>
    /// <param name="endY">the end Y of the farm land you want to make, this should be the bottom right of a rectangle.</param>
    /// <returns>List of <see cref="Point"/></returns>
    public List<GroundTile> CreateFarmLandTiles(int startX,int startY,int endX,int endY)
    {
        List<GroundTile> tiles = new();
        for (int x = startX; x < endX + 1; x++)
        {
            for (int y = startY; y < endY + 1; y++)
            {
                GameLocation location = BotBase.CurrentLocation;
                TerrainFeature? terrainFeature = null;
                ResourceClump? resourceClump = null;
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
                tiles.Add(new GroundTile(new Point(x,y),BotBase.CurrentLocation,terrainFeature,resourceClump));
            }
        }

        return tiles;
    }
    
    /// <summary>
    /// Make a portion of farm land based on the provided tiles, this will destroy any objects in the way.
    /// </summary>
    /// <param name="tiles">The tiles to turn into dirt, you can get these from <see cref="CreateFarmLandTiles"/></param>
    public async Task MakeFarmLand(List<GroundTile> tiles)
    {
        SwapItemHandler.SwapItem(typeof(Hoe),"");
        
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        PriorityQueue<GroundTile, int> groundTileQueue = new();
        int tileAmount = tiles.Count;
        for (int i = 0; i < tileAmount; i++)
        {
            groundTileQueue.Clear();
            foreach (var tile in tiles)
            {
                GroundTile newTile = tile;
                groundTileQueue.Enqueue(newTile,newTile.Cost);
            }
            GroundTile groundTile = groundTileQueue.Dequeue();
            tiles.Remove(groundTile);

            while (BotBase.Farmer.UsingTool)
            {
            }
            
            if (groundTile.WaterTile) continue;
            if (groundTile.TerrainFeature is HoeDirt) continue;
            
            bool result = await SwapItemAndDestroy(groundTile.Position,true);
            
            Logger.Info($"result of object in way: {result}");
            if (BotBase.Farmer.CurrentTool is not Hoe hoe)
            {
                SwapItemHandler.SwapItem(typeof(Hoe),"");
            }
            
            if (BotBase.Farmer.TilePoint == groundTile.Position)
            {
                Logger.Info($"first current tile");
                UseTool(-1,true);
                continue;
            }
            
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, groundTile.Position, out var direction, 4))
            {
                if (direction == -1)
                {
                    continue;
                }
                Logger.Info($"is in neighbours");
                ChangeFacingDirection(direction);
                UseTool(direction);
            }
            else
            {
                PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);
                
                Stack<PathNode> path = await pathing.FindPath(start,new Goal.GetToTile(groundTile.X,groundTile.Y),BotBase.CurrentLocation,10000);

                if (path == new Stack<PathNode>())
                {
                    Logger.Error($"Stack was empty");
                    UseTool(-1,true);
                    continue;
                }
                
                CharacterController.StartMoveCharacter(path);

                while (CharacterController.IsMoving()) continue; // this is not async
                
                if (BotBase.Farmer.TilePoint == groundTile.Position) // will sometimes path-find to tile, this should not happen, I'm too lazy to fix this.
                {
                    UseTool(-1,true);
                    continue;
                }

                if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, groundTile.Position, out var pathDirection, 4))
                {
                    Logger.Error($"Tile was not in neighbours: {groundTile}");
                    tiles.Add(groundTile);
                    continue;
                }
                Logger.Info($"last use tool");
                ChangeFacingDirection(pathDirection);
                UseTool(pathDirection,false);
            }
        }
    }

    #endregion

    #region RemoveObject

    /// <summary>
    /// Remove object at tile, current tool needs to be changed before this is called.
    /// </summary>
    /// <param name="tile">The tile of the object you want to destroy</param>
    public async Task RemoveObject(Point tile) //TODO: can have weird pathing as it does not account for updated collision map. I think I removed due to crashing or something?
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        // pathing.BuildCollisionMap(BotBase.CurrentLocation);

        Logger.Info($"starting removeObject public");
        PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);

        if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, tile, out var direction, 4))
        {
            Logger.Info($"object is a neighbour");
            ChangeFacingDirection(direction);
            GetObjectType(tile);
        }
        else
        {
            Stack<PathNode> path = await pathing.FindPath(start, new Goal.GetToTile(tile.X, tile.Y), BotBase.CurrentLocation, 10000);
            if (path == new Stack<PathNode>())
            {
                Logger.Error($"Stack was empty");
                GetObjectType(tile);
            }

            CharacterController.StartMoveCharacter(path);

            while (CharacterController.IsMoving()) continue; // this is not async

            if (BotBase.Farmer.TilePoint == tile) // will sometimes path-find to tile, this should not happen, I'm too lazy to fix this.
            {
                GetObjectType(tile);
            }

            if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, tile, out var pathDirection, 4))
            {
                Logger.Error($"Tile was not in neighbours: {tile}");
                return;
            }
            ChangeFacingDirection(pathDirection);
            GetObjectType(tile);
        }
    }
    
    /// <summary>
    /// Get radius of startPoint as a square.
    /// </summary>
    /// <param name="startPoint">The Point to get tiles in radius of.</param>
    /// <param name="radius">radius you want to extend to</param>
    public async Task RemoveObjectsInRadius(Point startPoint,int radius)
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

        await RemoveObjectsInTiles(tiles);
    }
    /// <summary>
    /// Remove objects that are in the provided dimensions
    /// </summary>
    /// <param name="startX">This should be thought of as the most left edge of a square</param>
    /// <param name="startY">This should be the top of the square</param>
    /// <param name="endX">This should be thought of as the most right edge of a square</param>
    /// <param name="endY">This should be the bottom of the square</param>
    public async Task RemoveObjectsInDimension(int startX, int startY, int endX, int endY)
    {
        GameLocation location = BotBase.CurrentLocation;
        List<GroundTile> tiles = new();
        for (int x = startX; x < endX + 1; x++)
        {
            for (int y = startY; y < endY + 1; y++)
            {
                TerrainFeature? terrainFeature = null;
                ResourceClump? resourceClump = null;
                Object? obj = null;
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

        await RemoveObjectsInTiles(tiles);
    }

    private async Task RemoveObjectsInTiles(List<GroundTile> tiles)
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        PriorityQueue<GroundTile, int> groundTileQueue = new();
        int tileAmount = tiles.Count;
        Logger.Info($"tile amount: {tileAmount}");
        for (int i = 0; i < tileAmount; i++)
        {
            Logger.Info($"running for loop");
            groundTileQueue.Clear();
            foreach (var tile in tiles)
            {
                GroundTile newTile = tile;
                groundTileQueue.Enqueue(newTile,newTile.Cost);
            }
            GroundTile groundTile = groundTileQueue.Dequeue();
            tiles.Remove(groundTile);
            
            while (BotBase.Farmer.UsingTool)
            {
            }
            
            if (groundTile.WaterTile) continue;
            if (groundTile.TerrainFeature is HoeDirt) continue;
            if (groundTile.TerrainFeature is null && groundTile.ResourceClump is null && groundTile.Obj is null) continue;
            
            Logger.Info($"running object in way at: {groundTile.Position}");
            bool result = await SwapItemAndDestroy(groundTile.Position,true);
            
            Logger.Info($"result of object in way: {result}");
        }
    }

    private static async Task PrivateDestroyObject(Point tile)
    {
        Logger.Info($"destroy object private");
        ToolHandling toolHandling = new();
        await toolHandling.RemoveObject(tile);
    }

    private static bool GetObjectType(Point tile)
    {
        GameLocation location = BotBase.CurrentLocation;
        Logger.Info($"running get object type");
        if (location.terrainFeatures.Keys.Contains(tile.ToVector2()) || location.largeTerrainFeatures.Count(feature => feature.getBoundingBox().Contains(tile.ToVector2())) > 0)
        {
            if (BotBase.CurrentLocation.terrainFeatures.ContainsKey(tile.ToVector2()))
            {
                Logger.Info($"terrain feature destroy");
                DestroyTerrainFeature.Destroy(location.terrainFeatures[tile.ToVector2()]);
                return true;
            }
            Logger.Info($"large terrain destroy");
            List<LargeTerrainFeature> list = location.largeTerrainFeatures.ToList()
                .FindAll(feature => feature.Tile == tile.ToVector2());
            DestroyTerrainFeature.Destroy(list[0]);
            return true;
        }

        if (location.resourceClumps.Count(clump => clump.Tile == tile.ToVector2()) > 0)
        {
            List<ResourceClump> list = location.resourceClumps.ToList()
                .FindAll(clump => clump.Tile == tile.ToVector2());
            DestroyResourceClump.Destroy(list[0]);
            return true;
        }

        foreach (var objDict in location.Objects)
        {
            if (objDict.ContainsKey(tile.ToVector2()))
            {
                Object obj = objDict[tile.ToVector2()];
                DestroyLitterObject.Destroy(obj);
                return true;
            }
        }

        return false;
    }

    #endregion

    /// <summary>
    /// change item and path-find then destroy object that is on specified tile.
    /// </summary>
    /// <returns></returns>
    private static async Task<bool> SwapItemAndDestroy(Point tile,bool modifyCollisionMap = false,bool destroy = true)
    {
        if (TerrainFeatureToolSwap.Swap(tile,false)) // we also handle bushes here
        {
            Logger.Info($"terrain feature swap");
            TerrainFeatureToolSwap.Swap(tile,false);
            AlgorithmBase.IPathing.collisionMap.RemoveBlockedTile(tile.X, tile.Y);
            await PrivateDestroyObject(tile);
            return true;
        }

        if (ResourceClumpToolSwap.Swap(tile))
        {
            Logger.Info($"resource set");
            ResourceClumpToolSwap.Swap(tile);
            AlgorithmBase.IPathing.collisionMap.RemoveBlockedTile(tile.X,tile.Y);
            await PrivateDestroyObject(tile);
            return true;
        }

        if (LitterObjectToolSwap.Swap(tile))
        {
            Logger.Info($"object set");
            LitterObjectToolSwap.Swap(tile);
            AlgorithmBase.IPathing.collisionMap.RemoveBlockedTile(tile.X,tile.Y);
            await PrivateDestroyObject(tile);
            return true;
        }
        
        return false;
    }
}