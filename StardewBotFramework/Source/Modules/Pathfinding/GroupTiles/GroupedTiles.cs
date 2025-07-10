using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public class GroupedTiles
{
    private readonly List<Point> _validPoints = new();

    public async Task<List<Point>> StartPropertyCheck(string property, GameLocation location,string? checkValue = null)
    {
        List<Point> tiles = new();

        int maxX = location.Map.DisplayWidth / Game1.tileSize;
        int maxY = location.Map.DisplayWidth / Game1.tileSize;
        
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                if (checkValue == null)
                {
                    if (location.doesTileHaveProperty(x, y, property, "Back") != checkValue && !_validPoints.Contains(new Point(x,y))) // null == false
                    {
                        await UsePropertyFunc(new Point(x, y), property, location);
                    }
                }
                else
                {
                    Logger.Info($"property value: {location.doesTileHaveProperty(x, y, property, "Back")}");
                    if (location.doesTileHaveProperty(x, y, property, "Back") == checkValue && !_validPoints.Contains(new Point(x,y)))
                    {
                        Logger.Info($"running function");
                        await UsePropertyFunc(new Point(x, y), property, location);
                    }
                }
            }
        }

        return _validPoints;
   }
    
    private List<List<HoeDirt>> _validFeatures = new();
    private List<Point> _validFeatureTiles = new();
    private BreadthFirstGrouping.Pathing breadthFirstGrouping = new();

    public async Task<List<List<HoeDirt>>> StartDirtCheck(GameLocation location)
    {
        _validFeatures = new();
        _validFeatureTiles = new();
        
        var locationTerrain = location.terrainFeatures;
        int maxX = location.Map.DisplayWidth / Game1.tileSize;
        int maxY = location.Map.DisplayWidth / Game1.tileSize;
        
        for (int x = 0; x < maxX; x++)
        {
            for (int y = 0; y < maxY; y++)
            {
                if (!locationTerrain.Keys.Contains(new Vector2(x,y))) continue;
                if (locationTerrain[new Vector2(x,y)] is HoeDirt && !_validFeatureTiles.Contains(locationTerrain[new Vector2(x,y)].Tile.ToPoint()))
                {
                    await UseDirtFunc(new Point(x, y), location);
                }
            }
        }

        BreadthFirstGrouping.Pathing._usedStartPoint.Clear();
        return _validFeatures;
    }

    public async Task UsePropertyFunc(Point tile,string property,GameLocation location)
    {
        Logger.Info($"running func");
        Stack<Point> points = await breadthFirstGrouping.GetGroup(tile, property, location, 100);
        foreach (var point in points)
        {
            Logger.Info($"adding point: {point}");
            _validPoints.Add(point);
        }
    }

    public async Task UseDirtFunc(Point tile, GameLocation location)
    {
        Logger.Info($"running func");
        BreadthFirstGrouping.Pathing breadthFirstGrouping = new BreadthFirstGrouping.Pathing();
        Stack<HoeDirt> points = await breadthFirstGrouping.GetTerrainGroup(tile, location, 100000);
        foreach (var point in points)
        {
            Logger.Info($"adding point: {point}");
            _validFeatures.Add(points.ToList());
            _validFeatureTiles.Add(point.Tile.ToPoint());
        }
    }
}