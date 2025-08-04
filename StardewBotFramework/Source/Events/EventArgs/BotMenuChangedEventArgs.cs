using StardewValley.Menus;

namespace StardewBotFramework.Source.Events.EventArgs;

public class BotMenuChangedEventArgs
{
	public IClickableMenu NewMenu;
	public IClickableMenu OldMenu;

	public BotMenuChangedEventArgs(IClickableMenu newMenu, IClickableMenu oldMenu)
	{
		NewMenu = newMenu;
		OldMenu = oldMenu;
	}
}