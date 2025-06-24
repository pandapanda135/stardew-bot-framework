using Microsoft.Xna.Framework;
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
        else if (e.Button == SButton.N)
        {
            // bool value = _bot.Dialogue.CheckForCharacterAtTile(Game1.currentCursorTile.ToPoint());
            // Logger.Info($"value: {value}");
            Dictionary<string,List<string>>? dialogue = _bot.Dialogue.GetAllPossibleDialogue(Game1.currentCursorTile.ToPoint());
            if (dialogue is null) return;
            foreach (var kvp in dialogue)
            {
                foreach (var listString in kvp.Value)
                {
                    Logger.Info($"character name: {kvp.Key} dialogue: {listString}");    
                }
            }
            
            Dictionary<string,Point>? characters = _bot.Characters.GetCharactersInCurrentLocation(Game1.currentLocation);
            foreach (var kvp in characters)
            {
                Logger.Info($"character name: {kvp.Key} Point: {kvp.Value}");
            }

            _bot.Dialogue.InteractWithCharacter(Game1.currentCursorTile.ToPoint());
        }
        else if (e.Button == SButton.G)
        {
            Game1.player.Position = Game1.currentCursorTile * Game1.tileSize;
        }
        else if (e.Button == SButton.V)
        {
            IInventory? inventory = new Inventory();
            foreach (var locationObjectDict in Game1.currentLocation.objects)
            {
                foreach (var kvp in locationObjectDict)
                {
                    if (kvp.Value.name == "Chest" && kvp.Key == Game1.currentCursorTile)
                    {
                        _currentchest = (Chest)kvp.Value;
                        
                        inventory = _bot.Chest.OpenChest((Chest)kvp.Value);

                        if (inventory is null) return;
                        foreach (var item in inventory)
                        {
                            Logger.Info($"Name: {item.Name}  Amount: {item.Stack}");
                        }
                        
                        foreach (var item in inventory)
                        {
                            Logger.Info($"item: {item.Name}, Amount: {item.Stack}");
                            if (item.Name == "Wood")
                            {
                                if (!Game1.player.Items.Contains(item))
                                {
                                    Logger.Warning($"found item in inventory");
                                    _bot.Chest.TakeItemFromChest((Chest)kvp.Value,item,Game1.player);    
                                }
                            }
                        }
                    }
                }
            }

        }
        else if (e.Button == SButton.F)
        {
            if (_currentchest is null) return;
            
            foreach (var item in Game1.player.Items)
            {
                if (!Game1.player.Items.Contains(item)) continue;
                Logger.Info($"foreach iteration");
                Logger.Info($"Item to add: {item.Name}");
                if (item.Name != "Wood") continue;
                
                Logger.Warning($"adding item to chest");
                _bot.Chest.PutItemInChest(_currentchest,item,Game1.player);
            }
            
            _bot.Chest.CloseChest();
        }
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
