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

	public Bundle CurrentBundle => Menu.currentPageBundle;

	public List<Bundle> Bundles => Menu.bundles;

	public void SetMenu(JunimoNoteMenu menu) => Menu = menu;

	public void ChangePage(bool right)
	{
		int direction = right ? 1 : -1;
		Menu.SwapPage(direction);
	}
	
	public void SelectBundle(int bundleIndex) => LeftClick(Menu.bundles[bundleIndex]);

	public void ExitCurrentBundle() => LeftClick(Menu.backButton);

	public bool AddItem(Item item)
	{
		int itemIndex = Menu.inventory.actualInventory.IndexOf(item);
		if (itemIndex == -1)
		{
			return false;
		}

		if (CurrentBundle.complete || !CurrentBundle.depositsAllowed) // this is also done in canAcceptThisItem
		{
			return false;
		}
		
		foreach (var ingCc in Menu.ingredientSlots)
		{
			if (!CurrentBundle.canAcceptThisItem(item, ingCc)) continue;
			
			CurrentBundle.tryToDepositThisItem(item, ingCc, "LooseSprites\\JunimoNote", Menu);
			return true;
		}

		return false;
	}
}