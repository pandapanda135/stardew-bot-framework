using StardewValley;

namespace StardewBotFramework.Source.Events.EventArgs;

public class EventEndedEventArgs
{
	public Event Event;
	
	public EventEndedEventArgs(Event e)
	{
		Event = e;
	}
}