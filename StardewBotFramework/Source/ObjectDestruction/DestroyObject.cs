namespace StardewBotFramework.Source.ObjectDestruction;

public class DestroyObject
{
	public static void UseTool()
	{
		while (BotBase.Farmer.UsingTool) {}
		// if (BotBase.Farmer.UsingTool)
		// {
		// 	return;
		// }
		
		BotBase.Farmer.BeginUsingTool(); // Object.performToolAction
	}
}