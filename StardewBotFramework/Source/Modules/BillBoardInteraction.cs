using System.Net.Http.Headers;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class BillBoardInteraction
{
	public Billboard? Menu;

	public void SetMenu(Billboard billboard)
	{
		Menu = billboard;
	}

	public Dictionary<int,Billboard.BillboardDay> GetCalendar()
	{
		if (Menu is null) return new();
		if (Menu.acceptQuestButton is null || Menu.acceptQuestButton.visible)
		{
			return new();
		}
		return Menu.calendarDayData;
	}

	public void GetDailyQuest(out string title, out string description,out string objective)
	{
		title = "";
		description = "";
		objective = "";
		if (Menu is null || Menu.acceptQuestButton is null || !Menu.acceptQuestButton.visible) return;

		title = Game1.questOfTheDay.questTitle;
		description = Game1.questOfTheDay.questDescription;
		objective = Game1.questOfTheDay.currentObjective;
	}

	/// <summary>
	/// Accept the daily quest, menu must be set
	/// </summary>
	/// <returns>Return null, if quest cannot be accepted, else false</returns>
	public bool AcceptDailyQuest()
	{
		if (Menu is null) return false;
		if (Menu.acceptQuestButton is null || !Menu.acceptQuestButton.visible) return false;
		
		Menu.receiveLeftClick(Menu.acceptQuestButton.bounds.X,Menu.acceptQuestButton.bounds.Y);
		return true;
	}

	/// <summary>
	/// Exit the current menu
	/// </summary>
	public void ExitMenu()
	{
		if (Menu is null) return;
		Menu.exitThisMenu();
		Menu = null;
	}
}