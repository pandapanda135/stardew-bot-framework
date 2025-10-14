using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class LetterViewer
{
	private static LetterViewerMenu? _menu;

	/// <summary>
	/// This is if the letter has a quest or special order
	/// </summary>
	public bool? HasQuest => (bool)_menu?.HasQuestOrSpecialOrder;

	/// <summary>
	/// If there are items to grab
	/// </summary>
	public bool? itemsToGrab => _menu?.itemsLeftToGrab();

	public string? recipeLearned => _menu?.learnedRecipe;

	public List<ClickableComponent> Items => _menu is null ? new() : _menu.itemsToGrab;

	public void SetMenu(LetterViewerMenu menu)
	{
		_menu = menu;
	}

	public string GetTitle()
	{
		if (_menu?.mailTitle is null) return "";
		return _menu.mailTitle;
	}

	public List<string> GetMessage()
	{
		if (_menu?.mailMessage is null) return new();
		return _menu.mailMessage;
	}

	public void GrabItems()
	{
		if (_menu?.itemsToGrab is null) return;
		foreach (var cc in _menu.itemsToGrab)
		{
			_menu.receiveLeftClick(cc.bounds.X,cc.bounds.Y);
		}
	}

	public void GrabItem(Item item)
	{
		if (_menu?.itemsToGrab is null) return;
		foreach (var cc in _menu.itemsToGrab)
		{
			if (cc.item != item) continue;

			_menu.receiveLeftClick(cc.bounds.X,cc.bounds.Y);
		}
	}
	
	public void AcceptQuest()
	{
		if (_menu?.acceptQuestButton is null) return;
		_menu.receiveLeftClick(_menu.acceptQuestButton.bounds.X,_menu.acceptQuestButton.bounds.Y);
	}

	public void NextPage()
	{
		if (_menu?.forwardButton is null) return;
		_menu.receiveLeftClick(_menu.forwardButton.bounds.X,_menu.forwardButton.bounds.Y);
	}
	
	public void PastPage()
	{
		if (_menu?.backButton is null) return;
		_menu.receiveLeftClick(_menu.backButton.bounds.X,_menu.backButton.bounds.Y);
	}

	public void ExitMenu()
	{
		if (_menu?.upperRightCloseButton is null) return;
		_menu.receiveLeftClick(_menu.upperRightCloseButton.bounds.X,_menu.upperRightCloseButton.bounds.Y);
		_menu = null;
	}
}