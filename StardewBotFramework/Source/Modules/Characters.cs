using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;

namespace StardewBotFramework.Source.Modules;

public class Characters
{
    public Dictionary<Point, NPC> GetCharactersInCurrentLocation(GameLocation location)
    {
        Dictionary<Point, NPC> characters = new();
        
        // remove one as to not go around map
        int maxX = location.Map.DisplayWidth / Game1.tileSize;
        int maxY = location.Map.DisplayHeight / Game1.tileSize; 

        for (int x = 0; x <= maxX; x++)
        {
            for (int y = 0; y <= maxY; y++)
            {
                if (Utility.checkForCharacterInteractionAtTile(new Vector2(x, y), Game1.player))
                {
                    NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == new Point(x,y), includeEventActors: false);

                    if (character is null) continue;
                    
                    if (characters.ContainsValue(character))
                    {
                        continue;
                    }
                    characters.Add(new Point(x, y),character);
                }
                    
            }
        }

        return characters;
    }

    /// <summary>
    /// Get the number of characters in the radius of a position
    /// </summary>
    /// <param name="position"><see cref="Point"/> of position you want to query</param>
    /// <param name="radius">Tile radius of where you want query</param>
    /// <returns>Will return amount of characters nearby</returns>
    public int GetNumberOfCharacterNearby(Point position, int radius)
    {
        return Utility.getNumberOfCharactersInRadius(BotBase.CurrentLocation, position, radius);
    }
}