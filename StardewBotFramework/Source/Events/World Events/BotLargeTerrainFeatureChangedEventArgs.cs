using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Events;

public class BotLargeTerrainFeatureChangedEventArgs : EventArgs
{
    internal BotLargeTerrainFeatureChangedEventArgs(IEnumerable<LargeTerrainFeature> added,IEnumerable<LargeTerrainFeature> removed,GameLocation location)
    {
        Added = added;
        Removed = removed;
        Location = location;
    }
    
    public IEnumerable<LargeTerrainFeature> Added;
    public IEnumerable<LargeTerrainFeature> Removed;
    public GameLocation Location;
}