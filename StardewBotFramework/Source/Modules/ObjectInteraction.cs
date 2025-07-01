using Microsoft.VisualBasic.CompilerServices;
using Microsoft.Xna.Framework.Graphics;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

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
        Furniture furniture = selectedObject as Furniture;
        return furniture.clicked(StardewClient.Farmer);
    }

    /// <summary>
    /// Will remove an object that is <see cref="Furniture"/>, there are no checks for this so you will have to implement them yourself if you want it to work like vanilla.
    /// </summary>
    /// <param name="selectedObject">Object to pick up</param>
    /// <returns>true if you can pick up else false</returns>
    public bool PickUpFurniture(Object selectedObject)
    {
        Logger.Info($"Object category: {selectedObject.Category}");
        Furniture furniture = selectedObject as Furniture;
        if (furniture.canBeRemoved(StardewClient.Farmer))
        {
            // furniture.performRemoveAction();
            furniture.AttemptRemoval(delegate(Furniture f)
            {
                Guid guid = StardewClient.CurrentLocation.furniture.GuidOf(f);
                
                StardewClient.AddRemoveFurniture(StardewClient.CurrentLocation,guid); // use reflection to add furniture to be removed (very dodgy solution, but it works so)
            });
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Use for interaction with objects this includes furniture this replicates rightclicking it
    /// </summary>
    /// <param name="selectedObject"><see cref="Object"/> to interact with</param>
    /// <returns>Returns true if the action was performed, or false if the player should pick up the item instead.</returns>
    public bool InteractWithObject(Object selectedObject)
    {
        return selectedObject.checkForAction(StardewClient.Farmer, false); // this should change to checkForActionOn{Object} in this function
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
        return Utility.tryToPlaceItem(StardewClient.CurrentLocation, item, x, y);
    }
}