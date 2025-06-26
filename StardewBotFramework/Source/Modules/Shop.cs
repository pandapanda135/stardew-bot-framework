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

    /// <summary>
    /// Open selected shop menu, this will need to be called before anything else
    /// </summary>
    /// <param name="shopMenu">Shop to open</param>
    public void OpenShop(ShopMenu shopMenu)
    {
        _currentShop = shopMenu;
    }

    /// <summary>
    /// Close selected shop menu.
    /// </summary>
    public void CloseShop()
    {
        _currentShop = null;
    }

    /// <summary>
    /// Buy an item from the shop based on it's corresponding index in the shop's UI.
    /// </summary>
    /// <param name="index">The index of item to buy.</param>
    /// <param name="quantity">Amount of item to buy.</param>
    public void BuyItem(int index, int quantity)
    {
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

    /// <summary>
    /// Buy an <see cref="Item"/> from the shop.
    /// </summary>
    /// <param name="item">Item to buy.</param>
    /// <param name="quantity">Amount of item to buy.</param>
    public void BuyItem(Item item, int quantity)
    {
        if (_currentShop is null || _currentShop is not ShopMenu) return;
        
        for (int i = 0; i < _currentShop.forSale.Count; i++)
        {
            if (_currentShop.forSale[i].Name == item.Name)
            {
                BuyItem(i, quantity);
                return;
            }
        }
    }

    /// <summary>
    /// Returns all items as their <see cref="ISalable"/> class 
    /// </summary>
    /// <returns>A list of all items for sale however is a shop has not been opened yet, it will return null</returns>
    public List<ISalable>? ListAllItems()
    {
        if (_currentShop is null || _currentShop is not ShopMenu) return null;
        
        return _currentShop.forSale;
    }

    /// <summary>
    /// The stats of the items on sale, this also includes the currency this shop accepts and the available items
    /// </summary>
    /// <param name="items">The available items at this shop.</param>
    /// <param name="currency">the currency this shop accepts.</param>
    /// <returns>The stock info for each item in the shop as a dictionary. If a shop is not open everything will return the lowest possible value</returns>
    public Dictionary<ISalable,ItemStockInformation> ForSaleStats(out List<ISalable> items, out int currency)
    {
        if (_currentShop is null || _currentShop is not ShopMenu)
        {
            currency = -1;
            items = new List<ISalable>();
            return new Dictionary<ISalable, ItemStockInformation>();
        }
        
        currency = _currentShop.currency;
        items = _currentShop.forSale;
        return _currentShop.itemPriceAndStock;
    }
    
    /// <summary>
    /// Change tab of shop.
    /// </summary>
    /// <param name="newTab">index of new tab.</param>
    public void ChangeTab(int newTab)
    {
        if (_currentShop is null || _currentShop is not ShopMenu) return;
        
        if (_currentShop.tabButtons.Count == 0) return;
        
        _currentShop.switchTab(newTab);
    }
    
    /// <summary>
    /// Will try to see back item in the provided index of the bot's inventory
    /// </summary>
    /// <param name="index">this is a value of 0 to max provided by the inventory level these can only be 11-23-35 (assuming the game is not modded)</param>
    /// <returns>The amount the item has sold for. However, if this returns -1 it means there has been an issue with selling item</returns>
    public int SellBackItem(int index) // Item item
    {
        if (_currentShop is null || _currentShop is not ShopMenu) return -1;
        
        if (_currentShop is null) return -1;

        if (!_currentShop.CanBuyback())
        {
            Logger.Error($"shop cannot buy back");
            return -1;
        }

        List<ClickableComponent> inventory = _currentShop.inventory.inventory;

        int itemSalePrice = _currentShop.inventory.actualInventory[index].GetSalableInstance().sellToStorePrice();
        int itemStackSize = _currentShop.inventory.actualInventory[index].Stack;
        
        _currentShop.receiveLeftClick(inventory[index].bounds.X + 3,inventory[index].bounds.Y + 3); //TODO: make work
        
        return itemSalePrice * itemStackSize;
    }

    
    /// <summary>
    /// Will try to see back item in the provided index of the bot's inventory
    /// </summary>
    /// <param name="item">This is the item you want to sell must be in the bot's inventory</param>
    /// <returns>The amount the item has sold for. However, if this returns -1 it means there has been an issue with selling item</returns>
    public int SellBackItem(Item item)
    {
        if (Game1.player.Items.IndexOf(item) == -1)
        {
            return -1;
        }
        return SellBackItem(Game1.player.Items.IndexOf(item));
    }
}