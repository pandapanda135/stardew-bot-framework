using StardewModdingAPI;

namespace StardewBotFramework.Source.Events;

public class BotPlayerConnectedEventArgs : EventArgs
{
    internal BotPlayerConnectedEventArgs(IMultiplayerPeer player)
    {
        Player = player;
    }
    
    public IMultiplayerPeer Player;
}