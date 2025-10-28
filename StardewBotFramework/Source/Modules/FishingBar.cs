using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Debug;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Tools;

namespace StardewBotFramework.Source.Modules;

public class FishingBar
{
	internal static event EventHandler? StaticCaughtFish;
	private static FishingRod? _fishingRod;
	private static BobberBar? BobberBar
	{
		get
		{
			if (Game1.activeClickableMenu is not StardewValley.Menus.BobberBar)
			{
				return null;
			}
			return Game1.activeClickableMenu as BobberBar;
		}
	}

	/// <summary>
	/// Start fishing, if the current tool is not a fishing rod this will return false.
	/// </summary>
	/// <param name="power">This should be between 0 and 1, if this is greater or lower than it will be clamped in update to 1 or 0</param>
	public bool Fish(float power)
	{
		return StartFishing(power);
	}

	private static float _selectedPower = -1;
	public static void Update(object? sender, UpdateTickingEventArgs e)
	{
		if (BotBase.Farmer.CurrentTool is FishingRod rod) _fishingRod = rod;
		else return;

		if (_fishingRod.fishCaught)
		{
			if (StaticCaughtFish is null) return;
			StaticCaughtFish.Invoke(new FishingBar(),EventArgs.Empty);
		}
		
		if (!BotBase.Farmer.UsingTool && _selectedPower < 0) return;

		if (_fishingRod.isNibbling && !_fishingRod.hit && (!_fishingRod.isReeling || !_fishingRod.isFishing)) 
		{
			BotBase.Farmer.CurrentTool.DoFunction(BotBase.CurrentLocation,0,0,0,BotBase.Farmer);
		}
		
		if (_fishingRod.castingPower < Math.Clamp(_selectedPower,0,0.98))
		{
			BotBase.Instance?.Helper.Input.OverrideButton(SButton.MouseLeft, true);
		}
		else _selectedPower = -1;
		
		if (BobberBar is null) return;
		
		var barCenter = BobberBar.bobberBarPos + BobberBar.bobberBarHeight / 3f; // this should be the top quarter of the bar
		if (barCenter <= BobberBar.bobberPosition) // the higher the bar is the lower the pos
		{
			Logger.Info($"returning: {Game1.oldMouseState.LeftButton}");
			return;
		}
		
		// set left click to pressed
		Logger.Info($"setting old mouse");
		Game1.oldMouseState = new MouseState(0, 0, 0, ButtonState.Pressed, ButtonState.Released,
			ButtonState.Released, ButtonState.Released, ButtonState.Released);
	}
	
	private bool StartFishing(float power)
	{
		if (BotBase.Farmer.CurrentTool is not StardewValley.Tools.FishingRod) return false;
		
		_fishingRod = BotBase.Farmer.CurrentTool as FishingRod ?? new FishingRod();
		_selectedPower = power;
		
		return true;
	}

	public void CloseRewardMenu()
	{
		_fishingRod?.doneHoldingFish(BotBase.Farmer);
	}
}