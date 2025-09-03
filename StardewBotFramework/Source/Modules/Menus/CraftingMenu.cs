using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class CraftingMenu
{
	public CraftingPage? Menu;

	public int CurrentPage
	{
		get
		{
			if (Menu is null) return -1;
			return Menu.currentCraftingPage;
		}
	}

	public void SetUI(CraftingPage menu)
	{
		Menu = menu;
	}

	public void ExitUI()
	{
		Menu?.exitThisMenu();
		Menu = null;
	}

	public List<Dictionary<ClickableTextureComponent, CraftingRecipe>> GetAllItems()
	{
		if (Menu is null) return new();
		return Menu.pagesOfCraftingRecipes;
	}

	public Dictionary<ClickableTextureComponent, CraftingRecipe> GetPageComponents()
	{
		if (Menu is null) return new();
		return GetAllItems()[CurrentPage];
	}

	public List<CraftingRecipe> GetPageItems()
	{
		if (Menu is null) return new();
		return Menu.pagesOfCraftingRecipes[CurrentPage].Values.ToList();
	}

	public void SetPageUI()
	{
		GameMenu? gameMenu = new GameMenu();
		Game1.activeClickableMenu = gameMenu;
		gameMenu = Game1.activeClickableMenu as GameMenu;
		gameMenu?.receiveLeftClick(gameMenu.tabs[4].bounds.X + 5,gameMenu.tabs[4].bounds.Y + 5);

		if (gameMenu?.GetCurrentPage() is CraftingPage page)
		{
			SetUI(page);
		}
		return;
	}

	public bool CraftItem(CraftingRecipe recipe,int amount = 1)
	{
		if (Menu is null) return false;

		List<ClickableTextureComponent> components = GetPageComponents().Keys.ToList(); 
		int index = GetPageItems().IndexOf(recipe);
		for (int i = 0; i < amount; i++)
		{
			Menu.receiveLeftClick(components[index].bounds.X,components[index].bounds.Y);
		}
		return true;
	}

	public void ChangePage(bool up)
	{
		if (Menu is null) return;

		Logger.Info($"changing page");
		ClickableComponent cc = up ? Menu.upButton : Menu.downButton;
		Menu.receiveLeftClick(cc.bounds.X,cc.bounds.Y);
	}
}