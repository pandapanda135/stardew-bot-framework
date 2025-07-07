using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.Events;

public class BotTerrainFeatureChangedEventArgs : EventArgs
{
    internal BotTerrainFeatureChangedEventArgs(IEnumerable<KeyValuePair<Vector2, TerrainFeature>> added,IEnumerable<KeyValuePair<Vector2, TerrainFeature>> removed,GameLocation location)
    {
        Added = added;
        Removed = removed;
        Location = location;
    }
    
    public IEnumerable<KeyValuePair<Vector2, TerrainFeature>> Added;
    public IEnumerable<KeyValuePair<Vector2, TerrainFeature>> Removed;
    public GameLocation Location;
}