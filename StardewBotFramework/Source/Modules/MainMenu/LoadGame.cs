using StardewValley.Menus;
using Logger = StardewBotFramework.Debug.Logger;

namespace StardewBotFramework.Source.Modules.MainMenu;

public class LoadGame
{
    public List<LoadGameMenu.MenuSlot> MenuSlots => _loadGameMenu.MenuSlots;

    public int SaveSlotAmount => _loadGameMenu.MenuSlots.Count;
    
    private LoadGameMenu _loadGameMenu = null!;

    public void SetLoadMenu(LoadGameMenu loadGameMenu)
    {
        _loadGameMenu = loadGameMenu;
    }

    public void LoadSlot(int save) // does not have fade in and out doesn't affect load time so it's fine
    {
        MenuSlots[save].Activate();
    }
}