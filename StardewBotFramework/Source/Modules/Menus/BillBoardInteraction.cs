using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class BillBoardInteraction : MenuHandler
{
	public Billboard Menu
	{
		get => _menu as Billboard ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	public void SetMenu(Billboard billboard) => Menu = billboard;

	public Dictionary<int,Billboard.BillboardDay> GetCalendar()
	{
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
		if (Menu.acceptQuestButton is null || !Menu.acceptQuestButton.visible) return;

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
		if (Menu.acceptQuestButton is null || !Menu.acceptQuestButton.visible) return false;
		
		Menu.receiveLeftClick(Menu.acceptQuestButton.bounds.X,Menu.acceptQuestButton.bounds.Y);
		return true;
	}
}