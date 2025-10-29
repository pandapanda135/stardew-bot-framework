using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewBotFramework.Source.ObjectDestruction;

public class DestroyTerrainFeature
{
	public static void Destroy(TerrainFeature terrainFeature)
	{
		GameLocation location = BotBase.CurrentLocation;

		Logger.Info($"terrain feature in destroy: {terrainFeature.GetType()}");
		if (terrainFeature is Bush or Grass)
		{
			do
			{ 
				DestroyObject.UseTool();
			} while (location.largeTerrainFeatures.Any(node => node.getBoundingBox().Contains(terrainFeature.Tile)));
		}

		switch (terrainFeature)
		{
			case Tree tree:
				if (tree.growthStage.Value < 3 && !tree.stump.Value)
				{
					DestroyObject.UseTool();
					break;
				}
				
				while (tree.health.Value > 0)
				{
					if (BotBase.Farmer.UsingTool) continue;
					
					DestroyObject.UseTool();
				}
				
				Logger.Info($"tree stump: {tree.stump.Value}");
				while (!tree.stump.Value && tree.growthStage.Value >= 5) // after 5 only the chance of moss growing increases 
				{}
				
				if (tree.stump.Value)
				{
					Logger.Info($"running if");
					while (tree.falling.Value)
					{}
					
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
			case Grass grass:
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