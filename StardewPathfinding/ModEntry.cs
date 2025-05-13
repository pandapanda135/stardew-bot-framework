using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewPathfinding.Debug;
using StardewPathfinding.TileInterface;

namespace StardewPathfinding;

public class Main : Mod
{
    public override void Entry(IModHelper helper)
    {
        Logger.SetMonitor(Monitor);
        
        helper.Events.Input.ButtonPressed += ButtonPressed;
    }

    public void ButtonPressed(object? sender,ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (e.Button == SButton.J)
        {
            Logger.Info("User has Pressed button to start");
            
            // Start pathfinding stuff here
        }
    }
}