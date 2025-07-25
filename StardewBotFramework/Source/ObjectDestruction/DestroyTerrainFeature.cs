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
			DestroyObject.UseTool();
		}

		switch (terrainFeature)
		{
			case Tree tree:
				while (tree.health.Value > 0)
				{
					DestroyObject.UseTool();
				}
				while (tree.health.Value > 0) // we do this in case there are stumps. ugly solution but it works
				{
					DestroyObject.UseTool();
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