using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class EndDaySkillMenu
{
    private LevelUpMenu? _levelUpMenu;

    public int CurrentSkill => BotBase.GetLevelUpMenuSkill();
    public int CurrentLevel => BotBase.GetLevelUpMenuLevel();

    public List<int> ProfessionsToChoose => BotBase.GetLevelUpMenuProfessions();
    
    public void SetMenu(LevelUpMenu levelUpMenu)
    {
        _levelUpMenu = levelUpMenu;
    }

    public void SelectPerk(bool left)
    {
        if (_levelUpMenu is null) return;
        if (left)
        {
            _levelUpMenu.receiveLeftClick(_levelUpMenu.leftProfession.bounds.X + 5,_levelUpMenu.leftProfession.bounds.Y + 5);
        }
        else
        {
            _levelUpMenu.receiveLeftClick(_levelUpMenu.rightProfession.bounds.X + 5,_levelUpMenu.rightProfession.bounds.Y + 5);
        }
    }

    public void SelectOkButton()
    {
        if (_levelUpMenu is null) return;
        _levelUpMenu.receiveLeftClick(_levelUpMenu.okButton.bounds.X + 5,_levelUpMenu.okButton.bounds.Y + 5);
        _levelUpMenu = null;
    }
}