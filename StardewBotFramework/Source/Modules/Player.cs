using Microsoft.Xna.Framework;
using Netcode;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Objects.Trinkets;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// Methods related to the player character, this ranges from eating food to look in a different direction.
/// </summary>
public class Player
{
    /// <summary>
    /// Goes 0-3 from North,East,South,West
    /// </summary>
    public int FacingDirection => BotBase.Farmer.FacingDirection;

    /// <summary>
    /// Changes direction the player sprite is facing
    /// </summary>
    /// <param name="direction">Goes 0-3 from North,East,South,West</param>
    public void ChangeFacingDirection(int direction)
    {
        BotBase.Farmer.FacingDirection = direction;
    }

    /// <summary>
    /// Bot will eat Object that is provided
    /// </summary>
    /// <para name="item">If you are using item you can use <see cref="Item"/> as Object</para>
    /// <returns>Will return true if food can be eaten else false</returns>
    public bool ConsumeFood(Object item)
    {
        if (item.Edibility == -300) return false;

        if (!Game1.player.Items.Contains(item)) return false;
        
        Game1.player.eatObject(item);
        Game1.player.Items.Reduce(item, 1);
        return true;
    }

    /// <summary>
    /// Try to add item to an object (e.g. input for machine, placed on a table)
    /// </summary>
    /// <param name="addObject">Object to add to</param>
    /// <param name="item">Item to add</param>
    /// <returns>Usually returns whether the item was accepted by the object.</returns>
    public bool AddItemToObject(Object addObject,Item item)
    {
        return addObject.performObjectDropInAction(item, false, StardewClient.Farmer);
    }
    
    // could use CheckForActionOn{item} in Object.cs for alot of the items
    // or could just use CheckForAction
    
    public Vector2 BotPixelPosition()
    {
        return Game1.player.Position;
    }

    public Point BotTilePosition()
    {
        return Game1.player.TilePoint;
    }

    #region PickUpDebris

    // this is the debris approximate position
    private static Vector2 DebrisPosition(NetObjectShrinkList<Chunk> chunks)
    {
        Vector2 total = new();
        
        foreach (var chunk in chunks)
        {
            total += chunk.position.Value;
        }
        return total / chunks.Count;
    }
    public async Task PickUpDebrisInRadius(Point startPoint,int radius,bool canDestroy = false)
    {
        GameLocation location = BotBase.CurrentLocation;

        List<Point> radiusPoints = new();
        Vector2 endPoint = new(startPoint.X + radius, startPoint.Y + radius);
        startPoint.X -= radius;
        startPoint.Y -= radius;
        for (int x = startPoint.X; x < endPoint.X; x++)
        {
            for (int y = startPoint.Y; y < endPoint.Y + radius; y++)
            {
                radiusPoints.Add(new Point(x,y));
            }
        }

        List<Point> points = new();
        foreach (var debris in location.debris)
        {
            Logger.Info($"debris: {debris}  debris message: {DebrisPosition(debris.Chunks) / Game1.tileSize}");
            if (debris.item is not null)
            {
                Logger.Info($"debris: {debris.item.Name}");
            }
            Vector2 debrisLocation = DebrisPosition(debris.Chunks) / Game1.tileSize;
            if (radiusPoints.Contains(debrisLocation.ToPoint()))
            {
                points.Add(debrisLocation.ToPoint());      
                Logger.Info($"debris tile {debrisLocation.ToPoint()}");
            }
        }
        await PickUpDebris(startPoint,points,canDestroy);
    }

    private static async Task PickUpDebris(Point startPoint, List<Point> debrisTiles,bool canDestroy = false)
    {
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);

        foreach (var debris in debrisTiles)
        {
            Logger.Info($"debris tile point: {debris}");
            if (InMagneticRadius(BotBase.Farmer.MagneticRadius / Game1.tileSize, debris))
            {
                Logger.Info($"in magnetic radius: {debris}  {BotBase.Farmer.MagneticRadius / Game1.tileSize}");
                continue;
            }

            PathNode start = new PathNode(BotBase.Farmer.TilePoint, null);
            
            Stack<PathNode> path = await pathing.FindPath(start, new Goal.GoalPosition(debris.X, debris.Y), BotBase.CurrentLocation, 10000,canDestroy);
            if (path == new Stack<PathNode>())
            {
                Logger.Error($"Stack was empty");
                continue;
            }

            CharacterController.StartMoveCharacter(path, Game1.currentGameTime);

            while (CharacterController.IsMoving()) continue; // this is not async
        }
    }

    private static bool InMagneticRadius(int radius, Point tile)
    {
        Vector2 endPoint = new(tile.X + radius, tile.Y + radius);
        tile.X -= radius;
        tile.Y -= radius;
        for (int x = tile.X; x < endPoint.X; x++)
        {
            for (int y = tile.Y; y < endPoint.Y + radius; y++)
            {
                if (BotBase.Farmer.TilePoint == new Point(x, y)) return true;
            }
        }

        return false;
    }

    #endregion
    
}

/// <summary>
/// Get information about bot's character, this includes information like skills and relationship status
/// </summary>
public class PlayerInformation
{
    /// <summary>
    /// bot's current health
    /// </summary>
    public int Health => StardewClient.Farmer.health;

