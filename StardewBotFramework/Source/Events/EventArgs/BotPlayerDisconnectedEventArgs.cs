using StardewModdingAPI;

namespace StardewBotFramework.Source.Events.EventArgs;

public class BotPlayerDisconnectedEventArgs : System.EventArgs
{
    internal BotPlayerDisconnectedEventArgs(IMultiplayerPeer peer)
    {
        Peer = peer;
    }
    
    public IMultiplayerPeer Peer;
}