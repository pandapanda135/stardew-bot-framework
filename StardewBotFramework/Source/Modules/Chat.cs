using StardewBotFramework.Debug;
using StardewValley;
using StardewValley.Menus;

namespace StardewBotFramework.Source.Modules;

public class Chat
{
    private ChatBox _chat = new ChatBox();
    
    List<string> emoteList = new (){ "happy","sad","heart" };
    
    /// <summary>
    /// Send chat message to everyone including the current player, This will also work in single-player.
    /// </summary>
    /// <param name="message">The message you would like to send</param>
    public void SendPublicMessage(string message)
    {
        try
        {
            _chat.textBoxEnter(message);
        }
        catch (Exception e) // issue with parseText idk why
        {
            Logger.Info($"separate other error");
            Logger.Error($"{e}");
            Logger.Error($"issue with {message}");
        }
    }

    public void SendPrivateMessage(string message,long playerId)
    {
        string[] messageArray = { message };
        ChatCommands.DefaultHandlers.Message(messageArray, _chat); // hasn't been tested if works
    }

    public void UseEmote(string emote) // can access emotes with Farmer.EMOTES
    {
        emote = emote.ToLower();

        string message = "/emote " + emote;

        try
        {
            _chat.textBoxEnter(message);
        }
        catch (Exception e)
        {
            Logger.Error($"{emote} is not a valid emote");
        }
    }
}