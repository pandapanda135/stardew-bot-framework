using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class GetNearestWaterTiles : AlgorithmBase
{
    private static readonly Queue<Point> _frontier = new();

    private static readonly Stack<Point> _closedList = new();

    private static Dictionary<Point,WaterTiles.WaterTileData> _waterTiles = new();
    private static readonly Stack<Point> _tileGroup = new();
    private static List<WaterTile> _usedWaterTiles = new();

    public async Task<Group> GetWaterGroup(Point startPoint, GameLocation location)
    {
        Stack<WaterTile> tiles = await Pathing.GetWater(startPoint, location, 10000);

        Group group = new();
        foreach (var waterTile in tiles)
        {
            group.Add(waterTile);
        }

        return group;
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
                        correctPath = await Task.Run(() => RunBreadthFirstWater(startPoint, location, limit));
                        _usedWaterTiles.AddRange(correctPath);
                        _waterTiles.Add(new Point(x,y),tile);
                        foreach (var waterTile in correctPath)
                        {
                            allWaterTiles.Push(waterTile);
                        }
                    }
                }
            }
            
            if (_waterTiles.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<WaterTile>();
            }
            return allWaterTiles;
        }

        private static readonly Stack<WaterTile> _WaterTileGroup = new();
        public static List<Point> _usedStartPoint = new();
        private static Stack<WaterTile> RunBreadthFirstWater(Point startTile, GameLocation location ,int limit) //TODO: move checks to own function
        {
            var locationWater = _waterTiles;
            int runs = 0;
            
            _usedStartPoint.Add(startTile);
            _frontier.Enqueue(startTile);
            _closedList.Push(startTile);
    
            while (_frontier.Count > 0)
            {
                Logger.Info($"running while");
                if (runs > limit)
                {
                    Logger.Error($"Breaking due to limit");
                    break;
                }
                
                Point current = _frontier.Dequeue();

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
    
                if (_closedList.Contains(current) && current != startTile) continue;
                
                if (!locationWater.Keys.Contains(current)) continue;
                
                _closedList.Push(current);
                
                Queue<Point> neighbours = IPathing.Graph.GroupNeighbours(current,7);
                foreach (var node in neighbours.Where(node => !_closedList.Contains(node)))
                {
                    Logger.Info($"in foreach this is node: {node.X},{node.Y}");
                    WaterTile waterTile = new WaterTile(current,location);
                    if (_usedWaterTiles.Contains(waterTile)) continue;
                    _frontier.Enqueue(new Point(node.X,node.Y));
                    _WaterTileGroup.Push(waterTile);
                }
    
                runs++;
            }

            return _WaterTileGroup;
        }
        
        
        private static void ClearVariables()
        {
            _frontier.Clear();
            _WaterTileGroup.Clear();
            _closedList.Clear();
            _tileGroup.Clear();
        }
        
    }
}