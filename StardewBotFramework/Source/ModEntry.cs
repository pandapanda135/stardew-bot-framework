using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using PathNode = StardewBotFramework.Source.Modules.Pathfinding.Base.PathNode;

namespace StardewBotFramework.Source;

public class Main : Mod
{
    public static Stack<PathNode> PathNodes = new();
    
    public override void Entry(IModHelper helper)
    {
        Logger.SetMonitor(Monitor);

        helper.Events.Input.ButtonPressed += ButtonPressed;
        helper.Events.Display.Rendered += DebugDraw.OnRenderTiles;
    }

    public void ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }
    }
}
