using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class GetNearestWaterTiles : AlgorithmBase
{
    private static readonly Queue<Point> Frontier = new();

    private static readonly Stack<Point> ClosedList = new();

    private static readonly Dictionary<Point,WaterTiles.WaterTileData> WaterTiles = new();
    private static readonly List<WaterTile> UsedWaterTiles = new();

    public async Task<Group> GetWaterGroup(Point startPoint, GameLocation location)
    {
        Stack<WaterTile> tiles = await Pathing.GetWater(startPoint, location, 10000);

        Group group = new();
        foreach (var waterTile in tiles)
        {
            group.Add(waterTile);
        }

        Group finalGroup = new();
        Logger.Info($"count of result: {group.GetTiles().Count}");
        foreach (var iTile in group.GetTiles()) // go through group of water
        {
            WaterTile tile = (iTile as WaterTile)!;
            
            // this is because I'm a horrible programmer that's too dumb to find a better way to do this. continue if duplicate tile.
            IEnumerable<ITile> ieTiles = finalGroup.GetTiles().Where(tile1 => tile1.Position == tile.Position);
            if (ieTiles.Any())
            {
                continue;
            }
            
            Logger.Info($"final group point: {tile.Position}");
            finalGroup.Add(tile);
        }
        
        return finalGroup;
    }
    
    public class Pathing : IPathing
    {
        public Task<Stack<PathNode>> FindPath(PathNode startPoint, Goal goal, GameLocation location, int limit, bool canDestroy = false)
        {
            throw new NotImplementedException();
        }
        
        public static async Task<Stack<WaterTile>> GetWater(Point startPoint,GameLocation location, int limit)
        {
            ClearVariables();

            int maxWidth = location.Map.DisplayWidth / 64;
            int maxHeight = location.Map.DisplayHeight / 64;
            Stack<WaterTile> correctPath = new();
            Stack<WaterTile> allWaterTiles = new();
            for (int x = 0; x < maxWidth; x++)
            {
                for (int y = 0; y < maxHeight; y++)
                {
                    WaterTiles.WaterTileData tile = location.waterTiles.waterTiles[x, y];

                    if (tile.isWater)
                    {
                        WaterTiles.Add(new Point(x,y),tile);
                        correctPath = await Task.Run(() => RunBreadthFirstWater(new Point(x,y), location, limit));
                        UsedWaterTiles.AddRange(correctPath);
                        foreach (var waterTile in correctPath)
                        {
                            allWaterTiles.Push(waterTile);
                        }
                    }
                }
            }
            
            if (allWaterTiles.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<WaterTile>();
            }
            _usedStartPoint.Clear();
            return allWaterTiles;
        }

        private static readonly Stack<WaterTile> _WaterTileGroup = new();
        public static List<Point> _usedStartPoint = new();
        private static Stack<WaterTile> RunBreadthFirstWater(Point startTile, GameLocation location ,int limit)
        {
            var locationWater = WaterTiles;
            int runs = 0;
            
            _usedStartPoint.Add(startTile);
            Frontier.Enqueue(startTile);
            ClosedList.Push(startTile);
    
            while (Frontier.Count > 0)
            {
                Logger.Info($"running while");
                if (runs > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }
                
                Point current = Frontier.Dequeue();

                if (_usedStartPoint.Contains(current) && runs != 0) return new();
    
                // We reduce by 1 to avoid pathfinding going along the side of the map
                if (current.X > location.Map.DisplayWidth / Game1.tileSize - 1 ||
                    current.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 ||
                    current.X < 0 || current.Y < 0)
                {
                    Logger.Info(
                        $"Blocking this tile: {current.X},{current.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                    continue;
                }
    
                if (ClosedList.Contains(current) && current != startTile) continue;
                
                if (!locationWater.Keys.Contains(current)) continue;
                
                ClosedList.Push(current);
                
                Queue<Point> neighbours = IPathing.Graph.GroupNeighbours(current,7);
                foreach (var node in neighbours.Where(node => !ClosedList.Contains(node)))
                {
                    Logger.Info($"in foreach this is node: {node.X},{node.Y}");
                    WaterTile waterTile = new WaterTile(current,location);
                    if (UsedWaterTiles.Contains(waterTile)) continue;
                    Frontier.Enqueue(new Point(node.X,node.Y));
                    _WaterTileGroup.Push(waterTile);
                }
    
                runs++;
            }

            return _WaterTileGroup;
        }

        internal static Group RemoveNonBorderWater(Group group,GameLocation location)
        {
            var waterTiles = location.waterTiles.waterTiles;
            Group newGroup = new();
            foreach (var tile in group.GetTiles())
            {
                foreach (var groupNeighbour in IPathing.Graph.GroupNeighbours(tile.Position,3))
                {
                    try
                    {
                        if (!waterTiles[groupNeighbour.X, groupNeighbour.Y].isWater)
                        {
                            if (!newGroup.Contains(tile))
                            {
                                newGroup.Add(new WaterTile(new Point(groupNeighbour.X,groupNeighbour.Y),location));
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        break;
                    }
                }

            }

            return newGroup;
        }

        internal static WaterTile? GetNeighbourWaterTile(WaterTile tile,GameLocation location)
        {
            var waterTiles = location.waterTiles.waterTiles;

            Queue<Point> tiles = IPathing.Graph.GroupNeighbours(tile.Position, 3);

            foreach (var point in tiles)
            {
                if (waterTiles[point.X, point.Y].isWater)
                {
                    return new WaterTile(point, location);
                }
            }

            return null;
        }
        
        private static void ClearVariables()
        {
            Frontier.Clear();
            _WaterTileGroup.Clear();
            WaterTiles.Clear();
            ClosedList.Clear();
        }
        
    }
}