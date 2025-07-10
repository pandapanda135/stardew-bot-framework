using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class BreadthFirstGrouping : AlgorithmBase
{
    private static readonly Queue<Point> _frontier = new();

    private static readonly Stack<Point> _closedList = new();
    private static readonly Stack<Point> _tileGroup = new();
    
    public class Pathing : IPathing
    {
        public Task<Stack<PathNode>> FindPath(PathNode startPoint, Goal goal, GameLocation location, int limit, bool canDestroy = false)
        {
            throw new NotImplementedException();
        }

        // make this run asynchronously so we can await it when using bot as don't want running actions when pathfinding or just make character controller async (as don't want moving while dropping items as an example)
        public async Task<Stack<Point>> GetGroup(Point startPoint, string property, GameLocation location, int limit)
        {
            ClearVariables();

            Stack<Point> correctPath = await Task.Run(() => RunBreadthFirstProperty(startPoint, property, location, limit));
                
            if (correctPath.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<Point>();
            }
            return correctPath;
        }

        public async Task<Stack<HoeDirt>> GetTerrainGroup(Point startPoint,
            GameLocation location, int limit)
        {
            ClearVariables();

            Stack<HoeDirt> correctPath = await Task.Run(() => RunBreadthFirstDirt(startPoint, location, limit));
                
            if (correctPath.Count == 0)
            {
                Logger.Error($"Rebuild path returned empty stack");
                return new Stack<HoeDirt>();
            }
            return correctPath;
        }

        private static readonly Stack<HoeDirt> _terrainTileGroup = new();
        public static List<Point> _usedStartPoint = new();
        private Stack<HoeDirt> RunBreadthFirstDirt(Point startTile, GameLocation location, int limit) //TODO: move checks to own function
        {
            var locationTerrain = location.terrainFeatures;
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

                if (_usedStartPoint.Contains(current) && runs > 0) return new();
    
                // We reduce by 1 to avoid pathfinding going along the side of the map
                if (current.X > location.Map.DisplayWidth / Game1.tileSize - 1 ||
                    current.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 ||
                    current.X < 0 || current.Y < 0)
                {
                    Logger.Info(
                        $"Blocking this tile: {current.X},{current.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                    continue;
                }
    
                if (_closedList.Contains(current) && current != startTile)
                {
                    continue;
                }

                if (!locationTerrain.Keys.Contains(current.ToVector2())) continue;

                
                _closedList.Push(current);

                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(new PathNode(current.X,current.Y,null));
                foreach (var node in neighbours.Where(node => !_closedList.Contains(new Point(node.X,node.Y)) && !IPathing.collisionMap.IsBlocked(node.X,node.Y)))
                {
                    Logger.Info($"in foreach this is node: {node.X},{node.Y}");
                    _frontier.Enqueue(new Point(node.X,node.Y));
                    _terrainTileGroup.Push((HoeDirt)locationTerrain[current.ToVector2()]);
                }
    
                runs++;
            }

            return _terrainTileGroup;
        }
        
        private Stack<Point> RunBreadthFirstProperty(Point startTile,string property, GameLocation location, int limit)
        {
            int runs = 0;
            
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
                
                Point current = _frontier.Dequeue(); // issue with going through same tile multiple times (This might actually be fine according to some stuff I've read I'll keep it here though)
    
                // We reduce by 1 to avoid pathfinding going along the side of the map
                if (current.X > location.Map.DisplayWidth / Game1.tileSize - 1 ||
                    current.Y > Game1.currentLocation.Map.DisplayHeight / Game1.tileSize - 1 ||
                    current.X < 0 || current.Y < 0)
                {
                    Logger.Info(
                        $"Blocking this tile: {current.X},{current.Y}     display width {location.Map.DisplayWidth}   display height {location.Map.DisplayHeight}");
                    continue;
                }
    
                if (_closedList.Contains(new Point(current.X, current.Y)) && current != startTile)
                {
                    continue;
                }
                
                if (location.doesTileHaveProperty(current.X, current.Y, property, "Back") == null)
                {
                    continue;
                }
    
                _closedList.Push(current);

                Queue<PathNode> neighbours = IPathing.Graph.Neighbours(new PathNode(current.X,current.Y,null));
                foreach (var node in neighbours.Where(node => !_closedList.Contains(new Point(node.X,node.Y)) && !IPathing.collisionMap.IsBlocked(node.X,node.Y)))
                {
                    Logger.Info($"in foreach this is node: {node.X},{node.Y}");
                    _frontier.Enqueue(new Point(node.X,node.Y));
                    _tileGroup.Push(current);
                }
    
                runs++;
            }

            return _tileGroup;
        }
    
        private static void ClearVariables()
        {
            _frontier.Clear();
            _terrainTileGroup.Clear();
            _closedList.Clear();
            _tileGroup.Clear();
        }
    }
}