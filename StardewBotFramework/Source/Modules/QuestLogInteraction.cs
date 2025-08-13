using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class QuestLogInteraction
{
	private static QuestLog? currentMenu = null;

	public void SetMenu(QuestLog? menu)
	{
		currentMenu = menu;
	}
	
	public void OpenLog()
	{
		Game1.activeClickableMenu = new QuestLog();
		SetMenu(Game1.activeClickableMenu as QuestLog);
	}

	public void CloseLog()
	{
		if (currentMenu is null) return;
		ClickableComponent button = currentMenu.upperRightCloseButton;
		currentMenu.receiveLeftClick(button.bounds.X,button.bounds.Y);
	}

	public void CloseQuest()
	{
		if (currentMenu is null) return;
		ClickableComponent button = currentMenu.backButton;
		currentMenu.receiveLeftClick(button.bounds.X,button.bounds.Y);
	}
	
	/// <summary>
	/// This must be between 0-5, as that is the amount of quests that can be shown on one page.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>This will return false if you gave an invalid index, or you have not opened quest log through OpenLog or set menu.</returns>
	public bool OpenQuestIndex(int index)
	{
		if (index >= QuestLog.questsPerPage || index < 0 || currentMenu is null)
		{
			return false;
		}

		ClickableComponent questButton = currentMenu.questLogButtons[index]; 
		currentMenu.receiveLeftClick(questButton.bounds.X + 2,questButton.bounds.Y + 2);
		return true;
	}

	public void NextRightPage()
	{
		if (currentMenu is null)
		{
			return;
		}
		ClickableComponent button = currentMenu.forwardButton;
		currentMenu.receiveLeftClick(button.bounds.X + 2,button.bounds.Y + 2);
	}

	public void NextLeftPage()
	{
		if (currentMenu is null)
		{
			return;
		}
		ClickableComponent button = currentMenu.backButton;
		currentMenu.receiveLeftClick(button.bounds.X + 2,button.bounds.Y + 2);
	}

	public void GetReward()
	{
		if (currentMenu is null) return;

		ClickableComponent rewardButton = currentMenu.rewardBox;
		currentMenu.receiveLeftClick(rewardButton.bounds.X + 2,rewardButton.bounds.Y + 2);
	}
}