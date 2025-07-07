using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Events;

public class BotObjectListChangedEventArgs : EventArgs
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