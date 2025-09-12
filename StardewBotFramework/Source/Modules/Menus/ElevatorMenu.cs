using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class ElevatorMenu
{
	public MineElevatorMenu Menu = null!;

	public void SetMenu(MineElevatorMenu menu)
	{
		Menu = menu;
	}

	public void RemoveMenu()
	{
		Menu = null!;
	}

	public void SelectButton(int index)
	{
		ClickableComponent cc = Menu.elevators[index];
		Menu.receiveLeftClick(cc.bounds.X,cc.bounds.Y);
	}
}