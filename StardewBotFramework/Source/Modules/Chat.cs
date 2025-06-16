using Microsoft.Xna.Framework;
using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class Chat
{
    private ChatBox _chat = new ChatBox();
    
    /// <summary>
    /// Send chat message to everyone including the current player, This will also work in single-player.
    /// </summary>
    /// <param name="message">The message you would like to send</param>
    public void SendPublicMessage(string message)
    {
        try
        {
            Game1.chatBox.textBoxEnter(message);
        }
        catch (Exception e)
        {
            Logger.Warning($"separate other error");
            Logger.Error($"{e}");
            Logger.Error($"issue with {message} from {Game1.player.UniqueMultiplayerID}  using {LocalizedContentManager.CurrentLanguageCode}");
        }
    }

    /// <summary>
    /// Send private message to specific user, This only works in multiplayer. Has not been tested.
    /// </summary>
    /// <param name="playerName">This should be the same as you would use if you were inputting the command yourself.</param>
    /// <param name="message">The message to the player.</param>
    public void SendPrivateMessage(string playerName,string message)
    {
        string formattedMessage = $"/dm {playerName} {message}";
        
        Game1.chatBox.textBoxEnter(formattedMessage); // Not tested looks good to me though
    }

    /// <summary>
    /// Use emote you can get see all emotes https://stardewvalleywiki.com/Multiplayer/Emotes.
    /// </summary>
    /// <param name="emote">the name of the emote this should be the same a normal player would use. You can see them here</param>
    public bool UseEmote(string emote) // can access emotes with Farmer.EMOTES
    {
        emote = emote.ToLower();

        foreach (var emoteType in Farmer.EMOTES)
        {
            if (emoteType.emoteString != emote) continue;
            
            string message = "/emote " + emote;
            _chat.textBoxEnter(message);
            return true;
        }

        return false;
    }
        
    /// <summary>
    /// Change colour of chat messages for this player.
    /// </summary>
    /// <param name="colour">available colours, you can see available colours here https://stardewvalleywiki.com/Multiplayer#Chat under color-list.</param>
    /// <returns>Will return false if colour is not available in the game else true.</returns>
    public bool ChangeColour(string colour)
    {
        colour = colour.ToLower();
        
        // default return from getColor is white.
        if (ChatMessage.getColorFromName(colour) == Color.White && colour != "white")
        {
            return false;
        }
        
        string message = "/color " + colour;
        _chat.textBoxEnter(message);
        
        // Couldn't get changing default player color to work with the method we use for chatting
        // Game1.player.defaultChatColor = chatColour;
        return true;
    }
}