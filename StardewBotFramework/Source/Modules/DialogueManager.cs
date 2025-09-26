using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class DialogueManager
{
    public Stack<Dialogue>? CurrentDialogueStack;

    /// <summary>
    /// The current dialogue, this should not need to be set.
    /// </summary>
    public Dialogue? CurrentDialogue => CurrentDialogueBox?.characterDialogue;
    
    public DialogueBox? CurrentDialogueBox = null;
    
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
            Dictionary<string, List<string>> dialogues = new();
            
            NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tileLocation, includeEventActors: false);

            if (character == null) return null;

            foreach (var dialogue in character.CurrentDialogue)
            {
                Logger.Warning($"Start new list of dialogue from character:");
                foreach (var dialogueLine in dialogue.dialogues)
                {
                    Logger.Info($"speaker: {dialogue.speaker.Name}  string: {dialogueLine.Text}"); // prints all the text possible by the speaker at that moment
                    if (dialogues.ContainsKey(dialogue.speaker.Name))
                    {
                        dialogues[dialogue.speaker.Name].Add(dialogueLine.Text);
                        continue;
                    }
                    dialogues.Add(dialogue.speaker.Name,new (){dialogueLine.Text});
                }
            }

            return dialogues;
        }

        return null;
    }

    public Stack<Dialogue>? GetCharacterDialogue(Point tilePoint)
    {
        NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tilePoint, includeEventActors: false);

        return character.CurrentDialogue ?? null;
    }
    
    /// <summary>
    /// Try to interact with a character.
    /// </summary>
    /// <param name="character">an <see cref="NPC"/></param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <param name="loadedDialogue">The currently loaded dialogue from the character</param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character.</returns>
    public bool InteractWithCharacter(NPC character,out Stack<Dialogue> dialogues,out Dialogue loadedDialogue)
    {
        bool checkAction = character.checkAction(Game1.player, Game1.currentLocation); 
        dialogues = character.CurrentDialogue;
        CurrentDialogueStack = character.CurrentDialogue;
        
        loadedDialogue = CurrentDialogueStack.Pop();
        
        return checkAction;
    }
    
    /// <summary>
    /// Try to interact with a character based on their name.
    /// </summary>
    /// <param name="characterName">Character's name</param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <param name="loadedDialogue">The currently loaded dialogue from the character</param>
    /// <returns>will return true if interacted with else will return false as cannot interact with the character or will return null if character with name does not exist.</returns>
    public bool? InteractWithCharacter(string characterName, out Stack<Dialogue>? dialogues,out Dialogue loadedDialogue)
    {
        NPC character = Game1.getCharacterFromName(characterName, false);
        bool checkAction = InteractWithCharacter(character, out var stack,out loadedDialogue);
        loadedDialogue = character.TryGetDialogue(character.LoadedDialogueKey);

        dialogues = stack;
        return checkAction;
    }

    /// <summary>
    /// Try to interact with character at tile
    /// </summary>
    /// <param name="tileLocation">Tile character is at</param>
    /// <param name="dialogues">A Stack of all possible dialogues from this character</param>
    /// <param name="loadedDialogue">The currently loaded dialogue from the character</param>
    /// <returns>will return true if interacted with else will either return false as cannot interact with the character or will return null if there is no character there.</returns>
    public bool? InteractWithCharacter(Point tileLocation,out Stack<Dialogue>? dialogues,out Dialogue loadedDialogue)
    {
        NPC? character = Game1.GetCharacterWhere<NPC>(npc => npc.TilePoint == tileLocation, includeEventActors: false);
        bool checkAction = InteractWithCharacter(character, out var stack,out loadedDialogue);

        dialogues = stack;
        return checkAction;
    }

    /// <summary>
    /// Advance current dialogue, this is done by simulating a left click, this can cause skipping the dialogue box instead of advancing it properly.
    /// </summary>
    /// <returns>Will return false if there is no current string or <see cref="Game1.activeClickableMenu"/> is not <see cref="DialogueBox"/> else false</returns>
    public bool AdvanceDialogBox(out string dialogueLine,int x = 0, int y = 0, bool playSound = true)
    {
        dialogueLine = "";

        if (CurrentDialogueBox is null) return false;
        if (Game1.activeClickableMenu is not DialogueBox) return false;
        
        Game1.activeClickableMenu.receiveLeftClick(x, y, playSound);
        dialogueLine = CurrentDialogueBox.getCurrentString();
        return dialogueLine != "";
    }
    
    /// <summary>
    /// Choose response from the current dialogue
    /// </summary>
    /// <param name="response">The <see cref="Response"/> you want to pick</param>
    public void ChooseResponse(Response response)
    {
        if (Game1.activeClickableMenu is not DialogueBox) return;

        Response[] responses = PossibleResponses();
        for (int i = 0; i < responses.Length; i++)
        {
            if (responses[i] != response) continue;
            
            Logger.Info($"setting response to: {i}");
            BotBase.ChangeSelectedResponse(i);
            break;
        }

        // I think this is related to event stuff kinda forgot.
        if (CurrentDialogue is null)
        {
            if (CurrentDialogueBox is null)
            {
                Logger.Error($"current dialogue box is not null");
                return;
            }
            if (Game1.eventUp && Game1.currentLocation.afterQuestion == null)
            {
                Game1.playSound("smallSelect");
                Game1.currentLocation.currentEvent.answerDialogue(Game1.currentLocation.lastQuestionKey, CurrentDialogueBox.selectedResponse);
            }
            else
            {
                Game1.currentLocation.answerDialogue(responses[CurrentDialogueBox.selectedResponse]);
            }
            CurrentDialogueBox.selectedResponse = -1;
            IReflectedMethod? method = BotBase.GetTryOutro(CurrentDialogueBox);
            if (method is not null) method.Invoke();
            else
            {
                Logger.Error($"method is null");
                return;
            }
            return;
        }
        
        Game1.activeClickableMenu.receiveLeftClick(0, 0);
        CurrentDialogue.chooseResponse(response);
    }
    
    /// <summary>
    /// The possible responses
    /// </summary>
    /// <returns><see cref="Array"/> of <see cref="Response"/>, if this dialogue is not a question it will return null</returns>
    public List<NPCDialogueResponse>? PossibleNpcDialogueResponses()
    {
        if (CurrentDialogue is null) return new();
        if (!CurrentDialogue.isCurrentDialogueAQuestion()) return null; // here

        return CurrentDialogue.getNPCResponseOptions();
    }

    /// <summary>
    /// Get all possible <see cref="Response"/>, this is recommended over NpcDialogues.
    /// </summary>
    public Response[] PossibleResponses()
    {
        if (CurrentDialogueBox is null) return Array.Empty<Response>();
        return CurrentDialogueBox.responses;
    }

    internal static void ChooseResponse(int option,DialogueBox? dialogueBox,Response response)
    {
        if (Game1.activeClickableMenu is not DialogueBox) return;

        for (int i = 0; i < dialogueBox?.responses.Length; i++)
        {
            if (dialogueBox.responses[i] == response)
            {
                BotBase.ChangeSelectedResponse(option);

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
                
                dialogueBox?.receiveLeftClick(460,860); // wont work as is not done transitioning to button's appearing
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