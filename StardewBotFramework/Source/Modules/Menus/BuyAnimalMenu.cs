using System.Net.NetworkInformation;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules.Menus;

public class BuyAnimalMenu
{
	public PurchaseAnimalsMenu? Menu;

	public bool OnFarm
	{
		get
		{
			if (Menu is null) return false;
			return Menu.onFarm;
		}
	}


	public void SetUI(PurchaseAnimalsMenu menu)
	{
		Menu = menu;
	}

	public void ExitStoreMenu()
	{
		Menu?.exitThisMenu();
		Menu = null;
	}

	public void ExitFarmMenu()
	{
		Menu?.setUpForReturnToShopMenu();
	}

	public void SelectAnimal(ClickableTextureComponent cc)
	{
		Menu?.receiveLeftClick(cc.bounds.X,cc.bounds.Y);	
	}

	public void SelectBuilding(Building building)
	{
		if (Menu is null) return;

		Location location = Menu.GetTopLeftPixelToCenterBuilding(building); // TODO: maybe use this for other building stuff?
		Game1.viewport.Location = location;
		Menu.receiveLeftClick(building.tileX.Value * 64, building.tileY.Value * 64);
	}

	public void NameAnimal(string name)
	{
		if (Menu is null) return;
		Menu.textBox.Text = name;
	}

	public void RandomName()
	{
		if (Menu is null) return;
		Menu.receiveLeftClick(Menu.randomButton.bounds.X,Menu.randomButton.bounds.Y);
	}

	public void ConfirmName()
	{
		if (Menu is null) return;		
		ClickableComponent cc = Menu.doneNamingButton;
		Menu.receiveLeftClick(cc.bounds.X,cc.bounds.Y);
	}

	public List<ClickableTextureComponent> GetAvailableButtons()
	{
		List<ClickableTextureComponent>? buttons =
			Menu?.animalsToPurchase.Where(cc => (cc.item as Object)?.Type is null).ToList();
		return buttons ?? new();
	}

	public List<Building> GetAnimalBuildings(FarmAnimal animal)
	{
		if (Menu is null) return new();
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

	public bool BuildingCheck(Building building,FarmAnimal animal)
	{
		if (building.GetIndoors() is AnimalHouse animalHouse && animal.CanLiveIn(building))
		{
			return true;
		}

		return false;
	}

	public List<Building> GetAvailableBuildings()
	{
		if (Menu is null) return new();
		return GetAnimalBuildings(Menu.animalBeingPurchased);
	}
}