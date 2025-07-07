using StardewValley;

namespace StardewBotFramework.Source.Events;

public class BotCharacterListChangedEventArgs : EventArgs
{
    internal BotCharacterListChangedEventArgs(IEnumerable<NPC> added, IEnumerable<NPC> removed, GameLocation location)
    {
        Added = added;
        Removed = removed;
        Location = location;
    }
    public IEnumerable<NPC> Added;
    public IEnumerable<NPC> Removed;
    public GameLocation Location;
}