    public int MaxHealth => BotBase.Farmer.maxHealth;
    /// <summary>
    /// Bot's current stamina
    /// </summary>
    public float Stamina => StardewClient.Farmer.Stamina;
    public float MaxStamina => BotBase.Farmer.MaxStamina;
    /// <summary>
    /// Bot's current Inventory
    /// </summary>
    public Inventory Inventory => StardewClient.Farmer.Items;
    public int MaxItems => BotBase.Farmer.MaxItems;
    /// <summary>
    /// Bot's Current held Item
    /// </summary>
    public Item? HeldItem => BotBase.Farmer.CurrentItem;
    /// <summary>
    /// The rings currently equipped by the player, the first will be the left ring the second the right
    /// </summary>
    public List<NetRef<Ring>> EquippedRings => new() { BotBase.Farmer.leftRing, BotBase.Farmer.rightRing };
    /// <summary>
    /// Bot's current Trinkets
    /// </summary>
    public NetList<Trinket, NetRef<Trinket>> Trinkets => BotBase.Farmer.trinketItems;
    /// <summary>
    /// this can be used for relationship this information is cached by the game
    /// </summary>
    public IDictionary<string, CharacterData> NpcData => Game1.characterData;

    public NetRef<Hat> EquippedHat => BotBase.Farmer.hat;
    public NetRef<Clothing> EquippedShirt => BotBase.Farmer.shirtItem;
    public NetRef<Clothing> EquippedPants => BotBase.Farmer.pantsItem;
    public NetRef<Boots> EquippedBoots => BotBase.Farmer.boots;
    
    /// <summary>
    /// The pages that the active clickable menu can be if you are using the methods provided by the framework this is changed for you.
    /// </summary>
    public enum MenuStates
    {
        Inventory,
        Relationship,
        Skill,
        None,
    }
    
    /// <summary>
    /// Use this to get the character's name.
    /// </summary>
    public string GetFarmerName()
    {
        return StardewClient.Farmer.Name;
    }
    
    /// <summary>
    /// Get which inventory page they are in
    /// </summary>
    public static MenuStates MenuState = MenuStates.None;
    
    /// <summary>
    /// Get the level of all skills.
    /// </summary>
    /// <param name="showUI">Will show the skill UI when this is called, ExitMenu will need to be called after to exit. Defaults to false</param>
    /// <returns>The index's of the skills are as follows: farming, fishing, foraging, mining, combat and luck</returns>
    public List<int> SkillLevel(bool showUI = false)
    {
        if (showUI)
        {
            MenuState = PlayerInformation.MenuStates.Skill;
            Game1.activeClickableMenu = new GameMenu();
            if (Game1.activeClickableMenu is GameMenu gameMenu)
                Game1.activeClickableMenu.receiveLeftClick(gameMenu.tabs[1].bounds.X + 5, gameMenu.tabs[1].bounds.Y + 5);
        }
        
        List<int> skills = new();
        for (int i = 0; i < 5; i++)
        {
            skills.Add(StardewClient.Farmer.GetSkillLevel(i));
        }

        return skills;
    }

    /// <summary>
    /// Get the relationship level of all available characters, will play sound of going to menu even when instantExit is true
    /// </summary>
    /// <param name="instantExit">Will not show the game UI when getting values (will still play the sound though)</param>
    /// <returns>Will return the character's name and the heart level that would typically be displayed as a dictionary</returns>
    public Dictionary<string, int> RelationshipLevel(bool instantExit)
    {
        MenuState = MenuStates.Relationship;
        Dictionary<string, int> relationships = new();
        
        Game1.activeClickableMenu = new GameMenu();
        GameMenu gameMenu = Game1.activeClickableMenu as GameMenu;
        if (instantExit) 
            Game1.activeClickableMenu.receiveLeftClick(gameMenu!.tabs[2].bounds.X + 5,gameMenu.tabs[2].bounds.Y + 5,false);
        else            
            Game1.activeClickableMenu.receiveLeftClick(gameMenu!.tabs[2].bounds.X + 5,gameMenu.tabs[2].bounds.Y + 5);

        SocialPage socialTabPage = gameMenu.pages[2] as SocialPage;
        
        List<SocialPage.SocialEntry> levels = socialTabPage.FindSocialCharacters();
        
        if (instantExit) ExitMenu();
        
        foreach (var socialEntry in levels)
        {
            relationships.Add(socialEntry.Character.Name,socialEntry.HeartLevel);
        }

        return relationships;
    }

    /// <summary>
    /// Will open the inventory menu
    /// </summary>
    /// <returns></returns>
    public Inventory OpenInventory()
    {
        MenuState = MenuStates.Inventory;
        Game1.activeClickableMenu = new GameMenu();
        GameMenu? gameMenu = Game1.activeClickableMenu as GameMenu;
        Game1.activeClickableMenu.receiveLeftClick(gameMenu!.tabs[0].bounds.X + 5,gameMenu.tabs[0].bounds.Y + 5);

        InventoryPage? tabPage = gameMenu.pages[0] as InventoryPage;

        return StardewClient.Farmer.Items;
    }
    
    public void ExitMenu()
    {
        MenuState = MenuStates.None;
        Game1.activeClickableMenu = null;
    }
}