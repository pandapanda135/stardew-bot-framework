using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class CommunityCenterMenu
{
	public JunimoNoteMenu Menu = null!;

	public void SetMenu(JunimoNoteMenu menu)
	{
		Menu = menu;
	}

	public void ExitMenu()
	{
		Menu.exitThisMenu();
		Menu = null!;
	}

	public void ChangePage(bool right)
	{
		int direction = right ? 1 : -1;
		Menu.SwapPage(direction);
	}
	
	public void SelectBundle(int bundleIndex)
	{
		Bundle bundle = Menu.bundles[bundleIndex];
		Menu.receiveLeftClick(bundle.bounds.X,bundle.bounds.Y);
	}

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
			if (Menu.currentPageBundle.canAcceptThisItem(item, ingCc))
			{
				Menu.currentPageBundle.tryToDepositThisItem(item, ingCc, "LooseSprites\\JunimoNote", Menu);
				return true;
			}
		}

		return false;
	}
}