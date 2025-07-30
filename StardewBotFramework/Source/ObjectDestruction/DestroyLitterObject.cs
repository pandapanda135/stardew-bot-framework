using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.ObjectDestruction;

public class DestroyLitterObject
{
	public static void Destroy(Object obj)
	{
		// should work fine for now maybe should be a bit more complicated
		while (BotBase.CurrentLocation.Objects.ContainsKey(obj.TileLocation))
		{
			DestroyObject.UseTool();
		}
	}

	/// <summary>
	/// Is a Litter object that can be destroyed
	/// </summary>
	public static bool IsDestructible(Object obj)
	{
		if (obj.IsTwig() || obj.IsBreakableStone() || obj.isSapling() || obj.IsFruitTreeSapling() || obj.IsWeeds())
		{
			return true;
		}
		return false;
	}
}