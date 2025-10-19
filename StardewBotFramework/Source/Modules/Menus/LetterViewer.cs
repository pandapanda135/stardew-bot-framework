using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class LetterViewer : MenuHandler
{
	public LetterViewerMenu Menu
	{
		get => _menu as LetterViewerMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	/// <summary>
	/// This is if the letter has a quest or special order
	/// </summary>
	public bool? HasQuest => Menu.HasQuestOrSpecialOrder;

	/// <summary>
	/// If there are items to grab
	/// </summary>
	public bool? itemsToGrab => Menu.itemsLeftToGrab();

	public string? recipeLearned => Menu.learnedRecipe;
	public List<ClickableComponent> Items => Menu is null ? new() : Menu.itemsToGrab;

	public void SetMenu(LetterViewerMenu menu) => Menu = menu;

	public string GetTitle() => Menu.mailTitle;

	public List<string> GetMessage() => Menu.mailMessage;

	public void GrabItems()
	{
		foreach (var cc in Menu.itemsToGrab)
		{
			LeftClick(cc);
		}
	}

	public void GrabItem(Item item)
	{
		foreach (var cc in Menu.itemsToGrab)
		{
			if (cc.item != item) continue;

			LeftClick(cc);
		}
	}
	
	public void AcceptQuest() => LeftClick(Menu.acceptQuestButton);

	public void NextPage() => LeftClick(Menu.forwardButton);
	
	public void PastPage() => LeftClick(Menu.backButton);

	public void ExitMenu()
	{
		LeftClick(Menu.upperRightCloseButton);
		_menu = null;
	}
}