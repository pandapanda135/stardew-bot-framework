using Netcode;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace StardewBotFramework.Source.Modules;

public class QuestLogInteraction
{
	private static QuestLog? _currentMenu = null;

	/// <summary>
	/// The quests the bot has access to.
	/// </summary>
	public NetObjectList<Quest> Quests => Game1.player.questLog;

	public void SetMenu(QuestLog? menu)
	{
		_currentMenu = menu;
	}
	
	public void OpenLog()
	{
		Game1.activeClickableMenu = new QuestLog();
		SetMenu(Game1.activeClickableMenu as QuestLog);
	}

	public void CloseLog()
	{
		if (_currentMenu is null) return;
		ClickableComponent button = _currentMenu.upperRightCloseButton;
		_currentMenu.receiveLeftClick(button.bounds.X,button.bounds.Y);
	}

	public void CloseQuest()
	{
		if (_currentMenu is null) return;
		ClickableComponent button = _currentMenu.backButton;
		_currentMenu.receiveLeftClick(button.bounds.X,button.bounds.Y);
	}
	
	/// <summary>
	/// This must be between 0-5, as that is the amount of quests that can be shown on one page.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>This will return false if you gave an invalid index, or you have not opened quest log through OpenLog or set menu.</returns>
	public bool OpenQuestIndex(int index)
	{
		if (index >= QuestLog.questsPerPage || index < 0 || _currentMenu is null)
		{
			return false;
		}

		ClickableComponent questButton = _currentMenu.questLogButtons[index]; 
		_currentMenu.receiveLeftClick(questButton.bounds.X + 2,questButton.bounds.Y + 2);
		return true;
	}

	public void NextRightPage()
	{
		if (_currentMenu is null)
		{
			return;
		}
		ClickableComponent button = _currentMenu.forwardButton;
		_currentMenu.receiveLeftClick(button.bounds.X + 2,button.bounds.Y + 2);
	}

	public void NextLeftPage()
	{
		if (_currentMenu is null)
		{
			return;
		}
		ClickableComponent button = _currentMenu.backButton;
		_currentMenu.receiveLeftClick(button.bounds.X + 2,button.bounds.Y + 2);
	}

	public void GetReward()
	{
		if (_currentMenu is null) return;

		ClickableComponent rewardButton = _currentMenu.rewardBox;
		_currentMenu.receiveLeftClick(rewardButton.bounds.X + 2,rewardButton.bounds.Y + 2);
	}
}