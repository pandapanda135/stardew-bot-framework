using System.Runtime.CompilerServices;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.MainMenu;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Network;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source;

public class StardewClient
{
    private static StardewClient? Instance { get; set; }

    private readonly IModHelper _helper;
    private readonly IManifest _manifest;
    private readonly IMonitor _monitor;
    private readonly IMultiplayerHelper _multiplayer;
    
    #region Events

    /// <summary>
    /// On new Day Started.
    /// </summary>
    public event EventHandler<BotDayStartedEventArgs> DayStarted;
    /// <summary>
    /// On day ended
    /// </summary>
    public event EventHandler<BotDayEndedEventArgs> DayEnded; 
    /// <summary>
    /// When a player connects to multiplayer world.
    /// </summary>
    public event EventHandler<BotPlayerConnectedEventArgs> PlayerConnected;
    /// <summary>
    /// When a player disconnects to multiplayer world.
    /// </summary>
    public event EventHandler<BotPlayerDisconnectedEventArgs> PlayerDisconnected;
    /// <summary>
    /// When the bot moves location.
    /// </summary>
    public event EventHandler<BotWarpedEventArgs> BotWarped;
    /// <summary>
    /// When the bot's inventory changes 
    /// </summary>
    public event EventHandler<BotInventoryChangedEventArgs> BotInventoryChanged;

    public event EventHandler<BotSkillLevelChangedEventArgs> BotSkillChanged;
    public event EventHandler<BotObjectListChangedEventArgs> BotObjectChanged;
    public event EventHandler<BotCharacterListChangedEventArgs> BotLocationNpcChanged;
    public event EventHandler<BotDebrisChangedEventArgs> BotLocationDebrisChanged;
    public event EventHandler<BotBuildingChangedEventArgs> BotLocationBuildingsChanged; 
    public event EventHandler<BotFurnitureChangedEventArgs> BotLocationFurnitureChanged;
    public event EventHandler<BotTerrainFeatureChangedEventArgs> BotTerrainFeatureChanged;
    public event EventHandler<BotLargeTerrainFeatureChangedEventArgs> BotLargeTerrainFeatureChanged;
    /// <summary>
    /// When the time on the clock shown in-game changes, this is sent in the notation of.
    /// 24-hour notation (like 1600 for 4pm). The clock time resets when the player sleeps, so 2am (before sleeping) is 2600.
    /// </summary>
    public event EventHandler<TimeEventArgs> UiTimeChanged;
    
    public static event EventHandler<BotOnDeathEventArgs> OnBotDeath;
    public static event EventHandler<OnOtherPlayerDeathEventArgs> OnOtherPlayerDeath;
    #endregion
    
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
    public GameInformation GameInformation { get; }

    

    #endregion
    
    public StardewClient(IModHelper helper, IManifest manifest,IMonitor monitor, IMultiplayerHelper multiplayer)
    {
        Instance = this;
        
        _helper = helper;
        _manifest = manifest;
        _monitor = monitor;
        _multiplayer = multiplayer;
        
        Logger.SetMonitor(_monitor); // this is here because I prefer it :)

        Harmony harmony = new(manifest.UniqueID);

        #region repetitive

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
        GameInformation = new GameInformation();

        
        _helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        _helper.Events.GameLoop.DayEnding += OnDayEnding;
        _helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        _helper.Events.GameLoop.DayStarted += OnDayStarted;
        
        _helper.Events.Multiplayer.PeerConnected += OnPeerConnected;
        _helper.Events.Multiplayer.PeerDisconnected += OnPeerDisconnected;
        
        _helper.Events.Player.Warped += OnWarped;
        _helper.Events.Player.InventoryChanged += OnInventoryChanged;
        _helper.Events.Player.LevelChanged += OnLevelChanged;
        
        _helper.Events.World.ObjectListChanged += OnObjectListChanged;
        _helper.Events.World.NpcListChanged += OnNpcListChanged;
        _helper.Events.World.DebrisListChanged += OnDebrisListChanged;
        _helper.Events.World.FurnitureListChanged += OnFurnitureListChanged;
        _helper.Events.World.BuildingListChanged += OnBuildingListChanged;
        _helper.Events.World.TerrainFeatureListChanged += OnTerrainFeatureListChanged;
        _helper.Events.World.LargeTerrainFeatureListChanged += OnLargeTerrainFeatureListChanged;
        
        _helper.Events.GameLoop.UpdateTicking += CharacterController.Update;
        
        #endregion
        
        harmony.Patch(original: AccessTools.Method(typeof(Farmer), nameof(Farmer.LoseItemsOnDeath)),
            postfix: new HarmonyMethod(typeof(DeathPatch), nameof(DeathPatch.LoseItemsOnDeath_Postfix))
            );
    }
    
    #region EventMethods
    
    private void OnDayStarted(object? sender, DayStartedEventArgs e) => DayStarted.Invoke(this,new BotDayStartedEventArgs());
    
    private void OnDayEnding(object? sender, DayEndingEventArgs e) => DayEnded.Invoke(this, new BotDayEndedEventArgs());
    
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e) => UiTimeChanged.Invoke(this,new TimeEventArgs(e.OldTime,e.NewTime));

    private void OnWarped(object? sender, WarpedEventArgs e) => BotWarped.Invoke(this,new BotWarpedEventArgs(e.Player,e.OldLocation,e.NewLocation,e.IsLocalPlayer));
    
    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e) => PlayerConnected.Invoke(this,new BotPlayerConnectedEventArgs(e.Peer)); 
    
    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e) => PlayerDisconnected.Invoke(this,new BotPlayerDisconnectedEventArgs(e.Peer));
    
    private void OnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLargeTerrainFeatureChanged.Invoke(this,new BotLargeTerrainFeatureChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnTerrainFeatureListChanged(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotTerrainFeatureChanged.Invoke(this,new BotTerrainFeatureChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationBuildingsChanged.Invoke(this,new BotBuildingChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnFurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationFurnitureChanged.Invoke(this,new BotFurnitureChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnDebrisListChanged(object? sender, DebrisListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationDebrisChanged.Invoke(this,new BotDebrisChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnNpcListChanged(object? sender, NpcListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationNpcChanged.Invoke(this,new BotCharacterListChangedEventArgs(e.Added,e.Removed,e.Location));
    }
    private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotObjectChanged.Invoke(this,new BotObjectListChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;
        BotInventoryChanged.Invoke(this,new BotInventoryChangedEventArgs(e.Added.ToArray(),e.Removed.ToArray(),e.QuantityChanged.ToArray()));
    }
    
    private void OnLevelChanged(object? sender, LevelChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;
        BotSkillChanged.Invoke(this,new BotSkillLevelChangedEventArgs(e.Skill,e.OldLevel,e.NewLevel));
    }
    
    private void OnGameLaunch(object? sender, GameLaunchedEventArgs e) => Logger.Log($"Game launched setting up bot");
    
    #endregion
    
    // internal stuff

    private class DeathPatch
    {
        public static void LoseItemsOnDeath_Postfix(Farmer __instance,ref int __result)
        {
            try
            {
                if (__instance == Farmer)
                {
                    OnBotDeath.Invoke(new DeathPatch(), new BotOnDeathEventArgs(CurrentLocation,Farmer.TilePoint,__result));  
                }
                else
                {
                    OnOtherPlayerDeath.Invoke(new DeathPatch(), new OnOtherPlayerDeathEventArgs()); 
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Failed in DeathPatch \n {e}", LogLevel.Error);
                throw;
            }
        } 
    }
    
    
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