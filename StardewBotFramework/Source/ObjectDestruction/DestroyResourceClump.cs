using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.ObjectDestruction;

public static class DestroyResourceClump
{
	public static void Destroy(ResourceClump clump)
	{
		GameLocation location = BotBase.CurrentLocation;
		if (!location.resourceClumps.Contains(clump))
		{
			return;
		}

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
				switch (clump.parentSheetIndex.Value)
				{
					case 600:
						if (BotBase.Farmer.CurrentTool.UpgradeLevel < 1) return;
						break;
					case 602:
						if (BotBase.Farmer.CurrentTool.UpgradeLevel < 2) return;
						break;
					case 622:
						if (BotBase.Farmer.CurrentTool.UpgradeLevel < 3) return;
						break;
					case 672:
						if (BotBase.Farmer.CurrentTool.UpgradeLevel < 2) return;
						break;
				}
				Logger.Info($"Destroying resourceClump");
				if (BotBase.Farmer.UsingTool) continue;
				DestroyObject.UseTool();
				Logger.Info($"after DestroyObject useTool");
			}
		}

		if (IsGreenRainBush(clump))
		{
			DestroyObject.UseTool();
		}
	}

	public static bool IsDestructible(ResourceClump clump)
	{
		if (clump is GiantCrop crop)
		{
			return true;
		}

		if (IsLog(clump) || IsBoulder(clump))
		{
			return true;
		}

		if (IsGreenRainBush(clump))
		{
			return true;
		}

		return false;
	}
	public static bool IsDestructible(Point tile)
	{
		foreach (var resourceClump in BotBase.CurrentLocation.resourceClumps.Where(clump => clump.Tile.ToPoint() == tile))
		{
			if (resourceClump is GiantCrop crop)
			{
				return true;
			}

			if (IsLog(resourceClump) || IsBoulder(resourceClump))
			{
				return true;
			}

			if (IsGreenRainBush(resourceClump))
			{
				return true;
			}
		}

		return false;
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