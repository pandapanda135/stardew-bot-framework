namespace StardewBotFramework.Source.Events;

public class TimeEventArgs : EventArgs
{
    internal TimeEventArgs(int oldTime, int newTime)
    {
        oldTime = OldTime;
        newTime = NewTime;
    }
    
    public int OldTime;
    public int NewTime;
}