using System;
using System.Reflection;
using StardewBotFramework.Source.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.SDKs;

namespace StardewBotFramework.Source.Modules.MainMenu;

public class MultiplayerMenu : MenuHandler
{
    public CoopMenu Menu
    {
        get => _menu as CoopMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _menu = value;
    }

    public TitleTextInputMenu InputMenu
    {
        get => _childMenu as TitleTextInputMenu ?? throw new InvalidOperationException("Menu has not been initialized. Call either SetStoredMenu() or another method around setting UI first.");
        private set => _childMenu = value;
    }
    /// <summary>
    /// This will get if the game is finished connecting to the current SDK. If your bot is not loaded when running this, it will be null so be careful.
    /// </summary>
    public bool? ConnectingFinished => BotBase.Instance?.Helper.Reflection
        .GetProperty<SDKHelper>(typeof(Program), "sdk").GetValue().ConnectionFinished;

    public bool ReadyForAddress => TitleMenu.subMenu is TitleTextInputMenu input && input.context == "join_menu";
    
    public void SetTitleMenu(CoopMenu multiplayer)
    {
        Menu = multiplayer;
    }

    private readonly Type? _lanSlotType = typeof(CoopMenu).GetNestedType("LanSlot", BindingFlags.NonPublic);
    public void ClickJoinLan()
    {
        if (Menu.currentTab != CoopMenu.Tab.JOIN_TAB) LeftClick(Menu.joinTab);

        LoadGameMenu.MenuSlot? slot = null;
        foreach (var menuMenuSlot in Menu.MenuSlots)
        {
            if (_lanSlotType is null || !_lanSlotType.IsInstanceOfType(menuMenuSlot)) continue;
            
            slot = menuMenuSlot;
            break;
        }

        slot?.Activate();
    }
    
    public void SetAndJoinAddress(string address)
    {
        if (TitleMenu.subMenu is not TitleTextInputMenu input || input.context != "join_menu") return;
        InputMenu = input;
        InputMenu.textBox.Text = address;
        
        MenuLeftClick(InputMenu, input.doneNamingButton);
    }
    
    // TODO: implement this someday.
    /// <summary>
    /// This has not been Implemented yet
    /// </summary>
    public void HostGame()
    {
        if (Menu.currentTab != CoopMenu.Tab.HOST_TAB) LeftClick(Menu.hostTab);
    }
}