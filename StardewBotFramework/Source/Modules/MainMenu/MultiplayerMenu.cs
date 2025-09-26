using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.MainMenu;

public class MultiplayerMenu
{
    private CoopMenu? _multiplayer;

    public void SetTitleMenu(CoopMenu multiplayer)
    {
        _multiplayer = multiplayer;
    }
}