using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.TerrainFeatures;

namespace StardewBotFramework.Source.ObjectDestruction;

public static class DestroyTerrainFeature
{
	public static void Destroy(TerrainFeature terrainFeature)
	{
		GameLocation location = BotBase.CurrentLocation;
		if (!location.terrainFeatures.TryGetValue(terrainFeature.Tile, out var value) || value is null)
		{
			return;
		}

		Logger.Info($"terrain feature in destroy: {terrainFeature.GetType()}");
		if (terrainFeature is Bush or Grass)
		{
			Logger.Info($"terrain feature is bush or grass");
			do
			{
				if (BotBase.Farmer.UsingTool) continue;
				
				DestroyObject.UseTool();
			} while (location.largeTerrainFeatures.Any(node => node.getBoundingBox().Contains(terrainFeature.Tile)));
		}

		switch (terrainFeature)
		{
			case Tree tree:
				Logger.Info($"terrain feature is tree");
				if (tree.growthStage.Value < 3 && !tree.stump.Value)
				{
					DestroyObject.UseTool();
					break;
				}
				
				// destroy standing tree
				while (tree.health.Value > 0)
				{
					if (BotBase.Farmer.UsingTool) continue;
					
					DestroyObject.UseTool();
				}
				
				while (!tree.stump.Value && tree.growthStage.Value >= 5) // after 5 only the chance of moss growing increases 
				{}
				
				while (tree.falling.Value)
				{}
				
				if (tree.stump.Value)
				{
					while (tree.falling.Value)
					{}
					
					Logger.Info($"stump health value: {tree.health}");
					while (tree.health.Value > 0) // we do this in case there are stumps. ugly solution but it works
					{
						if (BotBase.Farmer.UsingTool) continue;
						
						DestroyObject.UseTool();
					}
				}

				break;
			case HoeDirt dirt:
				if (dirt.crop is null) return;
				DestroyObject.UseTool(); // I guess this is for destroying crops?
				break;
			case Grass:
				Logger.Info($"destroying grass");
				DestroyObject.UseTool();
				break;
		}
	}

	public static bool IsDestructible(Point tile)
	{
		foreach (var dict in BotBase.CurrentLocation.terrainFeatures)
		{
			if (dict.ContainsKey(tile.ToVector2()))
			{
				TerrainFeature terrainFeature = dict[tile.ToVector2()];
				
				switch (terrainFeature)
				{
					case Tree:
						return true;
					case Bush:
						return true;
					case HoeDirt:
						return true;
					case Grass:
						return true;
				}
			}
		}

		return false;
	}
	
	public static bool IsDestructible(TerrainFeature terrainFeature)
	{
		switch (terrainFeature)
		{
			case Tree:
				return true;
			case Bush:
				return true;
			case HoeDirt:
				return true;
			case Grass:
				return true;
		}

		return false;
	}
}