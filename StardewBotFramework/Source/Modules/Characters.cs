using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;

namespace StardewBotFramework.Source.Modules;

public class Characters
{
    public Dictionary<string, Point> GetCharactersInCurrentLocation(GameLocation location)
    {
        Dictionary<string, Point> characters = new();
        
        // remove one as to not go around map
        int maxX = location.Map.DisplayWidth / Game1.tileSize;
        int maxY = location.Map.DisplayHeight / Game1.tileSize; 

        Logger.Error($"size of map {maxX},{maxY}");

        for (int x = 0; x <= maxX; x++)
        {
            for (int y = 0; y <= maxY; y++)
            {
                if (Utility.checkForCharacterInteractionAtTile(new Vector2(x, y), Game1.player))
                {
                    NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == new Point(x,y), includeEventActors: false);
                    
                    if (characters.ContainsKey(character.Name))
                    {
                        continue;
                    }
                    characters.Add(character.Name, new Point(x, y));
                }
                    
            }
        }

        return characters;
    }
}