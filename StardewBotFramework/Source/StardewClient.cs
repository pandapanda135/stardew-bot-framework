using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.MainMenu;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;

namespace StardewBotFramework.Source;

public class StardewClient
{
    private static StardewClient? Instance { get; set; }

    private readonly IModHelper _helper;
    private readonly IMonitor _monitor;
    private readonly IMultiplayerHelper _multiplayer;

    #region Modules

    public InventoryManagement Inventory { get; }
    public Time Time { get; }
    public Player Player { get; }
    public PlayerInformation PlayerInformation { get; }
    public Chat Chat { get; }
    public Pathfinder Pathfinding { get; }
    public Buildings Building { get; }
    public DialogueManager Dialogue { get; }
    public Characters Characters { get; }
    public ChestModule Chest { get; }
    public Shop Shop { get; }
    public ObjectInteraction ObjectInteraction { get; }
    public CharacterCreation CharacterCreation { get; }
    public MainMenuNavigation MainMenuNavigation { get; }
    public LoadGame LoadMenu { get; }
    public Blacksmith Blacksmith { get; }

    

    #endregion
    
    public StardewClient(IModHelper helper, IMonitor monitor, IMultiplayerHelper multiplayer)
    {
        Instance = this;
        
        _helper = helper;
        _monitor = monitor;
        _multiplayer = multiplayer;

        Logger.SetMonitor(_monitor); // this is here because I prefer it :)
        
        Inventory = new InventoryManagement();
        Time = new Time();
        Player = new Player();
        PlayerInformation = new PlayerInformation();
        Chat = new Chat();
        Pathfinding = new Pathfinder();
        Building = new Buildings();
        Dialogue = new DialogueManager();
        Characters = new Characters();
        Chest = new ChestModule();
        Shop = new Shop();
        CharacterCreation = new CharacterCreation();
        MainMenuNavigation = new MainMenuNavigation();
        LoadMenu = new LoadGame();
        ObjectInteraction = new ObjectInteraction();
        Blacksmith = new Blacksmith();

        
        _helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        _helper.Events.GameLoop.UpdateTicking += CharacterController.Update;
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        Logger.Log($"Game launched setting up bot");
    }
    
    // internal stuff
    
    internal IModHelper Helper => _helper;
    internal IMonitor Monitor => _monitor;
    internal IMultiplayerHelper Multiplayer => _multiplayer;

    public static Farmer Farmer => Game1.player;
    public static GameLocation CurrentLocation => Game1.currentLocation;

    private static IReflectedField<int>? _selectedResponse;

    private static IReflectedField<TextBox>? _reflectedTextBox;

    private static IReflectedField<NetMutexQueue<Guid>> _reflectedObjectDestroy;
    
    /// <summary>
    /// Change the selectedResponse value in <see cref="DialogueBox"/>, this will change the "Response" to a question dialogue.
    /// Must use Game1.ReceiveLeftClick after calling this for it to register.
    /// </summary>
    /// <param name="value">This should range from 0-3 as there is a max of four responses. Not 100% sure on that though so there is no check. If this is -1 no option will be picked</param>
    internal static void ChangeSelectedResponse(int value)
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return;
        }
        
        _selectedResponse = Instance.Helper.Reflection.GetField<int>(Game1.activeClickableMenu, "selectedResponse", true);
        
        _selectedResponse.SetValue(value);   
    }

    internal static void CharacterCreatorTextBox(CharacterCustomization menu,string name, List<string> properties)
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return;
        }
        
        _reflectedTextBox = Instance.Helper.Reflection.GetField<TextBox>(menu, properties[0], true);
        
        // IReflectedField<string> reflectedText = Instance.Helper.Reflection.GetField<string>(_reflectedTextBox, properties[1], true);

        TextBox textBox = _reflectedTextBox.GetValue();

        textBox.Text = name;
        
        _reflectedTextBox.SetValue(textBox);
        
        // reflectedText.SetValue(name);   
    }
    
    internal static void AddRemoveFurniture(GameLocation location,Guid f)
    {
        if (Instance is null)
        {
            Logger.Error($"Instance is not set");
            return;
        }
        
        _reflectedObjectDestroy = Instance.Helper.Reflection.GetField<NetMutexQueue<Guid>>(location,"furnitureToRemove",true);

        NetMutexQueue<Guid> value = _reflectedObjectDestroy.GetValue();
        
        if (!value.Contains(f))
        {
            value.Add(f);
        }
        
        _reflectedObjectDestroy.SetValue(value);
    }
}