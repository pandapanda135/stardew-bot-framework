using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class CommunityCenterMenu : MenuHandler
{
	public JunimoNoteMenu Menu
	{
		get => _menu as JunimoNoteMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	public void SetMenu(JunimoNoteMenu menu) => Menu = menu;

	public void ChangePage(bool right)
	{
		int direction = right ? 1 : -1;
		Menu.SwapPage(direction);
	}
	
	public void SelectBundle(int bundleIndex) => LeftClick(Menu.bundles[bundleIndex]);

	public bool AddItem(Item item)
	{
		int itemIndex = Menu.inventory.actualInventory.IndexOf(item);
		if (itemIndex == -1)
		{
			return false;
		}

		if (Menu.currentPageBundle.complete || !Menu.currentPageBundle.depositsAllowed) // this is also done in canAcceptThisItem
		{
			return false;
		}
		
		foreach (var ingCc in Menu.ingredientSlots)
		{
			if (!Menu.currentPageBundle.canAcceptThisItem(item, ingCc)) continue;
			
			Menu.currentPageBundle.tryToDepositThisItem(item, ingCc, "LooseSprites\\JunimoNote", Menu);
			return true;
		}

		return false;
	}
}