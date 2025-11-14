using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewBotFramework.Source.ObjectToolSwaps;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

/// <summary>
/// This is for interacting with <see cref="Object"/> this includes <see cref="Furniture"/>
/// </summary>
public class ObjectInteraction
{
    /// <summary>
    /// Will pick up an object that is on top of another e.g. The mug on top of the table in the farm house.
    /// There are no checks for this, so you will need to implement them yourself if you want this to work like vanilla 
    /// </summary>
    /// <param name="selectedObject">Object that holds the item you want to get</param>
    /// <returns>Will return false if the object has no item on top of if it can't get the object else will return true </returns>
    public bool PickUpItemOnFurniture(Object selectedObject)
    {
        Furniture? furniture = selectedObject as Furniture;
        if (furniture is null) return false;
        return furniture.clicked(BotBase.Farmer);
    }

    /// <summary>
    /// Will remove an object that is <see cref="Furniture"/>, there are no checks for this so you will have to implement them yourself if you want it to work like vanilla.
    /// </summary>
    /// <param name="selectedObject">Object to pick up</param>
    /// <returns>true if you can pick up else false</returns>
    public bool PickUpFurniture(Object selectedObject)
    {
        Logger.Info($"Object category: {selectedObject.Category}");
        Furniture? furniture = selectedObject as Furniture;
        if (furniture is null) return false;
        if (furniture.canBeRemoved(BotBase.Farmer))
        {
            // furniture.performRemoveAction();
            furniture.AttemptRemoval(delegate(Furniture f)
            {
                Guid guid = BotBase.CurrentLocation.furniture.GuidOf(f);
                
                BotBase.AddRemoveFurniture(BotBase.CurrentLocation,guid); // use reflection to add furniture to be removed (very dodgy solution, but it works so)
            });
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Use for interaction with objects this includes furniture this replicates right-clicking it
    /// </summary>
    /// <param name="selectedObject"><see cref="Object"/> to interact with</param>
    /// <returns>Returns true if the action was performed, or false if the player should pick up the item instead.</returns>
    public bool InteractWithObject(Object selectedObject)
    {
        return selectedObject.checkForAction(BotBase.Farmer); // this should change to checkForActionOn{Object} in this function
    }

    /// <summary>
    /// This will run the required functions to collect a quest object, quest objects can be found in GameLocation's overlayObject.
    /// This will also try to switch currently active slot to the first empty one to make sure a tool cannot get in the way to interacting. 
    /// </summary>
    /// <param name="o">The quest object</param>
    /// <returns>if the object has been interacted with.</returns>
    public void InteractWithQuestObject(Object o)
    {
        Point point = o.TileLocation.ToPoint();
        SwapItemHandler.EquipFirstEmptySlot();
        BotBase.Instance?.Helper.Input.SetCursorPosition(point.X * Game1.tileSize + 32,point.Y * Game1.tileSize + 32);
        BotBase.Instance?.Helper.Input.OverrideButton(SButton.MouseRight,true);
        o.performUseAction(BotBase.CurrentLocation);
    }

    /// <summary>
    /// This replicates interacting with a terrain feature with an empty inventory slot and right-clicking it. If you want to use a tool or item, e.g. add a tapper, you should use Tool.UseTool
    /// </summary>
    /// <param name="feature"></param>
    /// <param name="tile"></param>
    /// <returns></returns>
    public bool InteractWithTerrainFeature(TerrainFeature feature,Vector2 tile)
    {
        return feature.performUseAction(tile);
    }

    /// <summary>
    /// Will try to place an object in the current location, This will work for any object that can be placed down, this
    /// includes but is not limited to chests, seeds and fertilizer.
    /// </summary>
    /// <param name="item">This an <see cref="Object"/> of the item you want to place</param>
    /// <param name="x">This the X of the pixel location of where you want to place the item</param>
    /// <param name="y">This the Y of the pixel location of where you want to place the item</param>
    /// <returns>Will return true if placing the item was a success else false</returns>
    public bool TryToPlaceObject(Object item, int x, int y)
    {
        return Utility.tryToPlaceItem(BotBase.CurrentLocation, item, x, y);
    }

    /// <summary>
    /// Gets the object at the specified tile, this includes furniture.
    /// </summary>
    public Object? GetObjectAtTile(int x, int y)
    {
        foreach (var dictionary in BotBase.CurrentLocation.Objects)
        {
            var objs = dictionary.Values.Where(obj => obj.GetBoundingBox().Contains(x * Game1.tileSize, y * Game1.tileSize)).ToArray();
            if (!objs.Any()) continue;
            return objs[0];
        }

        var furniture = BotBase.CurrentLocation.furniture.Where(furniture => furniture.GetBoundingBox() // this is in pixels, x and y are tiles.
            .Contains(x * Game1.tileSize, y * Game1.tileSize)).ToArray();
        if (furniture.Any())
        {
            return furniture[0];
        }

        return null;
    }
}