using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

// Utility.TryOpenShopMenu("SeedShop", null, true); 
// This is a very jank solution however 

// chargePlayer(Game1.player, currency, -sell_unit_price * toSell.Stack);
    
// BuyBuybackItem(ISalable bought_item, int price, int stack)
    
// switchTab(int new_tab)
    
// tryToPurchaseItem(ISalable item, ISalable held_item, int stockToBuy, int x, int y)
    
// The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).

public class Shop
{
    private ShopMenu? _currentShop;

    /// <summary>
    /// The amount of tabs in this shop. If this returns null then that means _currentShop is not set
    /// </summary>
    public int? TabAmount
    {
        get
        {
            if (_currentShop is not null)
            {
                return _currentShop.tabButtons.Count;
            }

            return null;
        }
    }

    public void OpenShop(ShopMenu shopMenu)
    {
        _currentShop = shopMenu;
    }

    public void CloseShop()
    {
        _currentShop = null;
    }

    public void BuyItem(int index, int quantity)
    {
        _currentShop = (ShopMenu?)Game1.activeClickableMenu;
        
        if (_currentShop is null || _currentShop is not ShopMenu) return;
        
        if (index < 4)
        {
            for (int i = 0; i < quantity; i++)
            { _currentShop.receiveLeftClick(_currentShop.forSaleButtons[index].bounds.X + 10, _currentShop.forSaleButtons[index].bounds.Y + 10, true); }
            return;
        }
        
        // use down arrow
        for (int i = index; i >= 4; i--)
        {
            _currentShop.receiveLeftClick(_currentShop.downArrow.bounds.X, _currentShop.downArrow.bounds.Y, true);   
        }
        
        // buy amount
        int maxIndex = _currentShop.forSaleButtons.Count - 1;
        for (int i = 0; i < quantity; i++)
        {
            Logger.Info($"buying click {i}");
            _currentShop.receiveLeftClick(_currentShop.forSaleButtons[maxIndex].bounds.X, _currentShop.forSaleButtons[maxIndex].bounds.Y, true);    
        }
        
        // go back to top
        for (int i = index; i >= 4; i--)
        {
            Logger.Info($"using down arrow  i: {i}");
            _currentShop.receiveLeftClick(_currentShop.upArrow.bounds.X, _currentShop.upArrow.bounds.Y, true);   
        }
    }

    public void BuyItem(Item item, int quantity)
    {
        for (int i = 0; i < _currentShop.forSale.Count; i++)
        {
            if (_currentShop.forSale[i].Name == item.Name)
            {
                BuyItem(i, quantity);
                return;
            }
        }
    }

    public void ListAllItems()
    {
        _currentShop = (ShopMenu?)Game1.activeClickableMenu;
        
        if (_currentShop is null || _currentShop is not ShopMenu) return;
        
        foreach (var button in _currentShop.forSaleButtons)
        {
            if (button.item != null)
            {
                Logger.Info($"for sale button:  {button.item.Name}");
            }
            Logger.Info(button.label);
            Logger.Info(button.ScreenReaderText);
            Logger.Info(button.ScreenReaderDescription);
            Logger.Info($"Bounds X: {button.bounds.X}  bounds Y: {button.bounds.Y}");
        }
    }

    public Dictionary<ISalable,ItemStockInformation> ForSaleStats(ShopMenu shopMenu,out List<ISalable> items, out int currency)
    {
        currency = shopMenu.currency;
        items = shopMenu.forSale;
        return shopMenu.itemPriceAndStock;
    }
    
    public void ChangeTab(int newTab)
    {
        if (_currentShop.tabButtons.Count == 0) return;
        
        _currentShop.switchTab(newTab);
    }
    
    // click on item slot that has item in it, I think it sells the full stack everytime 
    
    public int SellBackItem(int index) // Item item
    {
        _currentShop = (ShopMenu?)Game1.activeClickableMenu;
        
        if (!_currentShop.CanBuyback()) return 0;

        List<ClickableComponent> inventory = _currentShop.inventory.inventory;
        
        _currentShop.receiveLeftClick(inventory[index].bounds.X + 3,inventory[index].bounds.Y + 3); //TODO: make work
        
        foreach (var button in _currentShop.inventory.inventory)
        {
            Logger.Info($"button name: {button.name} bounds x: {button.bounds.X} bounds Y: {button.bounds.Y}");
            if (button.item != null) Logger.Info($"{button.item}");
        }
        
        return 0;
    }
}