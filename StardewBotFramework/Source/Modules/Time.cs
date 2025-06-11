using StardewValley;
using StardewValley.Extensions;

namespace StardewBotFramework.Source.Modules;

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
    public static string GetStateOfYearString()
    {
        return $"Year: {Game1.year} Season: {Game1.season} Day: {Game1.dayOfMonth}";
    }

    /// <summary>
    /// Gets state of year as an Int array.
    /// </summary>
    /// <returns>
    /// Int array in format: Year,Season,Day of month. Season will be represented as:
    /// Spring,Summer,Fall,Winter from 0-3
    /// </returns>
    public static int[] GetStateOfYear()
    {
        return new [] { Game1.year, (int)Game1.season, Game1.dayOfMonth };
    }
    
    /// <summary>
    /// Gets time value of current day as a string
    /// </summary>
    /// <returns>will return an int of time, check the wiki for how to use it.
    /// https://stardewvalleywiki.com/Modding:Modder_Guide/Game_Fundamentals#Time_format</returns>
    public static string GetTimeString()
    {
        return Game1.getTimeOfDayString(Game1.timeOfDay);
    }

    /// <summary>
    /// Gets time value of current day
    /// </summary>
    /// <returns>will return an int of time, check the wiki for how to use it.
    /// https://stardewvalleywiki.com/Modding:Modder_Guide/Game_Fundamentals#Time_format</returns>
    public static int GetTime()
    {
        return Game1.timeOfDay;
    }

    /// <summary>
    /// If is currently past midnight
    /// </summary>
    /// <returns>True if GetTime is less than 2000 (midnight) else false</returns>
    public static bool IsDay()
    {
        if (GetTime() >= 2000) return false;

        return true;
    }
}