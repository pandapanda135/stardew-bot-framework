using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class EndDaySkillMenu
{
    private LevelUpMenu? _LevelUpMenu;
    
    public void SetMenu(LevelUpMenu levelUpMenu)
    {
        _LevelUpMenu = levelUpMenu;
    }

    public void SelectPerk(bool left)
    {
        if (_LevelUpMenu is null) return;
        if (left)
        {
            _LevelUpMenu.receiveLeftClick(_LevelUpMenu.leftProfession.bounds.X + 5,_LevelUpMenu.leftProfession.bounds.X + 5);
        }
        else
        {
            _LevelUpMenu.receiveLeftClick(_LevelUpMenu.leftProfession.bounds.X + 5,_LevelUpMenu.leftProfession.bounds.X + 5);
        }
    }

    public void SelectOkButton()
    {
        if (_LevelUpMenu is null) return;
        _LevelUpMenu.receiveLeftClick(_LevelUpMenu.okButton.bounds.X + 5,_LevelUpMenu.okButton.bounds.Y + 5);
        _LevelUpMenu = null;
    }
}