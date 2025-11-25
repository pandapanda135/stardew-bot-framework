using Microsoft.Xna.Framework;
using Netcode;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.Utilities;
using StardewValley;

namespace StardewBotFramework.Source.Modules;

public class DebrisHandling
{
    public NetCollection<Debris> Debris => BotBase.CurrentLocation.debris;
    
    
    #region PickUpDebris

    // this is the debris approximate position
    private static Vector2 DebrisPosition(NetObjectShrinkList<Chunk> chunks)
    {
        
        Vector2 total = new();
        
        foreach (var chunk in chunks)
        {
            total += chunk.position.Value;
        }
        return total / chunks.Count;
    }
    public async Task PickUpDebrisInRadius(Point startPoint,int radius,bool canDestroy = false)
    {
        GameLocation location = BotBase.CurrentLocation;

        List<Point> radiusPoints = new();
        Vector2 endPoint = new(startPoint.X + radius, startPoint.Y + radius);
        startPoint.X -= radius;
        startPoint.Y -= radius;
        for (int x = startPoint.X; x < endPoint.X; x++)
        {
            for (int y = startPoint.Y; y < endPoint.Y + radius; y++)
            {
                radiusPoints.Add(new Point(x,y));
            }
        }

        List<Point> points = new();
        foreach (var debris in location.debris)
        {
            Logger.Info($"debris: {debris}  debris message: {DebrisPosition(debris.Chunks) / Game1.tileSize}");
            if (debris.item is not null)
            {
                Logger.Info($"debris: {debris.item.Name}");
            }
            Vector2 debrisLocation = DebrisPosition(debris.Chunks) / Game1.tileSize;
            if (radiusPoints.Contains(debrisLocation.ToPoint()))
            {
                points.Add(debrisLocation.ToPoint());      
                Logger.Info($"debris tile {debrisLocation.ToPoint()}");
            }
        }
        await PickUpDebrisTiles(startPoint,points,canDestroy);
    }

    private static async Task PickUpDebrisTiles(Point startPoint, List<Point> debrisTiles,bool canDestroy = false)
    {
        foreach (var debris in debrisTiles)
        {
            Logger.Info($"debris tile point: {debris}");
            if (InMagneticRadius(BotBase.Farmer.MagneticRadius / Game1.tileSize, debris))
            {
                Logger.Info($"in magnetic radius: {debris}  {BotBase.Farmer.MagneticRadius / Game1.tileSize}");
                continue;
            }

            await PathfindingHelper.Goto(new Goal.GoalPosition(debris.X, debris.Y),canDestroy);
        }
    }

    public async Task PickUpDebris(Debris debris)
    {
        Logger.Info($"debris tile point: {debris}");
        // I think this is pixel position
        var pos = (DebrisPosition(debris.Chunks) / 64).ToPoint();
        if (InMagneticRadius(BotBase.Farmer.MagneticRadius / Game1.tileSize, pos))
        {
            Logger.Info($"in magnetic radius: {debris}  {BotBase.Farmer.MagneticRadius / Game1.tileSize}");
            return;
        }

        await PathfindingHelper.Goto(new Goal.GoalPosition(pos.X, pos.Y));
    } 

    private static bool InMagneticRadius(int radius, Point tile)
    {
        Vector2 endPoint = new(tile.X + radius, tile.Y + radius);
        tile.X -= radius;
        tile.Y -= radius;
        for (int x = tile.X; x < endPoint.X; x++)
        {
            for (int y = tile.Y; y < endPoint.Y + radius; y++)
            {
                if (BotBase.Farmer.TilePoint == new Point(x, y)) return true;
            }
        }

        return false;
    }

    #endregion
    
}