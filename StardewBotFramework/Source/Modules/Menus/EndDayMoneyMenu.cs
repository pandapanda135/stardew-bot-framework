using StardewBotFramework.Debug;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class EndDayShippingMenu
{
    private ShippingMenu? _ShippingMenu;
    
    public void SetMenu(ShippingMenu shippingMenu)
    {
        _ShippingMenu = shippingMenu;
    }

    public void AdvanceToNextDay()
    {
        if (_ShippingMenu is null) return;
        _ShippingMenu.receiveLeftClick(_ShippingMenu.okButton.bounds.X + 5,_ShippingMenu.okButton.bounds.Y + 5);
    }

    public void PressForwardArrow(bool back)
    {
        if (_ShippingMenu is null) return;
        _ShippingMenu.receiveLeftClick(_ShippingMenu.forwardButton.bounds.X + 5, _ShippingMenu.forwardButton.bounds.Y + 5);
    }

    public void PressBackArrow()
    {
        
        if (_ShippingMenu is null) return;
        _ShippingMenu.receiveLeftClick(_ShippingMenu.backButton.bounds.X + 5, _ShippingMenu.backButton.bounds.Y + 5);
    }
    public void OpenItemTypeMenu(int index)
    {
        if (_ShippingMenu is null) return;
        if (index > _ShippingMenu.categories.Count)
        {
            Logger.Error($"{index} is larger than the amount of categories there are");
        }
        _ShippingMenu.receiveLeftClick(_ShippingMenu.categories[index].bounds.X + 5,_ShippingMenu.categories[index].bounds.Y + 5);
    }
}