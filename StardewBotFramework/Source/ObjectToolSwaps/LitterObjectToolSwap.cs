using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Tools;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.ObjectToolSwaps;

public class LitterObjectToolSwap
{
	public static bool Swap(Point tile)
	{
		GameLocation location = BotBase.CurrentLocation;

		foreach (var objDict in location.Objects)
		{
			if (objDict.ContainsKey(tile.ToVector2()))
			{
				Object obj = objDict[tile.ToVector2()];
				if (obj.IsBreakableStone())
				{
					Logger.Info($"Changing to pickaxe in object");
					SwapItemHandler.SwapItem(typeof(Pickaxe),"");
					return true;
				}

				if (obj.IsTwig() || obj.isSapling() || obj.IsFruitTreeSapling())
				{
					Logger.Info($"Changing to axe in object");
					SwapItemHandler.SwapItem(typeof(Axe), "");
					return true;
				}

				if (obj.IsWeeds())
				{
					Logger.Info($"Changing to scythe in object");
					SwapItemHandler.SwapItem(typeof(MeleeWeapon),"Scythe");
					return true;
				}
			}
		}

		return false;
	}
}