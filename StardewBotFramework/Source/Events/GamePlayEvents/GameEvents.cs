using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Events.EventArgs;
using StardewBotFramework.Source.Events.World_Events;
using StardewBotFramework.Source.Modules;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;

namespace StardewBotFramework.Source.Events.GamePlayEvents;

public class GameEvents
{
    public GameEvents(IModHelper _helper)
    {
        _helper.Events.GameLoop.GameLaunched += OnGameLaunch;
        _helper.Events.GameLoop.DayStarted += OnDayStarted;
        _helper.Events.GameLoop.DayEnding += OnDayEnding;
        _helper.Events.GameLoop.TimeChanged += OnTimeChanged;
        _helper.Events.Display.MenuChanged += DisplayOnMenuChanged;
        
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
        _helper.Events.GameLoop.UpdateTicking += FishingBar.Update;
        
        FishingBar.StaticCaughtFish += OnStaticCaughtFish;
        StaticChatMessageReceived += OnStaticChatMessageReceived;
        StaticOnBotDeath += OnStaticOnBotDeath;
        StaticOnBotDamaged += StaticBotDamaged;
        StaticOnOtherPlayerDeath += OnStaticOnOtherPlayerDeath;
        StaticHudMessageAdded += OnStaticHUDMessageAdded;
        StaticEventFinished += OnStaticEventFinished;
    }

    private static IModHelper? _helper;

    public static void SetHelper(IModHelper helper)
    {
        _helper = helper;
    }

