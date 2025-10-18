using StardewBotFramework.Source.Utilities;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

/// <summary>
/// for <see cref="NamingMenu"/>, this is used in a few scenarios like naming horses, naming children and using signs.
/// You can see all of its uses in the stardew source.
/// </summary>
public class NamingMenuInteraction : MenuHandler
{
	public NamingMenu Menu
	{
		get => _menu as NamingMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	public void setUI(NamingMenu menu) => Menu = menu;

	public void ChangeName(string name) => Menu.textBox.Text = name;

	public void DoneNaming() => LeftClick(Menu.doneNamingButton);

	public void RandomizeName() => LeftClick(Menu.randomButton);
}