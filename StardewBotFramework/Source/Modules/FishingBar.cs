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
	internal static event EventHandler? StaticCaughtFish;
	private static FishingRod? fishingRod;
	static BobberBar? bobberBar
	{
		get
		{
			if (Game1.activeClickableMenu is not BobberBar)
			{
				return null;
			}
			return Game1.activeClickableMenu as BobberBar;
		}
	}

	public bool Fish(int power)
	{
		return StartFishing(power);
	}

	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		if (BotBase.Farmer.CurrentTool is FishingRod rod)
		{
			fishingRod = rod;
		}
		else
		{
			return;
		}

		if (fishingRod.fishCaught)
		{
			if (StaticCaughtFish is null) return;
			StaticCaughtFish.Invoke(new FishingBar(),EventArgs.Empty);
		}
		
		if (!BotBase.Farmer.UsingTool || BotBase.Farmer.CurrentTool is not FishingRod) return;

		if (fishingRod.isNibbling && !fishingRod.hit && (!fishingRod.isReeling || !fishingRod.isFishing)) 
		{
			BotBase.Farmer.CurrentTool.DoFunction(BotBase.CurrentLocation,0,0,0,BotBase.Farmer);
		}
		
		if (bobberBar is null) return;
		
		var barCenter = bobberBar.bobberBarPos + bobberBar.bobberBarHeight / 3f; // this should be the top quarter of the bar
		if (barCenter <= bobberBar.bobberPosition) // the higher the bar is the lower the pos
		{
			Logger.Info($"returning: {Game1.oldMouseState.LeftButton}");
			return;
		}
		
		// set left click to pressed
		Logger.Info($"setting old mouse");
		Game1.oldMouseState = new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released,
			ButtonState.Released, ButtonState.Released, ButtonState.Released);
	}

	private bool StartFishing(int power)
	{
		if (BotBase.Farmer.CurrentTool is not FishingRod)
		{
			SwapItemHandler.SwapItem(typeof(FishingRod),"");
			if (BotBase.Farmer.CurrentItem is not FishingRod) return false; // if no fishing rod in inv
		}
		
		fishingRod = BotBase.Farmer.CurrentTool as FishingRod ?? new FishingRod();
		BotBase.Farmer.BeginUsingTool();
		fishingRod.castingPower = power; // TODO: this solution is a bit jank as it will just teleport to be full. A better way would be using Update idk how to keep it running across frames though rn
		
		return true;
	}

	public void CloseRewardMenu()
	{
		fishingRod?.doneHoldingFish(BotBase.Farmer);
	}
}