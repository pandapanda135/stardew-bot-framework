using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

/// <summary>
/// for <see cref="NamingMenu"/>, this is used in a few scenarios like naming horses, naming children and using signs.
/// You can see all of its uses in the stardew source.
/// </summary>
public class NamingMenuInteraction
{
	public NamingMenu Menu = null!;

	public void setUI(NamingMenu menu)
	{
		Menu = menu;
	}

	public void ChangeName(string name)
	{
		Menu.textBox.Text = name;
	}

	public void DoneNaming()
	{
		Menu.receiveLeftClick(Menu.doneNamingButton.bounds.X,Menu.doneNamingButton.bounds.Y);
	}

	public void RandomizeName()
	{
		Menu.receiveLeftClick(Menu.randomButton.bounds.X,Menu.randomButton.bounds.Y);
	}
}