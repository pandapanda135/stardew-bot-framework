using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Source.Modules.Menus;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;
using xTile.Dimensions;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

public class CreateFarmBuilding : CraftingMenu
{
    public CarpenterMenu CarpenterMenu
    {
        get => _menu as CarpenterMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _menu = value;
    }
    public BuildingSkinMenu BuildingSkinMenu
    {
        get => _childMenu as BuildingSkinMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _childMenu = value;
    }

    public Building Building => CarpenterMenu.currentBuilding;

    public CarpenterMenu.BlueprintEntry? BlueprintEntry => CarpenterMenu.Blueprint;
    
    public void SetCarpenterUi(CarpenterMenu menu) => CarpenterMenu = menu;

    public void SetSkinUi(BuildingSkinMenu menu) => BuildingSkinMenu = menu;
    
    /// <summary>
    /// Change buildings current skin.
    /// </summary>
    /// <param name="skin"><see cref="BuildingSkinMenu.SkinEntry"/></param>
    public void ChangeSkin(BuildingSkinMenu.SkinEntry skin)
    {
        for (int i = 0; i < BuildingSkinMenu.Skins.Count; i++)
        {
            if (BuildingSkinMenu.Skin == skin)
            {
                break;
            }
            LeftClick(BuildingSkinMenu.NextSkinButton);
        }
    }

    public void ChangeBuilding(CarpenterMenu.BlueprintEntry blueprintEntry)
    {
        if (blueprintEntry == CarpenterMenu.Blueprint) return;
        int blueprintIndex = CarpenterMenu.Blueprints.IndexOf(blueprintEntry);
        int changeIndex = blueprintIndex - CarpenterMenu.Blueprints.IndexOf(CarpenterMenu.Blueprint);
        for (int i = 0; i < changeIndex; i++)
        {
            MoveBluePrintCarouselRight();
        }
    }
    
    /// <summary>
    /// use button from <see cref="StardewValley.Menus.CarpenterMenu"/>
    /// </summary>
    public void InteractWithButton(ClickableComponent button) => LeftClick(button);

    public void MoveBluePrintCarouselLeft() => LeftClick(CarpenterMenu.backButton);
    
    public void MoveBluePrintCarouselRight() => LeftClick(CarpenterMenu.forwardButton);

    public void SetSelectedTile(Point tile)
    {
        Game1.viewport.X = (tile.X * Game1.tileSize) - Game1.viewport.X;
        Game1.viewport.Y = tile.Y * Game1.tileSize - Game1.viewport.Y;
        Game1.oldMouseState = new MouseState((tile.X * Game1.tileSize) - Game1.viewport.X,
            (tile.Y * Game1.tileSize) - Game1.viewport.Y, 0, ButtonState.Released,
            ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
    }
    /// <summary>
    /// Check if building can be placed here, this checks the Game1.oldMouseState for where you want to place the building
    /// </summary>
    public bool CanPlaceBuilding(Building building)
    {
        Vector2 tileLocation = new Vector2((Game1.viewport.X + Game1.getOldMouseX(false)) / 64f, (Game1.viewport.Y + Game1.getOldMouseY(false)) / 64f);

        return BuildingUtilities.CanBuildHere(building, tileLocation);
    }
    
    /// <summary>
    /// Create building at tile, the tile should be the middle of the building you want to make.
    /// </summary>
    /// <param name="tile">Top left tile of building</param>
    /// <returns>Will return true if, can build a building at tile location else false</returns>
    public bool CreateBuilding(Point tile)
    {
        SetSelectedTile(tile);
        // tryToBuild uses oldMouseState

        if (!CanPlaceBuilding(CarpenterMenu.currentBuilding))
        {
            return false;
        }
        
        bool tryToBuild = CarpenterMenu.tryToBuild();
        if (!tryToBuild)
        {
            return false;
        }
        
        CarpenterMenu.ConsumeResources();
        DelayedAction.functionAfterDelay(CarpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild, 2000);
        CarpenterMenu.freeze = true;
        return true;
    }
    
    /// <summary>
    /// This can be used for demolishing, painting and upgrading buildings 
    /// </summary>
    /// <param name="building">Building to select</param>
    public void SelectBuilding(Building building)
    {
        var menu = new PurchaseAnimalsMenu(new List<Object>(),CarpenterMenu.TargetLocation);
        Location location = menu.GetTopLeftPixelToCenterBuilding(building);
        Game1.viewport.Location = location;
        Point tile = new Point(building.tileX.Value, building.tileY.Value);
        Point screenTile = TileUtilities.TileToScreen(new Vector2(tile.X, tile.Y));
        Game1.oldMouseState = new MouseState(screenTile.X, screenTile.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
        LeftClick(screenTile.X,screenTile.Y);
    }

    /// <summary>
    /// The currently displayed blueprint's required resources
    /// </summary>
    public List<BuildingMaterial> BluePrintResources() => CarpenterMenu.Blueprint.BuildMaterials;

    /// <summary>
    /// All blueprint's required resources 
    /// </summary>
    /// <returns>key will be the translated display name of the blueprint</returns>
    public Dictionary<string, List<BuildingMaterial>> AllBluePrintResources()
    {
        Dictionary<string, List<BuildingMaterial>> buildingMaterials = new();
        foreach (var blueprint in CarpenterMenu.Blueprints)
        {
            buildingMaterials.Add(blueprint.DisplayName,blueprint.BuildMaterials);
        }

        return buildingMaterials;
    }

    /// <summary>
    /// Get the current building's resources this should be used when you are creating a building
    /// </summary>
    public List<Item> CurrentBuildingResources() => CarpenterMenu.ingredients;

    public Building SkinMenuBuiding() => BuildingSkinMenu.Building;
    
    public List<BuildingSkinMenu.SkinEntry> GetBuildingSkins() => BuildingSkinMenu.Skins;

    public void MoveCarousel(bool left)
    {
        ClickableComponent cc = left ? BuildingSkinMenu.NextSkinButton : BuildingSkinMenu.PreviousSkinButton;
        LeftClick(cc);
    }

    public void ConfirmSkinAndExit()
    {
        LeftClick(BuildingSkinMenu.OkButton);
        _childMenu = null;
    }
}