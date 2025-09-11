using HarmonyLib;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events.GamePlayEvents;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.MainMenu;
using StardewBotFramework.Source.Modules.Menus;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewBotFramework.Source.Modules.Pathfinding.GroupTiles;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source;

/// <summary>
/// This is the default bot that includes all modules.
/// </summary>
public class StardewClient : BotBase
{
    public static List<ITile> debugTiles = new();
    public static List<PathNode> debugNode = new();
    // private static BotBase? Instance { get; set; }

    // private readonly IModHelper _helper;
    // private readonly IManifest _manifest;
    // private readonly IMonitor _monitor;
    // private readonly IMultiplayerHelper _multiplayer;
    
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
    public EndDaySkillMenu EndDaySkillMenu { get; }
    public EndDayShippingMenu EndDayShippingMenu { get; }
    public CreateFarmBuilding FarmBuilding { get; }
    public ShippingBinInteraction ShippingBinInteraction { get; }
    public GroupedTiles GroupedTiles { get; }
    public ToolHandling Tool { get; }
    public QuestLogInteraction QuestLog { get;}
    public WorldState WorldState { get;}
    public GrabItemMenuInteraction ItemGrabMenu { get; }
    public BillBoardInteraction BillBoard { get; }
    public LetterViewer LetterViewer { get; }
    public FishingBar FishingBar { get; }
    public CommunityCenterMenu JunimoNote { get; }
    public CraftingMenu CraftingMenu { get; }
    public BuyAnimalMenu AnimalMenu { get; }
    public ItemListMenuInteraction ItemListMenu { get; }
    #endregion
    
    public StardewClient(IModHelper helper, IManifest manifest,IMonitor monitor, IMultiplayerHelper multiplayer)
    {
        Instance = this;
        
        _helper = helper;
        _manifest = manifest;
        _monitor = monitor;
        _multiplayer = multiplayer;

        GameEvents = new GameEvents(helper);
        
        Logger.SetMonitor(_monitor); // this is here because I prefer it :)

        Harmony = new(manifest.UniqueID);

        #region repetitive

        Inventory = new ();
        Time = new ();
        Player = new ();
        PlayerInformation = new ();
        Chat = new ();
        Pathfinding = new ();
        Building = new ();
        Dialogue = new ();
        Characters = new ();
        Chest = new ();
        Shop = new ();
        CharacterCreation = new ();
        MainMenuNavigation = new ();
        LoadMenu = new ();
        ObjectInteraction = new ();
        Blacksmith = new ();
        GameInformation = new ();
        EndDaySkillMenu = new ();
        EndDayShippingMenu = new ();
        FarmBuilding = new ();
        ShippingBinInteraction = new ();
        GroupedTiles = new ();
        Tool = new();
        QuestLog = new();
        WorldState = new();
        ItemGrabMenu = new();
        BillBoard = new();
        LetterViewer = new();
        FishingBar = new();
        JunimoNote = new();
        CraftingMenu = new();
        AnimalMenu = new();
        ItemListMenu = new();

        #endregion
        
        // probably don't need this anymore
        // harmony.Patch(original: AccessTools.Method(typeof(Farmer), nameof(Farmer.LoseItemsOnDeath)),
        //     postfix: new HarmonyMethod(typeof(GameEvents.DeathPatch), nameof(GameEvents.DeathPatch.LoseItemsOnDeath_Postfix)));
        
        Harmony.Patch(original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.MineDeath)),
            postfix: new HarmonyMethod(typeof(GameEvents.DeathPatch), nameof(GameEvents.DeathPatch.MineDeath_Postfix)));
        
        Harmony.Patch(original: AccessTools.Method(typeof(Game1), nameof(Game1.PassOutNewDay)),
            postfix: new HarmonyMethod(typeof(GameEvents.DeathPatch), nameof(GameEvents.DeathPatch.PassOutNewDay_PostFix)));

        Harmony.Patch(original: AccessTools.Method(typeof(ChatBox), nameof(ChatBox.receiveChatMessage)),
            postfix: new HarmonyMethod(typeof(GameEvents.MessagePatch), nameof(GameEvents.MessagePatch.receiveChatMessage_Postfix)));

        Harmony.Patch(original: AccessTools.Method(typeof(Game1), nameof(Game1.addHUDMessage)),
            postfix: new HarmonyMethod(typeof(GameEvents.HudMessagePatch), nameof(GameEvents.HudMessagePatch.addHUDMessage_postfix)));

        // this is a prefix so we can check if the bot can be damaged again as a postfix would always be false
        Harmony.Patch(original: AccessTools.Method(typeof(Farmer), nameof(Farmer.takeDamage)),
            prefix: new HarmonyMethod(typeof(GameEvents.PlayerDamagedPatch), nameof(GameEvents.PlayerDamagedPatch.takeDamage_prefix)));
        
        Harmony.Patch(original: AccessTools.Method(typeof(Game1), nameof(Game1.eventFinished)),
            prefix: new HarmonyMethod(typeof(GameEvents.EventFinishedPatch), nameof(GameEvents.EventFinishedPatch.eventFinished_prefix)));
    }
    
}