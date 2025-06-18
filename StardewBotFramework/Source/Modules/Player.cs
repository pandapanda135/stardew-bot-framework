using Microsoft.Xna.Framework;
using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// Methods related to the player character, this ranges from eating food to look in a different direction.
/// </summary>
public class Player
{
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
        if (direction > 0 && direction < 4) // should account for if user uses a none valid value
        {
            ChangeFacingDirection(direction);
        }
        Game1.player.BeginUsingTool(); // Object.performToolAction
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