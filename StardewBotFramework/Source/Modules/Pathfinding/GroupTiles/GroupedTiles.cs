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
    
    private List<Group> _validFeatures = new();
    private List<Point> _usedFeatureTiles = new();
    private BreadthFirstGrouping.Pathing breadthFirstGrouping = new();

    /// <summary>
    /// Get all patches of <see cref="HoeDirt"/> in this location
    /// </summary>
    /// <param name="location">location to query</param>
    /// <returns>A lists of lists that contains the patches of <see cref="HoeDirt"/></returns>
    public async Task<List<Group>> StartDirtCheck(GameLocation location)
    {
        _validFeatures = new();
        _usedFeatureTiles = new();
        
        var locationTerrain = location.terrainFeatures;
        foreach (var terrainDict in locationTerrain)
        {
            foreach (var kvp in terrainDict)
            {
                if (kvp.Value is HoeDirt &&
                    !_usedFeatureTiles.Contains(kvp.Key.ToPoint()))
                {
                    Logger.Info($"Running: X: {kvp.Key.X} Y:{kvp.Key.Y}");
                    await UseDirtFunc(kvp.Key.ToPoint(), location);
                }
            }
        }
        
        BreadthFirstGrouping.Pathing._usedStartPoint.Clear();
        List<Group> groups = new();
        foreach (var kvp in _validFeatures)
        {
            Group group = new();
            foreach (var tile in kvp.GetTiles())
            {
                Logger.Info($"Get tiles: {tile.Position}");
                PlantTile? plantTile = tile as PlantTile;
                if (plantTile is null) continue;
                HoeDirt hoeDirt = plantTile.TerrainFeature as HoeDirt;
                if (hoeDirt is null) continue;
                if (group.Contains(plantTile)) continue;
                group.Add(plantTile);
            }
            groups.Add(group);
        }

        return groups;
    }

    private async Task UsePropertyFunc(Point tile,string property,GameLocation location)
    {
        Logger.Info($"running func");
        Stack<Point> points = await breadthFirstGrouping.GetGroup(tile, property, location, 100);
        foreach (var point in points)
        {
            Logger.Info($"adding point: {point}");
            _validPoints.Add(point);
        }
    }

    private async Task UseDirtFunc(Point tile, GameLocation location)
    {
        Logger.Info($"running func");
        Stack<HoeDirt> points = await breadthFirstGrouping.GetTerrainGroup(tile, location, 100000);
        Group group = new();
        foreach (var point in points)
        {
            group.Add(new PlantTile(point,point.isWatered(),point.needsWatering(),point.paddyWaterCheck()));
        }
        _validFeatures.Add(group);
        foreach (var point in points)
        {
            Logger.Info($"adding point: {point.Tile.ToPoint()}");
            _usedFeatureTiles.Add(point.Tile.ToPoint());
        }
    }
}