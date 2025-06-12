using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewBotFramework.Source;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Inventories;
using Object = StardewValley.Object;

namespace BotTesting;

internal sealed class ModEntry : Mod
{
    private StardewClient? _bot;
    
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
    }

    private void ButtonPressed(object? sender, ButtonPressedEventArgs e)
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
            // _bot.Chat.UseEmote("heart");
        }
        // else if (e.Button == SButton.K)
        // {
        //     _bot.Chat.ChangeColour("red");
        // }
    }
}
