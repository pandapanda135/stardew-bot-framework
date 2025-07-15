using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;

public abstract class ITile
{
    public Point Position => new Point(X, Y);
    public bool Visited;
    
    public int X;
    public int Y;

    public bool Equals(ITile tile)
    {
        return tile.X == this.X && tile.Y == this.Y;
    }
    
    public bool Equals(Point tile)
    {
        return tile.X == this.X && tile.Y == this.Y;
    }
}