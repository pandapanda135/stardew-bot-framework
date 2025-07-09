using HarmonyLib;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events.GamePlayEvents;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.MainMenu;
using StardewBotFramework.Source.Modules.Pathfinding;
using StardewModdingAPI;
using StardewValley;

namespace StardewBotFramework.Source;

public class StardewClient : BotBase
{
    private static StardewClient? Instance { get; set; }

    private readonly IModHelper _helper;
    private readonly IManifest _manifest;
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
    public GameInformation GameInformation { get; }
    public EndDaySkillMenu EndDaySkillMenu { get; }
    public EndDayShippingMenu EndDayShippingMenu { get; }
    public CreateFarmBuilding FarmBuilding { get; }
    

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
        EndDaySkillMenu = new EndDaySkillMenu();
        EndDayShippingMenu = new EndDayShippingMenu();
        FarmBuilding = new CreateFarmBuilding();

        #endregion
        
        harmony.Patch(original: AccessTools.Method(typeof(Farmer), nameof(Farmer.LoseItemsOnDeath)),
            postfix: new HarmonyMethod(typeof(GameEvents.DeathPatch), nameof(GameEvents.DeathPatch.LoseItemsOnDeath_Postfix))
            );
    }
    
}