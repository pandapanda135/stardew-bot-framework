using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Events.World_Events;

public class BotObjectListChangedEventArgs : System.EventArgs
{
    internal BotObjectListChangedEventArgs(IEnumerable<KeyValuePair<Vector2, Object>> added,
        IEnumerable<KeyValuePair<Vector2, Object>> removed, GameLocation currentLocation)
    {
        Added = added;
        Removed = removed;
        CurrentLocation = currentLocation;
    }
    
    public IEnumerable<KeyValuePair<Vector2, Object>> Added;
    public IEnumerable<KeyValuePair<Vector2, Object>> Removed;
    public GameLocation CurrentLocation;
}