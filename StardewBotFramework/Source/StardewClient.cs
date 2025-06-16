using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace StardewBotFramework.Source;

public class StardewClient
{
    private readonly IModHelper _helper;
    private readonly IMonitor _monitor;
    private readonly IMultiplayerHelper _multiplayer;

    #region Modules

    public InventoryManagement Inventory { get; }
    public Time Time { get; }
    public Player Player { get; }
    public Chat Chat { get; }
    public Pathfinder Pathfinding { get; }


    #endregion

    public StardewClient(IModHelper helper, IMonitor monitor, IMultiplayerHelper multiplayer)
    {
        _helper = helper;
        _monitor = monitor;
        _multiplayer = multiplayer;

        Logger.SetMonitor(_monitor); // this is here because I prefer it :)
        
        Inventory = new InventoryManagement();
        Time = new Time();
        Player = new Player();
        Chat = new Chat();
        Pathfinding = new Pathfinder();

        _helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        _helper.Events.GameLoop.UpdateTicking += CharacterController.Update;
        _helper.Events.GameLoop.UpdateTicking += AlgorithmBase.IPathing.Update;
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

}