using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class DialogueManager
{
    /// <summary>
    /// Get if there is a <see cref="NPC"/> at a tile
    /// </summary>
    /// <returns>Will return true if NPC at tile else false</returns>
    public bool CheckForCharacterAtTile(Point tileLocation)
    {
        return Utility.checkForCharacterInteractionAtTile(tileLocation.ToVector2(), Game1.player);
    }
    
    // could use CurrentDialogue in NPC Class
    // checkForCharacterInteractionAtTile in Utility
    
    /// <summary>
    /// Gets all possible dialogue from the character at the tile
    /// </summary>
    /// <returns>Will either return a dictionary of speaker name then text or null if there is no character at the tile</returns>
    public Dictionary<string,List<string>>? GetAllPossibleDialogue(Point tileLocation)
    {
        if (CheckForCharacterAtTile(tileLocation))
        {
            Dictionary<string, List<string>> Dialogue = new();
            
            NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tileLocation, includeEventActors: false);

            if (character == null) return null;

            foreach (var dialogue in character.CurrentDialogue)
            {
                foreach (var dialogueLine in dialogue.dialogues)
                {
                    Logger.Info($"speaker: {dialogue.speaker.Name}  string: {dialogueLine.Text}"); // prints all the text possible by the speaker at that moment
                    if (Dialogue.ContainsKey(dialogue.speaker.Name))
                    {
                        Dialogue[dialogue.speaker.Name].Add(dialogueLine.Text);
                        continue;
                    }
                    Dialogue.Add(dialogue.speaker.Name,new (){dialogueLine.Text});
                }
            }

            return Dialogue;
        }

        return null;
    }

    /// <summary>
    /// Try to interact with a character.
    /// </summary>
    /// <param name="character">an <see cref="NPC"/></param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character.</returns>
    public bool InteractWithCharacter(NPC character)
    {
        return character.checkAction(Game1.player, Game1.currentLocation);
    }
    
    /// <summary>
    /// Try to interact with a character based on their name.
    /// </summary>
    /// <param name="characterName">Character's name</param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character or will return null if character with name does not exist.</returns>
    public bool? InteractWithCharacter(string characterName)
    {
        NPC character = Game1.getCharacterFromName(characterName, false, false);
        
        if (character is null) return null;
        return character.checkAction(Game1.player, Game1.currentLocation);
    }

    /// <summary>
    /// Try to interact with character at tile
    /// </summary>
    /// <param name="tileLocation">Tile character is at</param>
    /// <returns>will return true if interacted with else will either return false as cannot interact with the character or will return null if there is no character there.</returns>
    public bool? InteractWithCharacter(Point tileLocation)
    {
        NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tileLocation, includeEventActors: false);

        if (character is null) return null;
        return character.checkAction(Game1.player, Game1.currentLocation);
    }

    /// <summary>
    /// Advance current dialog
    /// </summary>
    /// <param name="dialogueBox">the Dialogue box you want to advance </param>
    /// <param name="x"/>
    /// <param name="y"/>
    /// <param name="playSound"/>
    public void AdvanceDialogBox(DialogueBox dialogueBox, int x = 0, int y = 0, bool playSound = true)
    {
        dialogueBox.receiveLeftClick(x, y, playSound);
    }

    /// <summary>
    /// Choose response from the current dialogue
    /// </summary>
    /// <param name="dialogue">The dialogue you want to respond to</param>
    /// <param name="response">The <see cref="Response"/> you want to pick</param>
    public void ChooseResponse(Dialogue dialogue,Response response)
    {
        dialogue.chooseResponse(response);
    }
    
    /// <summary>
    /// The possible responses
    /// </summary>
    /// <param name="dialogue">The dialogue you want the responses to</param>
    /// <returns><see cref="Array"/> of <see cref="Response"/></returns>
    public Response[] PossibleResponses(Dialogue dialogue)
    {
        return dialogue.getResponseOptions();
    }
}