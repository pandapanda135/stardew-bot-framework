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
}