using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.ObjectDestruction;

public class DestroyResourceClump
{
	public static void Destroy(ResourceClump clump)
	{
		GameLocation location = BotBase.CurrentLocation;

		if (clump is GiantCrop crop) // idk what these are
		{
			while (crop.health.Value > 0)
			{
				DestroyObject.UseTool();
			}
		}

		if (IsLog(clump) || IsBoulder(clump))
		{
			while (clump.health.Value > 0)
			{
				DestroyObject.UseTool();
			}
		}

		if (IsGreenRainBush(clump))
		{
			DestroyObject.UseTool();
		}
	}
	
	private static bool IsLog(ResourceClump resourceClump)
	{
		return new List<int> { 600, 602 }.Contains(resourceClump.parentSheetIndex.Value);
	}

	private static bool IsBoulder(ResourceClump resourceClump)
	{
		return new List<int> { 148, 622, 672, 752, 754, 756, 758 }.Contains(resourceClump.parentSheetIndex.Value);
	}

	private static bool IsGreenRainBush(ResourceClump resourceClump)
	{
		return new List<int> { 44, 46 }.Contains(resourceClump.parentSheetIndex.Value);
	}
}