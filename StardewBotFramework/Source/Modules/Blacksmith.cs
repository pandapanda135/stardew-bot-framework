using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;
using xTile.Dimensions;

namespace StardewBotFramework.Source.Modules;

public class Blacksmith
{
// Utility.TryOpenShopMenu("SeedShop", null, true); 
// This is a very jank solution however 

// chargePlayer(Game1.player, currency, -sell_unit_price * toSell.Stack);

// BuyBuybackItem(ISalable bought_item, int price, int stack)

// switchTab(int new_tab)

// tryToPurchaseItem(ISalable item, ISalable held_item, int stockToBuy, int x, int y)

// The currency in which all items in the shop should be priced. The valid values are 0 (money), 1 (star tokens), 2 (Qi coins), and 4 (Qi gems).
    private ShopMenu? _currentShop;
    private GeodeMenu? _currentMenu;

    /// <summary>
    /// Open selected shop menu, this will need to be called before anything else
    /// </summary>
    /// <param name="menu">menu to open</param>
    private void OpenShop(ShopMenu menu)
    {
        _currentShop = menu;
    }

    /// <summary>
    /// Close selected shop menu.
    /// </summary>
    public void CloseShop()
    {
        _currentShop = null;
        _currentMenu = null;
    }

    public void OpenGeodeMenu(GeodeMenu menu)
    {
        _currentMenu = menu;
    }

    public void CloseGeodeMenu()
    {
        _currentShop = null;
        _currentMenu = null;
    }

    /// <summary>
    /// Interact with shopkeeper to open UI, this can be quite finicky so you may need to also check the tiles around where you think the shop tile is
    /// </summary>
    /// <param name="x">x position of shopkeeper</param>
    /// <param name="y">y position of shopkeeper</param>
    /// <param name="option">If this is 1 will open up upgrade menu else if this is 0 shop. If this is 2 it will either close the shop or open the geode menu depending on the current inventory</param>
    /// <returns>Will return false if there is no shop at the location else will return true</returns>
    public bool OpenShopUi(int x, int y, int option)
    {
        StardewClient.CurrentLocation.checkAction(new Location(x, y), Game1.viewport, StardewClient.Farmer);
        if (Game1.activeClickableMenu is not DialogueBox)
        {
            Logger.Warning($"There is no shop at {x},{y}");
            return false;
        }

        // OpenShop((Game1.activeClickableMenu as DialogueBox)!);
        Logger.Info($"active clickablebox type {Game1.activeClickableMenu.GetType()}");
        DialogueBox dialogueBox = (Game1.activeClickableMenu as DialogueBox);
        Logger.Info($"active clickablebox type {Game1.activeClickableMenu.GetType()}");
        foreach (var response in dialogueBox.responses)
        {
            Logger.Info($"Response: {response.responseText}");
        }
        
        Logger.Info($"Dialogue: {dialogueBox.characterDialogue}");

        // DialogueManager.AdvanceDialogue(0,0); // TODO: make this work so it will go through the dialogue box that shows up when you interact with npc
        // DialogueManager.ChooseResponse(option, Game1.activeClickableMenu as DialogueBox, dialogueBox.characterDialogue, dialogueBox.responses[option]);
        if (option == 0 || option == 1)
        {
            OpenShop((Game1.activeClickableMenu as ShopMenu)!);
        }
        else if (option == 2)
        {
            OpenGeodeMenu((Game1.activeClickableMenu as GeodeMenu)!);
        }

        return true;
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
            {
                _currentShop.receiveLeftClick(_currentShop.forSaleButtons[index].bounds.X + 10,
                    _currentShop.forSaleButtons[index].bounds.Y + 10, true);
            }

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
            _currentShop.receiveLeftClick(_currentShop.forSaleButtons[maxIndex].bounds.X,
                _currentShop.forSaleButtons[maxIndex].bounds.Y, true);
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
    public Dictionary<ISalable, ItemStockInformation> ForSaleStats(out List<ISalable> items, out int currency)
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

        _currentShop.receiveLeftClick(inventory[index].bounds.X + 3,
            inventory[index].bounds.Y + 3); //TODO: make work

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

    public Item? OpenGeode(int index)
    {
        if (_currentMenu is not GeodeMenu) return null;

        foreach (var inventoryItem in _currentMenu.inventory.actualInventory)
        {
            if (inventoryItem == null) continue;
            Logger.Info($"Inventory: {inventoryItem.Name}");
        }
        
        Item item = _currentMenu.inventory.actualInventory[index];

        if (!Utility.IsGeode(item))
        {
            Logger.Warning($"Item is not geode");
            return null;
        }

        if (StardewClient.Farmer._money < 25)
        {
            Logger.Warning($"Player does not have enough money");
            return null;
        }

        if (StardewClient.Farmer.freeSpotsInInventory() == 0)
        {
            Logger.Warning($"Player does not have enough inventory slots free");
            return null;
        }
        // move item to geode spot

        _currentMenu.heldItem = item.getOne();
        item.Stack -= 1;
        
        _currentMenu.receiveLeftClick(_currentMenu.geodeSpot.bounds.X, _currentMenu.geodeSpot.bounds.Y);
        
        return _currentMenu.geodeTreasure;
    }
}