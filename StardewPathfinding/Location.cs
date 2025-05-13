using Microsoft.Xna.Framework;

namespace StardewPathfinding;

// this will be location for pathfinding to visit
public class Location
{
    public Location(Vector2 position)
    {
        this.Position = position;
    }

    public Vector2 Position;

    public bool HasVisited = false;
}