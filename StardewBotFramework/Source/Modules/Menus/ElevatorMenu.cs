using StardewBotFramework.Source.Utilities;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class ElevatorMenu : MenuHandler
{
	public MineElevatorMenu Menu
	{
		get => _menu as MineElevatorMenu ?? throw new InvalidOperationException("PurchaseAnimalsMenu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	public void SetMenu(MineElevatorMenu menu)
	{
		Menu = menu;
	}

	public new void RemoveMenu()
	{
		_menu = null;
	}

	public void SelectButton(int index) => LeftClick(Menu.elevators[index]);
}