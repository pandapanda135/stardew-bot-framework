using StardewModdingAPI.Enums;

namespace StardewBotFramework.Source.Events.EventArgs;

public class BotSkillLevelChangedEventArgs : System.EventArgs
{
    internal BotSkillLevelChangedEventArgs(SkillType changedSkill,int oldLevel,int newLevel)
    {
        ChangedSkill = changedSkill;
        OldLevel = oldLevel;
        NewLevel = newLevel;
    }
    
    public SkillType ChangedSkill;
    public int OldLevel;
    public int NewLevel;
}