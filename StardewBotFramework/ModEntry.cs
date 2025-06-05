using StardewBotFramework.Debug;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace StardewBotFramework;

public class Main : Mod
{
    public override void Entry(IModHelper helper)
    {
        Logger.SetMonitor(Monitor);

        helper.Events.Input.ButtonPressed += ButtonPressed;

        // helper.ConsoleCommands.Add("bot", "used for testing bot framework", func);
    }

    public void ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (e.Button == SButton.H)
        {
            Logger.Info("User has Pressed button to create bot");

            // put creation of bot here (this will be for pathfinding
        }
    }

    // temp for testing
}
