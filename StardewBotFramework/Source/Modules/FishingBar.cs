using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace StardewBotFramework.Source.Modules;

public class FishingBar
{
	static BobberBar? bobberBar => Game1.activeClickableMenu as BobberBar;
	public bool Fish()
	{
		return StartFishing();
	}

	// this is here for making bar move
	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		if (!BotBase.Farmer.UsingTool || BotBase.Farmer.CurrentTool is not FishingRod) return;

		if (bobberBar is null) return;
		// if (bobberBar.bobberInBar) return;
		
		Logger.Info($"running fishing update after if: bobber position: {bobberBar.bobberPosition}  bar pos: {bobberBar.bobberBarPos} target: {bobberBar.bobberTargetPosition}   if: {bobberBar.bobberPosition <= bobberBar.bobberTargetPosition} speed: {bobberBar.bobberSpeed}");
		//  - bobberBar.bobberBarHeight / 2f 
		if (bobberBar.bobberBarPos <= bobberBar.bobberTargetPosition) // the higher the bar is the lower the pos
		{
			Logger.Info($"returning: {Game1.oldMouseState.LeftButton}");
			return;
		}

		// set left click to pressed
		Logger.Info($"setting old mouse");
		Game1.oldMouseState = new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released,
			ButtonState.Released, ButtonState.Released, ButtonState.Released);
	}

	private bool StartFishing()
	{
		if (BotBase.Farmer.CurrentTool is not FishingRod)
		{
			SwapItemHandler.SwapItem(typeof(FishingRod),"");
			if (BotBase.Farmer.CurrentItem is not FishingRod) return false; // if no fishing rod in inv
		}
		
		//use tool and do checks


		// if (Game1.activeClickableMenu is BobberBar bobberbar)
		// {
		// 	bobberBar = bobberbar;
		// }
		
		return true;
	} 
}