    #region Events
    /// <summary>
    /// On new Day Started.
    /// </summary>
    public event EventHandler<BotDayStartedEventArgs>? DayStarted;
    /// <summary>
    /// On day ended
    /// </summary>
    public event EventHandler<BotDayEndedEventArgs>? DayEnded; 
    /// <summary>
    /// When a player connects to multiplayer world.
    /// </summary>
    public event EventHandler<BotPlayerConnectedEventArgs>? PlayerConnected;
    /// <summary>
    /// When a player disconnects to multiplayer world.
    /// </summary>
    public event EventHandler<BotPlayerDisconnectedEventArgs>? PlayerDisconnected;
    /// <summary>
    /// When the bot moves location.
    /// </summary>
    public event EventHandler<BotWarpedEventArgs>? BotWarped;
    /// <summary>
    /// When the bot's inventory changes 
    /// </summary>
    public event EventHandler<BotInventoryChangedEventArgs>? BotInventoryChanged;
    /// <summary>
    /// When a skill the bot has changes, this will be called when it happens in the game and not be queued for when the day ends
    /// </summary>
    public event EventHandler<BotSkillLevelChangedEventArgs>? BotSkillChanged;
    /// <summary>
    /// When an object in the bot's current location changes.
    /// </summary>
    public event EventHandler<BotObjectListChangedEventArgs>? BotObjectChanged;
    /// <summary>
    /// When a NPC leaves or enters the bot's current locaiton.
    /// </summary>
    public event EventHandler<BotCharacterListChangedEventArgs>? BotLocationNpcChanged;
    /// <summary>
    /// When debris in the bot's current location changes.
    /// </summary>
    public event EventHandler<BotDebrisChangedEventArgs>? BotLocationDebrisChanged;
    /// <summary>
    /// When a building in the bot's current location changes.
    /// </summary>
    public event EventHandler<BotBuildingChangedEventArgs>? BotLocationBuildingsChanged;
    /// <summary>
    /// When furniture in the bot's current location changes.
    /// </summary>
    public event EventHandler<BotFurnitureChangedEventArgs>? BotLocationFurnitureChanged;
    /// <summary>
    /// When a Terrain feature (e.g. a tree or floor) in the bot's current location changes.
    /// </summary>
    public event EventHandler<BotTerrainFeatureChangedEventArgs>? BotTerrainFeatureChanged;
    /// <summary>
    /// When a Large terrain feature (e.g. bushes) in the bot's current location changes.
    /// </summary>
    public event EventHandler<BotLargeTerrainFeatureChangedEventArgs>? BotLargeTerrainFeatureChanged;
    /// <summary>
    /// If a fish or junk has been caught by the bot
    /// </summary>
    public event EventHandler? CaughtFish;
    /// <summary>
    /// When the time on the clock shown in-game changes, this is sent in the notation of.
    /// 24-hour notation (like 1600 for 4pm). The clock time resets when the player sleeps, so 2am (before sleeping) is 2600.
    /// </summary>
    public event EventHandler<TimeEventArgs>? UiTimeChanged;
    /// <summary>
    /// When the current modal that is displayed is changed, this can be used to detect when dialogue starts
    /// </summary>
    public event EventHandler<BotOnDeathEventArgs>? OnBotDeath;
    /// <summary>
    /// This is when the player takes damage using: <see cref="Farmer.takeDamage"/>
    /// </summary>
    public event EventHandler<BotDamagedEventArgs>? OnBotDamaged; 
    /// <summary>
    /// When player other than the bot dies.
    /// </summary>
    public event EventHandler<OnOtherPlayerDeathEventArgs>? OnOtherPlayerDeath; // not tested probably works though
    /// <summary>
    /// When there is a bot message
    /// </summary>
    public event EventHandler<ChatMessageReceivedEventArgs>? ChatMessageReceived;
    /// <summary>
    /// When a banner message pops up in the bottom right
    /// </summary>
    public event EventHandler<HUDMessageAddedEventArgs>? HUDMessageAdded;
    /// <summary>
    /// When an event ends according to <see cref="Game1.eventFinished"/>
    /// </summary>
    public event EventHandler<EventEndedEventArgs> EventFinished;
    /// <summary>
    /// When <see cref="Game1.activeClickableMenu"/> Changes
    /// </summary>
    public event EventHandler<BotMenuChangedEventArgs> MenuChanged;
    private static event EventHandler<BotOnDeathEventArgs>? StaticOnBotDeath;
    private static event EventHandler<BotDamagedEventArgs>? StaticOnBotDamaged; 
    private static event EventHandler<OnOtherPlayerDeathEventArgs>? StaticOnOtherPlayerDeath;
    private static event EventHandler<HUDMessageAddedEventArgs>? StaticHudMessageAdded;
    private static event EventHandler<ChatMessageReceivedEventArgs>? StaticChatMessageReceived;
    private static event EventHandler<EventEndedEventArgs>? StaticEventFinished; 
    #endregion

    #region Methods
    // StardewValley.Event
    private void DisplayOnMenuChanged(object? sender, MenuChangedEventArgs e) =>
        MenuChanged.Invoke(sender, new BotMenuChangedEventArgs(e.NewMenu!,e.OldMenu!)); 
    private void OnDayStarted(object? sender, DayStartedEventArgs e) => DayStarted?.Invoke(sender,new BotDayStartedEventArgs());
    
    private void OnDayEnding(object? sender, DayEndingEventArgs e) => DayEnded?.Invoke(sender, new BotDayEndedEventArgs());
    
    private void OnTimeChanged(object? sender, TimeChangedEventArgs e) => UiTimeChanged?.Invoke(sender, new TimeEventArgs(e.OldTime, e.NewTime));

    private void OnWarped(object? sender, WarpedEventArgs e) => BotWarped?.Invoke(sender, new BotWarpedEventArgs(e.Player, e.OldLocation, e.NewLocation, e.IsLocalPlayer));
    private void OnPeerConnected(object? sender, PeerConnectedEventArgs e) => PlayerConnected?.Invoke(sender,new BotPlayerConnectedEventArgs(e.Peer)); 
    
    private void OnPeerDisconnected(object? sender, PeerDisconnectedEventArgs e) => PlayerDisconnected?.Invoke(sender,new BotPlayerDisconnectedEventArgs(e.Peer));
    
