using StardewBotFramework.Debug;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules.Menus;

public class Blacksmith : IShopMenu
{
    private GeodeMenu? _currentMenu;

    /// <summary>
    /// Close selected shop menu.
    /// </summary>
    public new void CloseShop()
    {
        CurrentShop = null;
        _currentMenu = null;
    }

    public void OpenGeodeMenu(GeodeMenu menu)
    {
        _currentMenu = menu;
    }

    public void CloseGeodeMenu()
    {
        CurrentShop = null;
        _currentMenu = null;
    }

    /// <summary>
    /// Interact with shopkeeper to open UI, this can be quite finicky so you may need to also check the tiles around where you think the shop tile is
    /// This will open the shop no matter where the player is so you may need to implement your own checks for this.
    /// </summary>
    /// <param name="x">x position of shopkeeper</param>
    /// <param name="y">y position of shopkeeper</param>
    /// <param name="option">If this is 1 will open up upgrade menu else if this is 0 shop. If this is 2 it will either close the shop or open the geode menu depending on the current inventory</param>
    /// <returns>Will return false if there is no shop at the location else will return true</returns>
    public bool OpenShopUi(int x, int y, int option)
    {
        // StardewClient.CurrentLocation.checkAction(new Location(x, y), Game1.viewport, StardewClient.Farmer);
        // if (Game1.activeClickableMenu is not DialogueBox)
        // {
        //     Logger.Warning($"There is no shop at {x},{y}");
        //     return false;
        // }

        // OpenShop((Game1.activeClickableMenu as DialogueBox)!);
        // Logger.Info($"active clickablebox type {Game1.activeClickableMenu.GetType()}");
        // DialogueBox dialogueBox = (Game1.activeClickableMenu as DialogueBox);
        // Logger.Info($"active clickablebox type {Game1.activeClickableMenu.GetType()}");
        // foreach (var response in dialogueBox.responses)
        // {
        //     Logger.Info($"Response: {response.responseText}");
        // }
        //
        // Logger.Info($"Is question: {dialogueBox}");
        //
        // Logger.Info($"Dialogue: {dialogueBox.characterDialogue}");

        // DialogueManager.AdvanceDialogue(0,0);
        // DialogueManager.ChooseResponse(option, Game1.activeClickableMenu as DialogueBox, dialogueBox.characterDialogue, dialogueBox.responses[option]);
        // should try to move this to using dialogue but I cant make it work that way right now.

        // if (StardewClient.CurrentLocation is not ShopLocation) return false;
        
        // make this work so it will go through the dialogue box that shows up when you interact with npc as current solution is quite jank
        switch (option)
        {
            case 0:
                Utility.TryOpenShopMenu("Blacksmith", "Clint");
                OpenShop((Game1.activeClickableMenu as ShopMenu)!);
                break;
            case 1:
                Utility.TryOpenShopMenu("ClintUpgrade", "Clint");
                OpenShop((Game1.activeClickableMenu as ShopMenu)!);
                break;
            case 2:
                Game1.activeClickableMenu = new GeodeMenu(); // jank but it works (This will open anywhere)
                OpenGeodeMenu((Game1.activeClickableMenu as GeodeMenu)!);
                break;
        }
        return true;
    }

    public Item? OpenGeode(int index)
    {
        if (_currentMenu is not GeodeMenu)
        {
            Logger.Warning($"Current menu is not GeodeMenu");
            return null;
        }

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

        if (BotBase.Farmer._money < 25)
        {
            Logger.Warning($"Player does not have enough money");
            return null;
        }

        if (BotBase.Farmer.freeSpotsInInventory() == 0 && _currentMenu.inventory.actualInventory[index].Stack > 1)
        {
            Logger.Warning($"Player does not have enough inventory slots free");
            return null;
        }
        // move item to geode spot

        _currentMenu.heldItem = item.getOne();
        if (item.Stack == 1)
        {
            BotBase.Farmer.Items[index] = null;
        }
        else
        {
            BotBase.Farmer.Items[index].Stack -= 1;
        }
        
        _currentMenu.receiveLeftClick(_currentMenu.geodeSpot.bounds.X, _currentMenu.geodeSpot.bounds.Y);

        return _currentMenu.geodeTreasure;
    }
}