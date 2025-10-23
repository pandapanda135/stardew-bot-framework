using StardewModdingAPI;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// Modify the input of the keyboard and mouse
/// </summary>
public class Input
{
	public void SetButton(SButton button, bool down)
	{
		BotBase.Instance?.Helper.Input.OverrideButton(button,down);
	}

	public void ChangeMousePosition(int x, int y)
	{
		BotBase.Instance?.Helper.Input.SetCursorPosition(x,y);
	}
}