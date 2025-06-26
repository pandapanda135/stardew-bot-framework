﻿using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewBotFramework.Source;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Inventories;
using StardewValley.Objects;

namespace BotTesting;

internal sealed class ModEntry : Mod
{
    private StardewClient _bot = null!;

    private NPC? Npc;

    private Dialogue _dialogue;
    
    public override void Entry(IModHelper helper)
    {
        try
        {
            _bot = new StardewClient(helper, Monitor, helper.Multiplayer);
        }
        catch (Exception e)
        {
            Monitor.Log($"Issue with setting up Bot \n {e}",LogLevel.Debug);
        }
        
        // Monitor.Log($"Start setting events",LogLevel.Debug);
        helper.Events.Input.ButtonPressed += ButtonPressed;
        helper.ConsoleCommands.Add("chat", $"", ChatCommand);
        helper.ConsoleCommands.Add("colour", "White, red, blue, green, jade, yellowgreen, pink, purple, yellow, orange, brown, gray, cream, salmon, peach, aqua, jungle, plum", ColourCommand);
        helper.ConsoleCommands.Add("emote", $"", EmoteCommand);
        helper.ConsoleCommands.Add("craft", $"", CraftCommand);
        helper.ConsoleCommands.Add("building", $"", BuildingCommand);
        helper.ConsoleCommands.Add("Response", "", ResponseCommand);
        helper.ConsoleCommands.Add("buy", "", BuyCommand);
    }

    private readonly List<string> _desObjects = new List<string>() { "rock","twig","Rock","Twig","Weeds","weeds","Stone" };
    
    private Chest? _currentchest = null;

    private async void ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (e.Button == SButton.H)
        {
            Monitor.Log($"Test",LogLevel.Debug);
            Logger.Log($"{_bot.Time.GetTimeString()}");
        }
        else if (e.Button == SButton.J)
        {
            _bot.Chat.SendPublicMessage("happy");
            _bot.Chat.UseEmote("heart");
        }
        else if (e.Button == SButton.K)
        {
            Goal end = new Goal.GoalPosition((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);
            _bot.Pathfinding.DestructibleObjects = _desObjects;
            await _bot.Pathfinding.Goto(end,false, true);
            _bot.Chat.SendPublicMessage("This should send after the bot has path-found :)");
        }
        else if (e.Button == SButton.U)
        {
            _bot.Inventory.PossibleCrafts();
        }
        else if (e.Button == SButton.I)
        {
            Logger.Info($"\n IMPORTANT:cursor tile: {Game1.currentCursorTile.X}, {Game1.currentCursorTile.Y}");

            foreach (Building locationBuilding in Game1.currentLocation.buildings)
            {
                // can find where door for building are using this
                Logger.Info(locationBuilding.textureName());
                Logger.Info($"{locationBuilding.humanDoor.X}, {locationBuilding.humanDoor.Y}");
                Logger.Info(
                    $"{locationBuilding.tileX}, {locationBuilding.tileY} \n {locationBuilding.tilesWide}, {locationBuilding.tilesHigh}");
                Logger.Info(
                    $"door x: {locationBuilding.tileX.Value + locationBuilding.humanDoor.X}  door Y: {locationBuilding.tileY.Value + locationBuilding.humanDoor.Y}"); // this gets door position
                // Building.doAction()
            }
        }
        else if (e.Button == SButton.Y)
        {
            _bot.Player.UseTool();
        }
        else if (e.Button == SButton.O)
        {
                foreach (var locationObjectDict in Game1.currentLocation.objects)
                {
                    foreach (var kvp in locationObjectDict)
                    {
                        Logger.Info($"tile: {kvp.Key} object: {kvp.Value.name}");
                        if (kvp.Key == Game1.currentCursorTile)
                        {
                            _bot.Player.AddItemToObject(kvp.Value, Game1.player.CurrentItem);
                        }
                    }
                }
                
        }
        else if (e.Button == SButton.P)
        {
            Logger.Info($"current cursor tile:  {Game1.currentCursorTile}");
        }
        else if (e.Button == SButton.V)
        {
            
            foreach (var locationObjectDict in Game1.currentLocation.objects)
            {
                foreach (var kvp in locationObjectDict)
                {
                    if (kvp.Value is Chest chest)
                    {
                        Logger.Info(chest.name);
                        IInventory inventory = _bot.Chest.OpenChest(chest)!;
                        foreach (var item in inventory)
                        {
                            Logger.Info($"inventory item: {item.Name}");
                        }

                        return;
                    }
                }
            }
        }
        else if (e.Button == SButton.X)
        {
            _bot.Chest.CloseChest();
        }
        else if (e.Button == SButton.G)
        {
            Game1.player.Position = Game1.currentCursorTile * Game1.tileSize;
        }
        else if (e.Button == SButton.R)
        {
            _bot.Dialogue.AdvanceDialogBox();
        }
        else if (e.Button == SButton.C)
        {
            _bot.Shop.BuyItem(3,1);
        }
        else if (e.Button == SButton.T)
        {
            Utility.TryOpenShopMenu("SeedShop", null, true);
        }
        else if (e.Button == SButton.Z)
        {
            _bot.Shop.SellBackItem(11);
        }
        else if (e.Button == SButton.Q)
        {
            Inventory inventory = _bot.Inventory.GetInventory();
            foreach (var item in inventory)
            {
                if (item is null) continue;
                Logger.Info($"item name: {item.Name}  item stack: {item.Stack}  item index: {inventory.IndexOf(item)}");
                if (item.Name == "Blueberry Seeds")
                {
                    Logger.Info(_bot.Shop.SellBackItem(item).ToString());
                }
            }
        }
    }
    
