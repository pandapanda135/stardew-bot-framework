using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class DialogueManager
{
    public Stack<Dialogue>? CurrentDialogueStack;
    public Dialogue? CurrentDialogue;

    public NPC CurrentNpc;
    
    /// <summary>
    /// Get if there is a <see cref="NPC"/> at a tile
    /// </summary>
    /// <returns>Will return true if NPC at tile else false</returns>
    public bool CheckForCharacterAtTile(Point tileLocation)
    {
        return Utility.checkForCharacterInteractionAtTile(tileLocation.ToVector2(), Game1.player);
    }

    /// <summary>
    /// Get the character that is at a specified tile
    /// </summary>
    /// <returns>Will return the character if there is one at the tile location else will return null</returns>
    public NPC? GetCharacterAtTile(Point tileLocation)
    {
        foreach (var npc in Utility.getAllCharacters())
        {
            if (npc.TilePoint == tileLocation) return npc;
        }

        return null;
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
    public bool InteractWithCharacter(NPC character,out Stack<Dialogue> dialogues,out Dialogue loadedDialogue)
    {
        bool checkAction = character.checkAction(Game1.player, Game1.currentLocation); 
        dialogues = character.CurrentDialogue;
        CurrentDialogueStack = character.CurrentDialogue;
        
        loadedDialogue = CurrentDialogueStack.Pop();
        CurrentDialogue = loadedDialogue;
        
        CurrentNpc = character;
        return checkAction;
    }
    
    /// <summary>
    /// Try to interact with a character based on their name.
    /// </summary>
    /// <param name="characterName">Character's name</param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character or will return null if character with name does not exist.</returns>
    public bool? InteractWithCharacter(string characterName, out Stack<Dialogue>? dialogues,out Dialogue loadedDialogue)
    {
        NPC character = Game1.getCharacterFromName(characterName, false, false);
        bool checkAction = InteractWithCharacter(character, out var stack,out loadedDialogue);
        loadedDialogue = character.TryGetDialogue(character.LoadedDialogueKey);
        CurrentDialogue = loadedDialogue;

        dialogues = stack;
        return checkAction;
    }

    /// <summary>
    /// Try to interact with character at tile
    /// </summary>
    /// <param name="tileLocation">Tile character is at</param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <returns>will return true if interacted with else will either return false as cannot interact with the character or will return null if there is no character there.</returns>
    public bool? InteractWithCharacter(Point tileLocation,out Stack<Dialogue>? dialogues,out Dialogue loadedDialogue)
    {
        NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tileLocation, includeEventActors: false);
        bool checkAction = InteractWithCharacter(character, out var stack,out loadedDialogue);
        CurrentDialogue = loadedDialogue;

        dialogues = stack;
        return checkAction;
    }

    /// <summary>
    /// Advance current dialogue
    /// </summary>
    /// <returns>Will return true if Game1.activeClickableMenu is <see cref="DialogueBox"/> else false</returns>
    public bool AdvanceDialogBox(out string dialogueLine,int x = 0, int y = 0, bool playSound = true)
    {
        if (Game1.activeClickableMenu is DialogueBox)
        {
            Game1.activeClickableMenu.receiveLeftClick(x, y, playSound);
            if (CurrentDialogue is null)
            {
                Logger.Error($"CurrentDialogue is null in advance dialogue");
                dialogueLine = "";
                return false;
            }
            dialogueLine = !CurrentDialogue.isOnFinalDialogue() ? CurrentNpc.Dialogue[CurrentNpc.LoadedDialogueKey] : "";
            return true;
        }

        dialogueLine = "";
        return false;
    }
    
    /// <summary>
    /// Choose response from the current dialogue
    /// </summary>
    /// <param name="response">The <see cref="Response"/> you want to pick</param>
    public void ChooseResponse(Response response)
    {
        if (Game1.activeClickableMenu is not DialogueBox) return;

        Response[] responses = PossibleResponses()!;
        for (int i = 0; i < responses.Length; i++)
        {
            if (responses[i] == response)
            {
                Logger.Info($"setting response to: {i}");
                StardewClient.ChangeSelectedResponse(i);
                break;
            }
        }
        
        Game1.activeClickableMenu.receiveLeftClick(0, 0, true);
        CurrentDialogue.chooseResponse(response);
    }
    
    /// <summary>
    /// The possible responses
    /// </summary>
    /// <returns><see cref="Array"/> of <see cref="Response"/>, if this dialogue is not a question it will return null</returns>
    public List<NPCDialogueResponse>? PossibleNpcDialogueResponses()
    {
        if (!CurrentDialogue.isCurrentDialogueAQuestion()) return null; // here

        return CurrentDialogue.getNPCResponseOptions();
    }

    public Response[]? PossibleResponses()
    {
        if (!CurrentDialogue.isCurrentDialogueAQuestion()) return null; // here

        return CurrentDialogue.getResponseOptions();
    }

    internal static void ChooseResponse(int option,DialogueBox dialogueBox,Response response)
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