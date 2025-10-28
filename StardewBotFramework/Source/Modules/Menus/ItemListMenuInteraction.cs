using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class ItemListMenuInteraction : MenuHandler
{
	public ItemListMenu Menu
	{
		get => _menu as ItemListMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}
	public int ItemPerPage => Menu.itemsPerCategoryPage;
	// public int CurrentPage => BotBase.?.Helper.Reflection.GetField<int>(Menu, "currentTab").GetValue() ?? -1;

	public void SetMenu(ItemListMenu menu) => Menu = menu;

	public new void RemoveMenu() => _menu = null;

	public List<Item> GetItems() => BotBase.GetItemListItems();

	public void ClickOk() => LeftClick(Menu.okButton);
	
	public void ClickForward() => LeftClick(Menu.forwardButton);
	
	public void ClickBack() => LeftClick(Menu.backButton);
}