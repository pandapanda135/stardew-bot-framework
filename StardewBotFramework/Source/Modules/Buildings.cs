using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Buildings;

namespace StardewBotFramework.Source.Modules;

public class Buildings
{
    /// <summary>
    /// This will do an action on the specified building, This is primarily used for interacting with doors.
    /// </summary>
    /// <param name="building">The building you want to interact with, you can get this from Game1.currentLocation.buildings</param>
    /// <param name="tileLocation">The tile you want to interact with as a <see cref="Vector2"/>.</param>
    /// <returns>Will return true if you can do the action else false (sometimes when this is false it will also show a message. You will have to that handle.)</returns>
    public bool DoBuildingAction(Building building,Vector2 tileLocation)
    {
        return building.doAction(tileLocation, Game1.player);
    }

    /// <summary>
    /// This will toggle an animal door at the specified building.
    /// </summary>
    /// <param name="building">The building that has the animal door.</param>
    public void UseAnimalDoor(Building building)
    {
        building.ToggleAnimalDoor(StardewClient.Farmer);
    }

}