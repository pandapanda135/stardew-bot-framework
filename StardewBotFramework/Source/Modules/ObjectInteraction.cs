using StardewValley;
using Object = StardewValley.Object;

namespace StardewBotFramework.Source.Modules;

public class ObjectInteraction
{
    public void InteractWithObject(Object selectedObject)
    {
        selectedObject.checkForAction(StardewClient.Farmer, false); // this should change to checkForActionOn{Object} in this function
    }
}