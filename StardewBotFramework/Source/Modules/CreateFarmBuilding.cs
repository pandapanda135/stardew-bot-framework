using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class CreateFarmBuilding
{
    public CarpenterMenu? _carpenterMenu;
    public BuildingSkinMenu? _buildingSkinMenu;

    public Building Building
    {
        get
        {
            if (_carpenterMenu is null) return new Building();
            return _carpenterMenu.currentBuilding;
        }
    }

    public CarpenterMenu.BlueprintEntry BlueprintEntry
    {
        get
        {
            if (_carpenterMenu is null) return null;
            return _carpenterMenu.Blueprint;
        }
    }
    public void SetCarpenterUI(CarpenterMenu menu)
    {
        _carpenterMenu = menu;
    }

    public void SetBuildingUI(BuildingSkinMenu menu)
    {
        _buildingSkinMenu = menu;
    }

    /// <summary>
    /// Change buildings current skin.
    /// </summary>
    /// <param name="skin"><see cref="BuildingSkinMenu.SkinEntry"/></param>
    public void ChangeSkin(BuildingSkinMenu.SkinEntry skin)
    {
        if (_buildingSkinMenu is null || _carpenterMenu is null) return;
        for (int i = 0; i < _buildingSkinMenu.Skins.Count; i++)
        {
            if (_buildingSkinMenu.Skin == skin)
            {
                break;
            }
            _buildingSkinMenu.receiveLeftClick(_buildingSkinMenu.NextSkinButton.bounds.X,_buildingSkinMenu.NextSkinButton.bounds.Y);
        }

        _carpenterMenu.SetChildMenu(null);
    }

    public void ChangeBuilding(CarpenterMenu.BlueprintEntry blueprintEntry)
    {
        if (_carpenterMenu is null) return;
        if (blueprintEntry == _carpenterMenu.Blueprint) return;
        int blueprintIndex = _carpenterMenu.Blueprints.IndexOf(blueprintEntry);
        int changeIndex = blueprintIndex - _carpenterMenu.Blueprints.IndexOf(_carpenterMenu.Blueprint);
        for (int i = 0; i < changeIndex; i++)
        {
            MoveBluePrintCarouselRight();
        }
    }
    
    /// <summary>
    /// use button from <see cref="CarpenterMenu"/>
    /// </summary>
    public void InteractWithButton(ClickableComponent button)
    {
        if (_carpenterMenu is null) return;
        Rectangle bounds = button.bounds;
        _carpenterMenu.receiveLeftClick(bounds.X + 5,bounds.Y + 5);
    }

    public void MoveBluePrintCarouselLeft()
    {
        if (_carpenterMenu is null) return;
        Rectangle bounds = _carpenterMenu.backButton.bounds;
        _carpenterMenu.receiveLeftClick(bounds.X + 5,bounds.Y + 5);
        Console.WriteLine($"moving left");
    }
    
    public void MoveBluePrintCarouselRight()
    {
        if (_carpenterMenu is null) return;
        Rectangle bounds = _carpenterMenu.forwardButton.bounds;
        _carpenterMenu.receiveLeftClick(bounds.X + 1,bounds.Y + 1);
        Console.WriteLine($"moving right");
    }
    
    /// <summary>
    /// Create building at tile, the tile should be the middle of the building you want to make.
    /// </summary>
    /// <param name="tile">Top left tile of building</param>
    /// <returns>Will return true if, can build a building at tile location else false</returns>
    public bool CreateBuilding(Point tile)
    {
        if (_carpenterMenu is null) return false;
        Game1.viewport.X = (tile.X * Game1.tileSize) - Game1.viewport.X;
        Game1.viewport.Y = tile.Y * Game1.tileSize - Game1.viewport.Y;
        Game1.oldMouseState = new MouseState((tile.X * Game1.tileSize) - Game1.viewport.X, (tile.Y * Game1.tileSize) - Game1.viewport.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
        // tryToBuild uses oldMouseState

        bool tryToBuild = _carpenterMenu.tryToBuild();
        if (!tryToBuild)
        {
            return false;
        }
        
        _carpenterMenu.ConsumeResources();
        DelayedAction.functionAfterDelay(_carpenterMenu.returnToCarpentryMenuAfterSuccessfulBuild, 2000);
        _carpenterMenu.freeze = true;
        return true;
    }
    
    /// <summary>
    /// This can be used for demolishing, painting and upgrading buildings 
    /// </summary>
    /// <param name="tile">Should try to be at the middle point of the building</param>
    public void SelectBuilding(Point tile)
    {
        if (_carpenterMenu is null) return;
        Game1.viewport.X = tile.X * Game1.tileSize;
        Game1.viewport.Y = tile.Y * Game1.tileSize;
        Game1.oldMouseState = new MouseState((tile.X * Game1.tileSize) - Game1.viewport.X, (tile.Y * Game1.tileSize) - Game1.viewport.Y, 0, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released, ButtonState.Released);
        _carpenterMenu.receiveLeftClick(tile.X * 64, tile.Y * 64);
    }

    /// <summary>
    /// The currently displayed blueprint's required resources
    /// </summary>
    public List<BuildingMaterial> BluePrintResources()
    {
        if (_carpenterMenu is null) return new ();
        return _carpenterMenu.Blueprint.BuildMaterials;
    }

    /// <summary>
    /// All blueprint's required resources 
    /// </summary>
    /// <returns>key will be the translated display name of the blueprint</returns>
    public Dictionary<string, List<BuildingMaterial>> AllBluePrintResources()
    {
        if (_carpenterMenu is null) return new ();
        Dictionary<string, List<BuildingMaterial>> buildingMaterials = new();
        foreach (var blueprint in _carpenterMenu.Blueprints)
        {
            buildingMaterials.Add(blueprint.DisplayName,blueprint.BuildMaterials);
        }

        return buildingMaterials;
    }

    /// <summary>
    /// Get the current building's resources this should be used when you are creating a building
    /// </summary>
    public List<Item> CurrentBuildingResources()
    {
        if (_carpenterMenu is null) return new ();
        return _carpenterMenu.ingredients;
    }

    public Building BuildingSkin()
    {
        if (_buildingSkinMenu is null) return new Building();
        return _buildingSkinMenu.Building;
    }

    public List<BuildingSkinMenu.SkinEntry> GetBuildingSkins()
    {
        if (_buildingSkinMenu is null) return new ();
        return _buildingSkinMenu.Skins;
    }

    public void MoveCarousel(bool left)
    {
        if (_buildingSkinMenu is null) return;
        Rectangle bounds = _buildingSkinMenu.NextSkinButton.bounds;
        if (left)
        {
            _buildingSkinMenu.receiveLeftClick(bounds.X + 5,bounds.Y + 5);
        }
        else
        {
            _buildingSkinMenu.receiveLeftClick(bounds.X + 5,bounds.Y + 5);
        }
    }

    public void ExitSkinMenu()
    {
        if (_buildingSkinMenu is null) return;
        _buildingSkinMenu.receiveLeftClick(_buildingSkinMenu.OkButton.bounds.X + 5,_buildingSkinMenu.OkButton.bounds.Y + 5);
    }
}