using System.Net.Sockets;
using StardewValley;

namespace StardewBotFramework.Source.Events;

public class BotWarpedEventArgs : EventArgs
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