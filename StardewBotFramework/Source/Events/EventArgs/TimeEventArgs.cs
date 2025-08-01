namespace StardewBotFramework.Source.Events.EventArgs;

public class TimeEventArgs : System.EventArgs
{
    internal TimeEventArgs(int oldTime, int newTime)
    {
        OldTime = oldTime;
        NewTime = newTime;
    }
    
    public int OldTime;
    public int NewTime;
}