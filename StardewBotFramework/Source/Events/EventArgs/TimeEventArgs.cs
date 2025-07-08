namespace StardewBotFramework.Source.Events.EventArgs;

public class TimeEventArgs : System.EventArgs
{
    internal TimeEventArgs(int oldTime, int newTime)
    {
        oldTime = OldTime;
        newTime = NewTime;
    }
    
    public int OldTime;
    public int NewTime;
}