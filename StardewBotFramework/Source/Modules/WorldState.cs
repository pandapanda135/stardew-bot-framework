using StardewValley.Network;

namespace StardewBotFramework.Source.Modules;

public class WorldState
{
    /// <summary>Get the current weather in this location's context (regardless of whether the player is currently indoors or outdoors).</summary>
    /// <returns><see cref="LocationWeather"/> of current location</returns>
    public LocationWeather GetCurrentLocationWeather()
    {
        return StardewClient.CurrentLocation.GetWeather();
    }
}