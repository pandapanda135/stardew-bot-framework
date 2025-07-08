using StardewValley;

namespace StardewBotFramework.Source.Events.EventArgs;

public class BotWarpedEventArgs : System.EventArgs
{
    internal BotWarpedEventArgs(Farmer player,GameLocation oldLocation, GameLocation newLocation, bool isBot)
    {
        Player = player;
        OldLocation = oldLocation;
        NewLocation = newLocation;
        IsBot = isBot;
    }
    
    public Farmer Player;
    public GameLocation OldLocation;
    public GameLocation NewLocation;
    
    /// <summary>
    /// This will always be true
    /// </summary>
    public bool IsBot;
}