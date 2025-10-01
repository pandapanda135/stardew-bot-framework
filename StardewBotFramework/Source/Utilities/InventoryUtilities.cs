using StardewValley.Inventories;

namespace StardewBotFramework.Source.Utilities;

internal static class InventoryUtilities
{
	public static int GetFirstEmptySlot(Inventory inventory)
	{
		for (var i = 0; i < inventory.Count; i++)
		{
			if (inventory[i] is null) return i;
		}

		return -1;
	}
}