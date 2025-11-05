using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class CraftingMenu : MenuHandler
{
	public CraftingPage Menu
	{
		get => _menu as CraftingPage ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
		private set => _menu = value;
	}

	public int CurrentPage => Menu.currentCraftingPage;

	public void SetUI(CraftingPage menu) => Menu = menu;

	public List<Dictionary<ClickableTextureComponent, CraftingRecipe>> GetAllItems() => Menu.pagesOfCraftingRecipes;

	public Dictionary<ClickableTextureComponent, CraftingRecipe> GetPageComponents() => GetAllItems()[CurrentPage];

	public List<CraftingRecipe> GetPageItems() => Menu.pagesOfCraftingRecipes[CurrentPage].Values.ToList();

	public void SetPageUI()
	{
		// this is based on .pages index
		GameMenu gameMenu = new GameMenu(4);
		Game1.activeClickableMenu = gameMenu;
		
		if (gameMenu.GetCurrentPage() is CraftingPage page)
		{
			SetUI(page);
		}
	}

	public bool CraftItem(CraftingRecipe recipe,int amount = 1)
	{
		List<ClickableTextureComponent> components = GetPageComponents().Keys.ToList(); 
		int index = GetPageItems().Select(r => r.createItem().ItemId).ToList().IndexOf(recipe.createItem().ItemId);
		if (index == -1)
		{
			Logger.Error($"Index was -1");
			return false;
		}
		for (int i = 0; i < amount; i++)
		{
			LeftClick(components[index]);
		}
		return true;
	}

	public void ChangePage(bool up)
	{
		Logger.Info($"changing page");
		ClickableComponent cc = up ? Menu.upButton : Menu.downButton;
		LeftClick(cc);
	}
}