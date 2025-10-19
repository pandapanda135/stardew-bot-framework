using Netcode;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Quests;

namespace StardewBotFramework.Source.Modules.Menus;

public class QuestLogInteraction : MenuHandler
{
	public QuestLog Menu
	{
		get => _menu as QuestLog ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}
	
	/// <summary>
	/// The quests the bot has access to.
	/// </summary>
	public NetObjectList<Quest> Quests => Game1.player.questLog;

	public void SetMenu(QuestLog menu) => Menu = menu;
	
	public void OpenLog()
	{
		QuestLog log = new QuestLog();
		Game1.activeClickableMenu = new QuestLog();
		SetMenu(log);
	}

	public void CloseLog() => LeftClick(Menu.upperRightCloseButton);

	public void CloseQuest() => LeftClick(Menu.backButton);

	public void NextRightPage() => LeftClick(Menu.forwardButton);

	public void NextLeftPage() => LeftClick(Menu.backButton);

	public void GetReward() => LeftClick(Menu.rewardBox);
	
	/// <summary>
	/// This must be between 0-5, as that is the amount of quests that can be shown on one page.
	/// </summary>
	/// <param name="index"></param>
	/// <returns>This will return false if you gave an invalid index, or you have not opened quest log through OpenLog or set menu.</returns>
	public bool OpenQuestIndex(int index)
	{
		if (index >= QuestLog.questsPerPage || index < 0)
		{
			return false;
		}
		
		LeftClick(Menu.questLogButtons[index]);
		return true;
	}
}