using StardewModdingAPI;

namespace StardewBotFramework.Source.Events.EventArgs;

public class BotPlayerConnectedEventArgs : System.EventArgs
{
    internal BotPlayerConnectedEventArgs(IMultiplayerPeer player)
    {
        Player = player;
    }
    
    public IMultiplayerPeer Player;
}