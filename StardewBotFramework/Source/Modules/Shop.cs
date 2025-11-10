using StardewBotFramework.Debug;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace StardewBotFramework.Source.Modules;

// Utility.TryOpenShopMenu("SeedShop", null, true); 
// This is a very jank solution however 

// chargePlayer(Game1.player, currency, -sell_unit_price * toSell.Stack);
    
// BuyBuybackItem(ISalable bought_item, int price, int stack)
    
// switchTab(int new_tab)
    
// tryToPurchaseItem(ISalable item, ISalable held_item, int stockToBuy, int x, int y)
    
// The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).

public class Shop : MenuHandler
{
    public ShopMenu Menu
    {
        get => _menu as ShopMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _menu = value;
    }

    /// <summary>
    /// The amount of tabs in this shop.
    /// </summary>
    public int? TabAmount => Menu.tabButtons.Count;

    public Dictionary<ISalable, ItemStockInformation> StockInformation => Menu.itemPriceAndStock;

    /// <summary>
    /// Open selected shop menu, this will need to be called before anything else. If you are calling OpenShopUI this is done for you.
    /// </summary>
    /// <param name="shopMenu">Shop to open</param>
    public void OpenShop(ShopMenu shopMenu) => Menu = shopMenu;

    /// <summary>
    /// Close selected shop menu.
    /// </summary>
    /// <returns>This will not close the current menu if it cannot be closed according to Menu.readyToClose</returns>
    public override bool RemoveMenu()
    {
        if (!Menu.readyToClose()) return false; 
        base.RemoveMenu();
        return true;
    }
    
    /// <summary>
    /// Interact with shopkeeper to open UI, this can be quite finicky so you may need to also check the tiles around where you think the shop tile is
    /// </summary>
    /// <param name="x">x position of shopkeeper</param>
    /// <param name="y">y position of shopkeeper</param>
    /// <returns>Will return false if there is no shop at the location else will return true</returns>
    public bool OpenShopUi(int x,int y)
    {
        BotBase.CurrentLocation.checkAction(new Location(x,y),Game1.viewport,BotBase.Farmer);
        if (Game1.activeClickableMenu is not ShopMenu)
        {
            Logger.Warning($"There is no shop at {x},{y}");
            return false;
        }
        OpenShop((Game1.activeClickableMenu as ShopMenu)!);
        return true;
    }
    
    /// <summary>
    /// Buy an item from the shop based on it's corresponding index in the shop's UI.
    /// </summary>
    /// <param name="index">The index of item to buy.</param>
    /// <param name="quantity">Amount of item to buy.</param>
    public async Task BuyItem(int index, int quantity)
    {
        await TaskDispatcher.SwitchToMainThread();
        if (index < 4)
        {
            for (int i = 0; i < quantity; i++)
            {
                LeftClick(Menu.forSaleButtons[index]);
            }
            await Task.Delay(500);
            await TaskDispatcher.SwitchToMainThread();

            for (int i = 0; i < Menu.inventory.actualInventory.Count; i++)
            {
                if (Menu.inventory.actualInventory[i] is null)
                {
                    LeftClick(Menu.inventory.inventory[i]);
                }
            }
            
            return;
        }
        
        // use down arrow
        for (int i = index; i >= 3; i--)
        {
            LeftClick(Menu.downArrow);
        }

        await Task.Delay(500);
        await TaskDispatcher.SwitchToMainThread();
        
        // TODO: this may have issues with buttons at the bottom, probably just need to find a new way to handle this.
        var bottomButton = Menu.forSaleButtons[^1];
        for (int i = 0; i < quantity; i++)
        {
            LeftClick(bottomButton);
        }
        await Task.Delay(500);
        await TaskDispatcher.SwitchToMainThread();

        for (int i = 0; i < Menu.inventory.actualInventory.Count; i++)
        {
            if (Menu.inventory.actualInventory[i] is not null) continue;
            
            LeftClick(Menu.inventory.inventory[i]);
        }

        // go back to top
        for (int i = index; i >= 3; i--)
        {
            Logger.Info($"using down arrow  i: {i}");
            LeftClick(Menu.upArrow);   
        }
    }

    /// <summary>
    /// Buy an <see cref="Item"/> from the shop.
    /// </summary>
    /// <param name="item">Item to buy.</param>
    /// <param name="quantity">Amount of item to buy.</param>
    public async Task BuyItem(Item item, int quantity)
    {
        for (int i = 0; i < Menu.forSale.Count; i++)
        {
            if (Menu.forSale[i].Name != item.Name) continue;
            
            await BuyItem(i, quantity);
            await TaskDispatcher.SwitchToMainThread();
        }
    }

    /// <summary>
    /// Returns all items as their <see cref="ISalable"/> class 
    /// </summary>
    /// <returns>A list of all items for sale however is a shop has not been opened yet, it will return null</returns>
    public List<ISalable> ListAllItems() => Menu.forSale;

    /// <summary>
    /// The stats of the items on sale, this also includes the currency this shop accepts and the available items
    /// </summary>
    /// <param name="items">The available items at this shop.</param>
    /// <param name="currency">the currency this shop accepts.</param>
    /// <returns>The stock info for each item in the shop as a dictionary. If a shop is not open everything will return the lowest possible value</returns>
    public Dictionary<ISalable,ItemStockInformation> ForSaleStats(out List<ISalable> items, out int currency)
    {
        currency = Menu.currency;
        items = Menu.forSale;
        return Menu.itemPriceAndStock;
    }
    
    /// <summary>
    /// Change tab of shop.
    /// </summary>
    /// <param name="newTab">index of new tab.</param>
    public void ChangeTab(int newTab)
    {
        if (Menu.tabButtons.Count == 0) return;
        
        Menu.switchTab(newTab);
    }
    
    /// <summary>
    /// Will try to see back item in the provided index of the bot's inventory
    /// </summary>
    /// <param name="index">this is a value of 0 to max provided by the inventory level these can only be 11-23-35 (assuming the game is not modded)</param>
    /// <returns>The amount the item has sold for. However, if this returns -1 it means there has been an issue with selling item</returns>
    public int SellBackItem(int index) // Item item
    {
        if (!Menu.CanBuyback())
        {
            Logger.Error($"shop cannot buy back");
            return -1;
        }

        List<ClickableComponent> inventory = Menu.inventory.inventory;

        int itemSalePrice = Menu.inventory.actualInventory[index].GetSalableInstance().sellToStorePrice();
        int itemStackSize = Menu.inventory.actualInventory[index].Stack;
        
        LeftClick(inventory[index]);
        
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