    private void BuyCommand(string arg, string[] args)
    {
        int intargs = Convert.ToInt32(args[0]);
        int quantity = Convert.ToInt32(args[1]);
        
        _bot.Shop.BuyItem(intargs,quantity);
    }
    
    private void ResponseCommand(string arg, string[] args)
    {
        List<NPCDialogueResponse>? responses = _bot.Dialogue.PossibleNpcDialogueResponses(_dialogue);

        int intargs = Convert.ToInt32(args[0]);
            
        _bot.Dialogue.ChooseResponse(_dialogue,responses[intargs]);
    }

    private void ChatCommand(string arg, string[] args)
    {
        string message = "";
        foreach (var word in args)
        {
            message += word + " ";
        }
        
        _bot.Chat.SendPublicMessage(message);
    }
    
    private void ColourCommand(string arg, string[] args)
    {
        if (!_bot.Chat.ChangeColour(args[0]))
        {
            Logger.Error($"\"{args[0]}\" is not an available colour");
        }
    }
    
    private void EmoteCommand(string arg, string[] args)
    {
        if (!_bot.Chat.UseEmote(args[0]))
        {
            
            Logger.Error($"\"{args[0]}\" is not an available emote");
        }
    }

    // this works doesn't update player's sprite though 
    private void CraftCommand(string arg, string[] args)
    {
        args[0] = args[0].ToLower();
        
        List<CraftingRecipe> craftingRecipes = _bot.Inventory.PossibleCrafts();
        
        foreach (var recipe in craftingRecipes)
        {
            if (recipe.name.ToLower() == args[0])
            {
                Logger.Info($"making {args[0]}");
                _bot.Inventory.CraftItem(recipe);
            }
        }
    }

    private void BuildingCommand(string arg, string[] args)
    {
        int x = int.Parse(args[0]);
        int y = int.Parse(args[1]);
        Vector2 tileVector = new Vector2(x, y);

        foreach (var building in Game1.currentLocation.buildings)
        {
            Vector2 buildingDoorVector = new Vector2(building.tileX.Value + building.humanDoor.X,
                building.tileY.Value + building.humanDoor.Y);
            if (buildingDoorVector == tileVector)
            {
                _bot.Building.DoBuildingAction(building, tileVector);
                if (building.textureName() == "Buildings\\Coop")
                {
                    _bot.Building.UseAnimalDoor(building);
                }
            }
        }
    }
}
