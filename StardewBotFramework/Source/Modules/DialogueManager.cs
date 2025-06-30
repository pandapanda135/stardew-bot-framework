using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class DialogueManager
{
    public Stack<Dialogue>? CurrentDialogueStack;
    public Dialogue CurrentDialogue;

    public static List<Keys> ResponseHotKeys = new ();
    
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
                Logger.Warning($"Start new list of dialogue from character:");
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

    public Stack<Dialogue>? GetCharacterDialogue(Point tilePoint)
    {
        NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tilePoint, includeEventActors: false);

        if (character is null) return null;
        
        return character.CurrentDialogue; // get Game1.npcDialogues
    }
    
    /// <summary>
    /// Try to interact with a character.
    /// </summary>
    /// <param name="character">an <see cref="NPC"/></param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character.</returns>
    public bool InteractWithCharacter(NPC character,out Stack<Dialogue> dialogues)
    {
        dialogues = character.CurrentDialogue;
        CurrentDialogueStack = character.CurrentDialogue;
        return character.checkAction(Game1.player, Game1.currentLocation);
    }
    
    /// <summary>
    /// Try to interact with a character based on their name.
    /// </summary>
    /// <param name="characterName">Character's name</param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character or will return null if character with name does not exist.</returns>
    public bool? InteractWithCharacter(string characterName, out Stack<Dialogue>? dialogues)
    {
        NPC character = Game1.getCharacterFromName(characterName, false, false);

        if (character is null)
        {
            dialogues = null;
            return null;    
        }
        
        dialogues = character.CurrentDialogue;
        CurrentDialogueStack = character.CurrentDialogue;
        return character.checkAction(Game1.player, Game1.currentLocation);
    }

    /// <summary>
    /// Try to interact with character at tile
    /// </summary>
    /// <param name="tileLocation">Tile character is at</param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <returns>will return true if interacted with else will either return false as cannot interact with the character or will return null if there is no character there.</returns>
    public bool? InteractWithCharacter(Point tileLocation,out Stack<Dialogue>? dialogues)
    {
        NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tileLocation, includeEventActors: false);

        if (character is null)
        {
            dialogues = null;
            return null;    
        }
        
        dialogues = character.CurrentDialogue;
        CurrentDialogueStack = character.CurrentDialogue;
        return character.checkAction(Game1.player, Game1.currentLocation);
    }

    /// <summary>
    /// Advance current dialog
    /// </summary>
    /// <param name="x"/>
    /// <param name="y"/>
    /// <param name="playSound"/>
    /// <returns>Will return true if Game1.activeClickableMenu if is <see cref="DialogueBox"/> else false</returns>
    public bool AdvanceDialogBox(int x = 0, int y = 0, bool playSound = true)
    {
        if (Game1.activeClickableMenu is DialogueBox)
        {
            Game1.activeClickableMenu.receiveLeftClick(x, y, playSound);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Choose response from the current dialogue
    /// </summary>
    /// <param name="dialogue">The dialogue you want to respond to</param>
    /// <param name="response">The <see cref="Response"/> you want to pick</param>
    public void ChooseResponse(Dialogue dialogue,Response response)
    {
        if (Game1.activeClickableMenu is not DialogueBox) return;

        Response[] responses = PossibleResponses(dialogue)!;
        for (int i = 0; i < responses.Length; i++)
        {
            if (responses[i] == response)
            {
                Logger.Info($"setting response to: {i}");
                StardewClient.ChangeSelectedResponse(i);
                break;
            }
        }
        
        Game1.activeClickableMenu.receiveLeftClick(0, 0 , true);
        dialogue.chooseResponse(response);
    }
    
    /// <summary>
    /// The possible responses
    /// </summary>
    /// <param name="dialogue">The dialogue you want the responses to</param>
    /// <returns><see cref="Array"/> of <see cref="Response"/>, if this dialogue is not a question it will return null</returns>
    public List<NPCDialogueResponse>? PossibleNpcDialogueResponses(Dialogue dialogue)
    {
        if (ResponseHotKeys.Count < 0)
        {
            ResponseHotKeys.Clear();
        }
        
        if (!dialogue.isCurrentDialogueAQuestion()) return null;

        foreach (var response in dialogue.getNPCResponseOptions())
        {
            ResponseHotKeys.Add(response.hotkey);
        }
        
        return dialogue.getNPCResponseOptions();
    }

    public Response[]? PossibleResponses(Dialogue dialogue)
    {
        ResponseHotKeys.Clear();
        if (!dialogue.isCurrentDialogueAQuestion()) return null;

        foreach (var response in dialogue.getResponseOptions())
        {
            ResponseHotKeys.Add(response.hotkey);
        }
        
        return dialogue.getResponseOptions();
    }

    internal static void ChooseResponse(int option,DialogueBox dialogueBox,Dialogue dialogue,Response response)
    {
        if (Game1.activeClickableMenu is not DialogueBox) return;

        for (int i = 0; i < dialogueBox.responses.Length; i++)
        {
            if (dialogueBox.responses[i] == response)
            {
                StardewClient.ChangeSelectedResponse(option);

                dialogueBox = (Game1.activeClickableMenu as DialogueBox);
                // if (dialogueBox.allClickableComponents == null)
                // {
                //     Logger.Warning($"allclickablecomponents is null");
                //     return;
                // }
                //
                // if (dialogueBox.responseCC == null)
                // {
                //     Logger.Warning($"responseCC is null");
                //     return;
                // }

                // dialogue.chooseResponse(response);
                
                dialogueBox.receiveLeftClick(460,860); // wont work as is not done transitioning to button's appearing
            }
        }
        
        // Logger.Info($"setting response to: {option}");
        //
        // Game1.activeClickableMenu.receiveLeftClick(0, 0 , true);
        //
        // if (dialogueBox?.responses == null || dialogue == null || response == null)
        // {
        //     Logger.Warning($"dialogueBox.responses or responseCC is null.  {response}");
        //     return;
        // }
        //
        // dialogue.chooseResponse(response); // dialogue is null
    }

    internal static bool AdvanceDialogue(int x,int y)
    {
        if (Game1.activeClickableMenu is DialogueBox)
        {
            Game1.activeClickableMenu.receiveLeftClick(x, y);
            return true;
        }

        return false;
    }
}