    private void OnLargeTerrainFeatureListChanged(object? sender, LargeTerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLargeTerrainFeatureChanged?.Invoke(sender,new BotLargeTerrainFeatureChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnTerrainFeatureListChanged(object? sender, TerrainFeatureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotTerrainFeatureChanged?.Invoke(sender,new BotTerrainFeatureChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnBuildingListChanged(object? sender, BuildingListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationBuildingsChanged?.Invoke(sender,new BotBuildingChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnFurnitureListChanged(object? sender, FurnitureListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationFurnitureChanged?.Invoke(sender,new BotFurnitureChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnDebrisListChanged(object? sender, DebrisListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationDebrisChanged?.Invoke(sender,new BotDebrisChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnNpcListChanged(object? sender, NpcListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotLocationNpcChanged?.Invoke(sender,new BotCharacterListChangedEventArgs(e.Added,e.Removed,e.Location));
    }
    private void OnObjectListChanged(object? sender, ObjectListChangedEventArgs e)
    {
        if (!e.IsCurrentLocation) return;
        BotObjectChanged?.Invoke(sender,new BotObjectListChangedEventArgs(e.Added,e.Removed,e.Location));
    }

    private void OnInventoryChanged(object? sender, InventoryChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;
        BotInventoryChanged?.Invoke(sender,new BotInventoryChangedEventArgs(e.Added.ToArray(),e.Removed.ToArray(),e.QuantityChanged.ToArray()));
    }
    
    private void OnLevelChanged(object? sender, LevelChangedEventArgs e)
    {
        if (!e.IsLocalPlayer) return;
        BotSkillChanged?.Invoke(sender,new BotSkillLevelChangedEventArgs(e.Skill,e.OldLevel,e.NewLevel));
    }
    
    private void OnGameLaunch(object? sender, GameLaunchedEventArgs e) => Logger.Log($"Game launched setting up bot");

    private void OnStaticChatMessageReceived(object? sender, ChatMessageReceivedEventArgs e) =>
        ChatMessageReceived?.Invoke(sender, e);

    private void OnStaticOnOtherPlayerDeath(object? sender, OnOtherPlayerDeathEventArgs e) =>
        OnOtherPlayerDeath?.Invoke(sender, e);
    private void OnStaticHUDMessageAdded(object? sender, HUDMessageAddedEventArgs e)
    {
        // Logger.Info($"sender: {sender}");
        // Logger.Info($"e: {e}");
        HUDMessageAdded?.Invoke(sender, e);
    }

    private void OnStaticOnBotDeath(object? sender, BotOnDeathEventArgs e) => OnBotDeath?.Invoke(sender, e);
    private void StaticBotDamaged(object? sender, BotDamagedEventArgs e) => OnBotDamaged?.Invoke(sender, e);
    private void OnStaticCaughtFish(object? sender, System.EventArgs e) => CaughtFish?.Invoke(sender, e);
    private void OnStaticEventFinished(object? sender, EventEndedEventArgs e) => EventFinished.Invoke(sender, e);
        
    #endregion

    public class DeathPatch
    {
        public static void LoseItemsOnDeath_Postfix(Farmer __instance,ref int __result)
        {
            try
            {
                if (__instance == BotBase.Farmer)
                {
                    StaticOnBotDeath?.Invoke(new DeathPatch(), new BotOnDeathEventArgs(BotBase.CurrentLocation,BotBase.Farmer.TilePoint,__result));  
                }
                else
                {
                    StaticOnOtherPlayerDeath?.Invoke(new DeathPatch(), new OnOtherPlayerDeathEventArgs(__instance));
                }
            }
            catch (Exception e)
            {
                Logger.Log($"Failed in DeathPatch \n {e}", LogLevel.Error);
                throw;
            }
        }

        private static Point _previousDeathTile;
        public static void MineDeath_Postfix(ref Event @event, ref string[] args, EventContext context)
        {
            try
            {
                if (context.Event.farmer != BotBase.Farmer) return;
                if (_previousDeathTile == BotBase.Farmer.TilePoint) return; // stops from spamming context as cutscene likes to spam
                _previousDeathTile = BotBase.Farmer.TilePoint;
                StaticOnBotDeath?.Invoke(new DeathPatch(),new BotOnDeathEventArgs(BotBase.CurrentLocation,BotBase.Farmer.TilePoint, -1));
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }

        public static void PassOutNewDay_PostFix()
        {
            try
            {
                StaticOnBotDeath?.Invoke(new DeathPatch(), new BotOnDeathEventArgs(BotBase.CurrentLocation, BotBase.Farmer.TilePoint, -1));
            }
            catch (Exception e)
            {
                Logger.Error(e.ToString());
                throw;
            }
        }
    }

    public class MessagePatch
    {
        public static void receiveChatMessage_Postfix()
        {
            try
            {
                ChatMessage message = Game1.chatBox.messages[^1];
                foreach (var snippet in message.message)
                {
                    string[] chat = snippet.message.Split(":");
                    int index = snippet.message.IndexOf(":", StringComparison.Ordinal);
                    string removedMessage = snippet.message.Remove(0, index + 2); // we add 2 to remove padding after the colon
                    if (chat[0] == BotBase.Farmer.Name)
                    {
                        Logger.Info($"chat: {chat[0]}  index: {index}  removedMessage: {removedMessage}");
                        StaticChatMessageReceived?.Invoke(new MessagePatch(), new ChatMessageReceivedEventArgs(chat[0],removedMessage,0,false));
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error($"Failed in ReceiveChatMessage \n {e} \n This is mostly likely because it is not subscribed to anything");
            }
        }
    }

    public class HudMessagePatch
    {
        public static void addHUDMessage_postfix(Game1 __instance,HUDMessage message)
        {
            try
            {
                if (message.transparency < 1) return;
                // Logger.Info($"game: {__instance}");
                // Logger.Info($"message: {message}");
                StaticHudMessageAdded?.Invoke(__instance,
                    new HUDMessageAddedEventArgs(message.message, message.type, message.whatType,message.number, message.achievement,
                        message.noIcon, message.messageSubject));
            }
            catch (Exception e)
            {
                Logger.Error($"Failed in addHUDMessage \n {e} \n This is mostly likely because it is not subscribed to anything");
            }
        }
    }

    public class PlayerDamagedPatch
    {
        public static void takeDamage_prefix(Farmer __instance,ref int damage,ref bool overrideParry,ref Monster damager)
        {
            try
            {
                if (__instance != BotBase.Farmer || !__instance.CanBeDamaged()) return;
                // totally not stolen from the game :)
                if (Game1.eventUp || __instance.FarmerSprite.isPassingOut() || (__instance.isInBed.Value && Game1.activeClickableMenu != null && Game1.activeClickableMenu is ReadyCheckDialog))
                {
                    return;
                }
                StaticOnBotDamaged?.Invoke(new PlayerDamagedPatch(),new BotDamagedEventArgs(damage,overrideParry,damager));
            }
            catch (Exception e)
            {
                Logger.Error($"Failed in takeDamage \n {e} \n This is mostly likely because it is not subscribed to anything");
            }
        }
    }

    public class EventFinishedPatch
    {
        public static void eventFinished_prefix()
        {
            try
            {
                Logger.Info($"game 1 event: {Game1.CurrentEvent}   location event: {BotBase.CurrentLocation.currentEvent}");
                StaticEventFinished?.Invoke(new EventFinishedPatch(), new(BotBase.CurrentLocation.currentEvent));
            }
            catch (Exception e)
            {
                Logger.Error($"Failed in eventFinished event \n {e} \n This is mostly likely because it is not subscribed to anything");
            }
        }
    }
}