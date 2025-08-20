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
        if (left) // we do this because this uses mouse state instead of how everything else in the game works.
        {
            Game1.player.professions.Add(ProfessionsToChoose[0]);
            _levelUpMenu.getImmediateProfessionPerk(ProfessionsToChoose[0]);
        }
        else
        {
            Game1.player.professions.Add(ProfessionsToChoose[1]);
            _levelUpMenu.getImmediateProfessionPerk(ProfessionsToChoose[1]);
        }

        _levelUpMenu.isActive = false;
        _levelUpMenu.informationUp = false;
        _levelUpMenu.isProfessionChooser = false;
        _levelUpMenu.RemoveLevelFromLevelList();
    }

    public void SelectOkButton()
    {
        if (_levelUpMenu is null) return;
        _levelUpMenu.okButtonClicked();
        // _levelUpMenu.receiveLeftClick(_levelUpMenu.okButton.bounds.X + 5,_levelUpMenu.okButton.bounds.Y + 5);
        _levelUpMenu = null;
    }
}