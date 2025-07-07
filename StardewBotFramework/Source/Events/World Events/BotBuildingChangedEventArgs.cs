using StardewValley;
using StardewValley.Buildings;

namespace StardewBotFramework.Source.Events;

public class BotBuildingChangedEventArgs : EventArgs
{
    internal BotBuildingChangedEventArgs(IEnumerable<Building> added,IEnumerable<Building> removed,GameLocation location)
    {
        Added = added;
        Removed = removed;
        Location = location;
    }
    
    public IEnumerable<Building> Added;
    public IEnumerable<Building> Removed;
    public GameLocation Location;
}