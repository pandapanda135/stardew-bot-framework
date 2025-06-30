using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

public class ObjectInteraction
{
    /// <summary>
    /// Use for interaction with objects this includes furniture this replicates rightclicking it
    /// </summary>
    /// <param name="selectedObject"><see cref="Object"/> to interact with</param>
    /// <returns>Returns true if the action was performed, or false if the player should pick up the item instead.</returns>
    public bool InteractWithObject(Object selectedObject)
    {
        // selectedObject.clicked(StardewClient.Farmer);
        return selectedObject.checkForAction(StardewClient.Farmer, false); // this should change to checkForActionOn{Object} in this function
    }
}