using Microsoft.Xna.Framework;
using Netcode;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.Modules.Pathfinding.Algorithms;
using StardewBotFramework.Source.Modules.Pathfinding.Base;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Characters;
using StardewValley.Inventories;
using StardewValley.Menus;
using StardewValley.Objects.Trinkets;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// Methods related to the player character, this ranges from eating food to look in a different direction.
/// </summary>
public class Player
{
    public int Health => StardewClient.Farmer.health;
    public float Stamina => StardewClient.Farmer.Stamina;
    public Inventory Inventory => StardewClient.Farmer.Items;
    public Item? HeldItem => StardewClient.Farmer.CurrentItem;
    public NetList<Trinket, NetRef<Trinket>> Trinkets => StardewClient.Farmer.trinketItems;
    public IDictionary<string, CharacterData> NpcData => Game1.characterData;// this can be used for relationship and stuff this information is cached by the game

    /// <summary>
    /// Goes 0-3 from North,East,South,West
    /// </summary>
    public int FacingDirection => StardewClient.Farmer.FacingDirection;

    /// <summary>
    /// Changes direction the player sprite is facing
    /// </summary>
    /// <param name="direction">Goes 0-3 from North,East,South,West</param>
    public void ChangeFacingDirection(int direction)
    {
        Game1.player.FacingDirection = direction;
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
    /// Will use currently held tool.
    /// </summary>
    /// <param name="direction">This acts as a shortcut to <see cref="ChangeFacingDirection"/>. If this is not set or not a valid value the tool will be used in the currently facing direction</param>
    public void UseTool(int direction = -1)
    {
        Logger.Info($"direction: {direction}");
        if (direction < 0 || direction > 4) // should account for if user uses a none valid value
        {
            Logger.Info($"Not using direction at tool");
            Game1.player.BeginUsingTool(); // Object.performToolAction
            return;
        }
        
        Logger.Info($"using tool at direction: {direction}");
        ChangeFacingDirection(direction);
        switch (direction) // N,E,S,W
        {
            case 0:
                StardewClient.Farmer.lastClick = new Vector2(StardewClient.Farmer.lastClick.X,
                    StardewClient.Farmer.lastClick.Y - 1 * 64);
                StardewClient.Farmer.BeginUsingTool();
                break;
            case 1:
                StardewClient.Farmer.lastClick = new Vector2(StardewClient.Farmer.lastClick.X - 1 * 64,
                    StardewClient.Farmer.lastClick.Y);
                StardewClient.Farmer.BeginUsingTool();
                break;
            case 2:
                StardewClient.Farmer.lastClick = new Vector2(StardewClient.Farmer.lastClick.X,
                    StardewClient.Farmer.lastClick.Y + 1 * 64);
                StardewClient.Farmer.BeginUsingTool();
                break;
            case 3:
                StardewClient.Farmer.lastClick = new Vector2(StardewClient.Farmer.lastClick.X + 1 * 64,
                    StardewClient.Farmer.lastClick.Y);
                StardewClient.Farmer.BeginUsingTool();
                break;
        }
    }

    public async Task UseToolOnGroup(List<TerrainFeature> group,Tool tool)
    {
        if (BotBase.Farmer.Items.Contains(tool))
        {
            int index = BotBase.Farmer.Items.IndexOf(tool);
            for (int i = 0; i < (int)Math.Floor((double)index / 11); i++)
            {
                BotBase.Farmer.shiftToolbar(true);    
            }

            BotBase.Farmer.CurrentToolIndex = index;
        }
        
        AlgorithmBase.IPathing pathing = new AStar.Pathing();
        pathing.BuildCollisionMap(BotBase.CurrentLocation);
        foreach (var point in group) // TODO: the use tool at direction is very inaccurate I think this is partially related to the other todo
        {
            StardewClient.debugTiles.Remove(point);
            if (Graph.IsInNeighbours(BotBase.Farmer.TilePoint, point.Tile.ToPoint(), out var direction, 3))
            {
                Logger.Info($"using neighbour if");
                if (direction == -1) continue;
                UseTool(direction);
            }
            else // pathfind to node
            {
                PathNode start = new PathNode(BotBase.Farmer.TilePoint.X, BotBase.Farmer.TilePoint.Y, null);
                
                // TODO: pathfinding lets going on top of the plant on not next to it this also happens with GoalNearby with a radius of 1
                Stack<PathNode> path = await pathing.FindPath(start,new Goal.GetToTile((int)point.Tile.X,(int)point.Tile.Y),BotBase.CurrentLocation,10000);
        
                CharacterController.StartMoveCharacter(path, Game1.player, Game1.currentLocation,
                    Game1.currentGameTime);

                while (CharacterController.IsMoving()) continue; // this is not async

                if (!Graph.IsInNeighbours(BotBase.Farmer.TilePoint, point.Tile.ToPoint(), out var pathDirection, 3)) continue;
                UseTool(pathDirection);
            }
        }
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
}

/// <summary>
/// Get information about bot's character, this includes information like skills and relationship status
/// </summary>
public class PlayerInformation
{
    
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