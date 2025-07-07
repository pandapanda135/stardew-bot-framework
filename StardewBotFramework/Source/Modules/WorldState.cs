using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// Get facts about the current world this includes information like the current weather
/// </summary>
public class WorldState
{
    /// <summary>
    /// Get all characters in the current <see cref="GameLocation"/>.
    /// </summary>
    public Dictionary<Point,NPC> CharactersInLocation => _characters.GetCharactersInCurrentLocation(StardewClient.CurrentLocation);

    /// <summary>
    /// Get all <see cref="Object"/> in location as a <see cref="OverlaidDictionary"/>
    /// </summary>
    public OverlaidDictionary ObjectsInLocation => Game1.currentLocation.Objects;
    private readonly Characters _characters = new Characters();
    /// <summary>Get the current weather in this location's context (regardless of whether the player is currently indoors or outdoors).</summary>
    /// <returns><see cref="LocationWeather"/> of current location</returns>
    public LocationWeather GetCurrentLocationWeather()
    {
        return StardewClient.CurrentLocation.GetWeather();
    }
}

/// <summary>
/// Returns current game time in different formats.
/// Check this to see how to use the values returned.
/// https://stardewvalleywiki.com/Modding:Modder_Guide/Game_Fundamentals#Time_format
/// </summary>
public class Time
{
    /// <summary>
    /// Returns state of year as a string
    /// </summary>
    public string GetStateOfYearString()
    {
        Logger.Info($"Year: {Game1.year} Season: {Game1.season} Day: {Game1.dayOfMonth}");
        return $"Year: {Game1.year} Season: {Game1.season} Day: {Game1.dayOfMonth}";
    }

    /// <summary>
    /// Gets state of year as an Int array.
    /// </summary>
    /// <returns>
    /// Int array in format: Year,Season,Day of month. Season will be represented as:
    /// Spring,Summer,Fall,Winter from 0-3
    /// </returns>
    public int[] GetStateOfYear()
    {
        return new [] { Game1.year, (int)Game1.season, Game1.dayOfMonth };
    }
    
    /// <summary>
    /// Gets time value of current day as a string
    /// </summary>
    /// <returns>will return an int of time, check the wiki for how to use it.
    /// https://stardewvalleywiki.com/Modding:Modder_Guide/Game_Fundamentals#Time_format</returns>
    public string GetTimeString()
    {
        return Game1.getTimeOfDayString(Game1.timeOfDay);
    }

    /// <summary>
    /// Get the current date as an <see cref="SDate"/> from SMAPI. This is the recommended way of getting the state of the year 
    /// </summary>
    public SDate GetSDate()
    {
        return SDate.Now();
    }

    /// <summary>
    /// Gets time value of current day
    /// </summary>
    /// <returns>will return an int of time, check the wiki for how to use it.
    /// https://stardewvalleywiki.com/Modding:Modder_Guide/Game_Fundamentals#Time_format</returns>
    public int GetTime()
    {
        return Game1.timeOfDay;
    }

    /// <summary>
    /// If is currently past midnight
    /// </summary>
    /// <returns>True if GetTime is less than 2000 (midnight) else false</returns>
    public bool IsDay()
    {
        if (GetTime() >= 2000) return false;

        return true;
    }

    /// <summary>
    /// If there is a festival.
    /// </summary>
    /// <returns>true if festival else false</returns>
    public bool IsFestival()
    {
        return Game1.isFestival();
    }
}