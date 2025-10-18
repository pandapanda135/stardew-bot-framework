using StardewBotFramework.Debug;
using StardewBotFramework.Source.Utilities;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class EndDayShippingMenu : MenuHandler
{
    public ShippingMenu Menu
    {
        get => _menu as ShippingMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _menu = value;
    }
    
    public void SetMenu(ShippingMenu shippingMenu) => Menu = shippingMenu;

    public void AdvanceToNextDay() => LeftClick(Menu.okButton);

    public void PressForwardArrow() => LeftClick(Menu.forwardButton);

    public void PressBackArrow() => LeftClick(Menu.backButton);
    
    public void OpenItemTypeMenu(int index)
    {
        if (index > Menu.categories.Count)
        {
            Logger.Error($"{index} is larger than the amount of categories there are");
        }
        LeftClick(Menu.categories[index]);
    }
}