using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewBotFramework.Source;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;

namespace BotTesting;

internal sealed class ModEntry : Mod
{
    private StardewClient _bot;
    
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
    }

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
            await _bot.Pathfinding.Goto(end, false);
            _bot.Chat.SendPublicMessage("This should send after the bot has path-found :)");
        }
        else if (e.Button == SButton.U)
        {
            _bot.Inventory.PossibleCrafts();
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
}
