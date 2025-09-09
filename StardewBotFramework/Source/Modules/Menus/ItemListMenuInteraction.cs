using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class ItemListMenuInteraction
{
	public ItemListMenu Menu = null!;
	public int ItemPerPage => Menu.itemsPerCategoryPage;
	// public int CurrentPage => BotBase.Instance?.Helper.Reflection.GetField<int>(Menu, "currentTab").GetValue() ?? -1;

	public void SetMenu(ItemListMenu menu)
	{
		Menu = menu;
	}

	public void RemoveMenu()
	{
		Menu = null!;
	}

	public List<Item> GetItems()
	{
		return BotBase.GetItemListItems();
	}

	public void ClickOk()
	{
		Menu.receiveLeftClick(Menu.okButton.bounds.X,Menu.okButton.bounds.Y);
	}
	
	public void ClickForward()
	{
		Menu.receiveLeftClick(Menu.forwardButton.bounds.X,Menu.forwardButton.bounds.Y);
	}
	
	public void ClickBack()
	{
		Menu.receiveLeftClick(Menu.backButton.bounds.X,Menu.backButton.bounds.Y);
	}
}