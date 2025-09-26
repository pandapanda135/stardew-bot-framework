using Microsoft.Xna.Framework;
using StardewValley.Menus;
using Logger = StardewBotFramework.Debug.Logger;

namespace StardewBotFramework.Source.Modules.MainMenu;

public class MainMenuNavigation
{
    private TitleMenu? _titleMenu;

    public void SetTitleMenu(TitleMenu titleMenu)
    {
        _titleMenu = titleMenu;
    }
    
    public void Exit()
    {
        if (_titleMenu is null) return;
        Rectangle buttonBounds = _titleMenu.buttons[3].bounds; 
        _titleMenu.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }

    public void ExitSubMenu()
    {
        TitleMenu.ReturnToMainTitleScreen();
    }
    
    public void GotoCreateNewCharacter()
    {
        if (_titleMenu is null) return;
        Rectangle buttonBounds = _titleMenu.buttons[0].bounds; 
        _titleMenu.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }
    
    public void GotoLoad()
    {
        if (_titleMenu is null) return;
        Rectangle buttonBounds = _titleMenu.buttons[1].bounds; 
        _titleMenu.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }

    public void GotoMultiplayer()
    {
        if (_titleMenu is null) return;
        Rectangle buttonBounds = _titleMenu.buttons[2].bounds; 
        _titleMenu.receiveLeftClick(buttonBounds.X,buttonBounds.Y);
    }
}