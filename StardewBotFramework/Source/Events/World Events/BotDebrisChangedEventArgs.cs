using StardewValley;

namespace StardewBotFramework.Source.Events.World_Events;

public class BotDebrisChangedEventArgs : System.EventArgs
{
    internal BotDebrisChangedEventArgs(IEnumerable<Debris> added,IEnumerable<Debris> removed,GameLocation location)
    {
        Added = added;
        Removed = removed;
        Location = location;
    }
    
    public IEnumerable<Debris> Added;
    public IEnumerable<Debris> Removed;
    public GameLocation Location;
}