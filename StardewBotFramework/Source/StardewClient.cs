using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

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
    public Chat Chat { get; }
    public Pathfinder Pathfinding { get; }

    public Buildings Building { get; }
    public DialogueManager Dialogue { get; }
    public Characters Characters { get; }
    public ChestModule Chest { get; }
    public Shop Shop { get; }


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
        Chat = new Chat();
        Pathfinding = new Pathfinder();
        Building = new Buildings();
        Dialogue = new DialogueManager();
        Characters = new Characters();
        Chest = new ChestModule();
        Shop = new Shop();
        
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
}