using Microsoft.Xna.Framework;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Menus;

public class BuyAnimalMenu : MenuHandler
{
	public PurchaseAnimalsMenu Menu
	{
		get => _menu as PurchaseAnimalsMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	public bool OnFarm => Menu.onFarm;
	
	public void SetUI(PurchaseAnimalsMenu menu) => Menu = menu;

	public void ExitFarmMenu() => Menu.setUpForReturnToShopMenu();

	public void SelectAnimal(ClickableTextureComponent cc) => LeftClick(cc);

	public void SelectBuilding(Building building)
	{
		Location location = Menu.GetTopLeftPixelToCenterBuilding(building);
		Game1.viewport.Location = location;
		Point tile = TileUtilities.TileToScreen(new Vector2(building.tileX.Value, building.tileY.Value));
		LeftClick(tile.X,tile.Y); // tile to screen
	}

	public void NameAnimal(string name)
	{
		Menu.textBox.Text = name;
	}

	public void RandomName()
	{
		LeftClick(Menu.randomButton);
	}

	public void ConfirmName()
	{
		LeftClick(Menu.doneNamingButton);
	}

	public List<ClickableTextureComponent> GetAvailableButtons()
	{
		List<ClickableTextureComponent>? buttons =
			Menu.animalsToPurchase.Where(cc => (cc.item as Object)?.Type is null).ToList();
		return buttons ?? new();
	}

	public List<Building> GetAnimalBuildings(FarmAnimal animal)
	{
		List<Building> buildings = new();
		foreach (var building in Menu.TargetLocation.buildings)
		{
			if (BuildingCheck(building,animal))
			{
				buildings.Add(building);
			}
		}

		return buildings;
	}

	/// <summary>
	/// Check if the provided animal can live in the provided building
	/// </summary>
	public bool BuildingCheck(Building building,FarmAnimal animal)
	{
		return building.GetIndoors() is AnimalHouse animalHouse && animal.CanLiveIn(building) && !animalHouse.isFull();
	}

	public List<Building> GetAvailableBuildings()
	{
		return GetAnimalBuildings(Menu.animalBeingPurchased);
	}
}