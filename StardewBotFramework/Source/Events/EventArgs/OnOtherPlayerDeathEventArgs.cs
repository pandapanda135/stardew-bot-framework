using StardewValley;

namespace StardewBotFramework.Source.Events.EventArgs;

public class OnOtherPlayerDeathEventArgs : System.EventArgs
{
    public Farmer Farmer;

    public OnOtherPlayerDeathEventArgs(Farmer farmer)
    {
        Farmer = farmer;
    }
}