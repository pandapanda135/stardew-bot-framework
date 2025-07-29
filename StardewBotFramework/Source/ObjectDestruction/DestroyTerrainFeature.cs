using System.Diagnostics;
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
		if (terrainFeature is Bush)
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
					DestroyObject.UseTool();
				}
				
				Logger.Info($"tree stump: {tree.stump.Value}");
				while (!tree.stump.Value && tree.growthStage.Value >= 5) // after 5, the chance of moss growing increases 
				{} // sometimes this doesn't work idk why
				if (tree.stump.Value)
				{
					Logger.Info($"running if");
					while (tree.falling.Value)
					{}
					
					while (tree.health.Value > 0) // we do this in case there are stumps. ugly solution but it works
					{
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
}