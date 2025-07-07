using StardewValley;

namespace StardewBotFramework.Source.Events;

public class BotDebrisChangedEventArgs : EventArgs
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