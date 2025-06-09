using StardewPathfinding.Pathfinding;
using StardewValley;

namespace StardewBotFramework;

public interface IBot
{
    delegate void BotFinishedEventHandler();
    public event BotFinishedEventHandler OnBotFinished;
    
    /// <summary>
    /// These actions should be used as a way for you to signify what you want to happen at the end of the bots routine
    /// </summary>
    enum Actions // these actions are not 100% finalised and most likely will be changed in the future 
    {
        Movement,
        Tool,
        Weapon,
    }
}
