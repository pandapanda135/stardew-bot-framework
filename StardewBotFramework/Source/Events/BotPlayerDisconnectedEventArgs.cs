using StardewModdingAPI;

namespace StardewBotFramework.Source.Events;

public class BotPlayerDisconnectedEventArgs : EventArgs
{
    internal BotPlayerDisconnectedEventArgs(IMultiplayerPeer peer)
    {
        Peer = peer;
    }
    
    public IMultiplayerPeer Peer;
}