using StardewBotFramework.Debug;
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

    #endregion

    public StardewClient(IModHelper helper, IMonitor monitor, IMultiplayerHelper multiplayer)
    {
        _helper = helper;
        _monitor = monitor;
        _multiplayer = multiplayer;

        Logger.SetMonitor(_monitor); // this is here because I prefer it :)
        
        Inventory = new InventoryManagement();

        _helper.Events.GameLoop.GameLaunched += OnGameLaunch;
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        Logger.Log($"Game launched setting up bot");
    }
    
    // this will be used to make it
    
    internal IModHelper Helper => _helper;
    internal IMonitor Monitor => _monitor;
    internal IMultiplayerHelper Multiplayer => _multiplayer;

    public Farmer Player => Game1.player;
    public GameLocation CurrentLocation => Game1.currentLocation;

}