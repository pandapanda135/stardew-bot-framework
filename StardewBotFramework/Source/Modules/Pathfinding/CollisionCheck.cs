using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace StardewBotFramework.Source.Modules.Pathfinding;

public class CollisionCheck
{
    public Rectangle Position;
    public xTile.Dimensions.Rectangle Viewport;
    public bool IsFarmer;
    public Character Character;
    public GameLocation Location;
    public TaskCompletionSource<bool> CompletionSource;

    public CollisionCheck(Rectangle position,xTile.Dimensions.Rectangle viewport,bool isFarmer,Character character,GameLocation location,TaskCompletionSource<bool> tcs)
    {
        Position = position;
        Viewport = viewport;
        IsFarmer = isFarmer;
        Character = character;
        Location = location;
        CompletionSource = tcs;
    }
}