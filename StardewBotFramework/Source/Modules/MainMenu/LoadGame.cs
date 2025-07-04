using StardewValley.Menus;
using Logger = StardewBotFramework.Debug.Logger;

namespace StardewBotFramework.Source.Modules.MainMenu;

public class LoadGame
{
    public List<LoadGameMenu.MenuSlot> MenuSlots
    {
        get
        {
            if (Loading || _loadGameMenu.MenuSlots is null || _loadGameMenu.MenuSlots.Count == 0)
            {
                return new List<LoadGameMenu.MenuSlot>();
            }

            return _loadGameMenu.MenuSlots;
        }
    }

    public int SaveSlotAmount => _loadGameMenu.MenuSlots.Count;

    public bool Loading => _loadGameMenu.loading;
    
    private LoadGameMenu _loadGameMenu = null!;

    public void SetLoadMenu(LoadGameMenu loadGameMenu)
    {
        _loadGameMenu = loadGameMenu;
    }

    /// <summary>
    /// Load from save slot 
    /// </summary>
    /// <param name="save"></param>
    public void LoadSlot(int save) // does not have fade in and out doesn't affect load time so it's fine
    {
        if (MenuSlots.Count == 0)
        {
            return;
        }
        
        MenuSlots[save].Activate();
    }
}