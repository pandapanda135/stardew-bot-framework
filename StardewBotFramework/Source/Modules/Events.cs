using StardewValley;

namespace StardewBotFramework.Source.Modules;

public delegate EventHandler OnItemCollected();

public class Events
{
    public event OnItemCollected OnItemCollected;
}