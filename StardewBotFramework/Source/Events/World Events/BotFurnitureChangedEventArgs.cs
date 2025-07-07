using System.Diagnostics.Tracing;
using StardewValley;
using StardewValley.Objects;

namespace StardewBotFramework.Source.Events;

public class BotFurnitureChangedEventArgs : EventArgs
{
    internal BotFurnitureChangedEventArgs(IEnumerable<Furniture> added,IEnumerable<Furniture> removed,GameLocation location)
    {
        Added = added;
        Removed = removed;
        Location = location;
    }
    
    public IEnumerable<Furniture> Added;
    public IEnumerable<Furniture> Removed;
    public GameLocation Location;
}