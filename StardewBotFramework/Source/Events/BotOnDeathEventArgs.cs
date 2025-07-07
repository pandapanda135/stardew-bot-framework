using Microsoft.Xna.Framework;
using StardewValley;

namespace StardewBotFramework.Source.Events;

public class BotOnDeathEventArgs : EventArgs
{
    internal BotOnDeathEventArgs(GameLocation deathLocation,Point deathPoint,int itemLostAmount)
    {
        DeathLocation = deathLocation;
        DeathPoint = deathPoint;
        ItemLostAmount = itemLostAmount;
    }

    public GameLocation DeathLocation;
    public Point DeathPoint;
    public int ItemLostAmount;
}