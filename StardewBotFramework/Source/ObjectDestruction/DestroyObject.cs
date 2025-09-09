using StardewBotFramework.Debug;
using StardewValley;

namespace StardewBotFramework.Source.ObjectDestruction;

public class DestroyObject
{
	public static void UseTool()
	{
		if (!BotBase.Farmer.CanMove) { return;}
		while (BotBase.Farmer.UsingTool) { Logger.Info($"running destroy while"); BotBase.Farmer.EndUsingTool();}
		
		Logger.Info($"begin using tool.");
		BotBase.Farmer.BeginUsingTool(); // Object.performToolAction
	}
}