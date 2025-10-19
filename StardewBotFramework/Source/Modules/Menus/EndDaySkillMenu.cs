using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class EndDaySkillMenu : MenuHandler
{
    public LevelUpMenu Menu
    {
        get => _menu as LevelUpMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _menu = value;
    }

    public int CurrentSkill => BotBase.GetLevelUpMenuSkill();
    public int CurrentLevel => BotBase.GetLevelUpMenuLevel();

    public List<int> ProfessionsToChoose => BotBase.GetLevelUpMenuProfessions();
    
    public void SetMenu(LevelUpMenu levelUpMenu) => Menu = levelUpMenu;
    
    public void SelectPerk(bool left)
    {
        if (left) // we do this because this uses mouse state instead of how everything else in the game works.
        {
            BotBase.Farmer.professions.Add(ProfessionsToChoose[0]);
            Menu.getImmediateProfessionPerk(ProfessionsToChoose[0]);
        }
        else
        {
            BotBase.Farmer.professions.Add(ProfessionsToChoose[1]);
            Menu.getImmediateProfessionPerk(ProfessionsToChoose[1]);
        }

        Menu.isActive = false;
        Menu.informationUp = false;
        Menu.isProfessionChooser = false;
        Menu.RemoveLevelFromLevelList();
    }

    public void SelectOkButton()
    {
        Menu.okButtonClicked();
        _menu = null;
    }
}