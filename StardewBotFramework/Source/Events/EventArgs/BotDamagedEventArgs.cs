using StardewValley.Monsters;

namespace StardewBotFramework.Source.Events.EventArgs;

public class BotDamagedEventArgs
{
	public int Damaged;
	public bool OverrideParry;
	public Monster Damager;

	public BotDamagedEventArgs(int damaged, bool overrideParry, Monster damager)
	{
		Damaged = damaged;
		OverrideParry = overrideParry;
		Damager = damager;
	}
}