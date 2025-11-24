namespace StardewBotFramework.Source.Events.EventArgs;


public class ChatMessageReceivedEventArgs : System.EventArgs
{
    internal ChatMessageReceivedEventArgs(string playerName, string message, int chatKind,bool isBot)
    {
        PlayerName = playerName;
        Message = message;
        ChatKind = chatKind;
        IsBot = isBot;
    }

    public string PlayerName;

    public string Message;

    /// <summary>
    /// Chat kind is only meant to have these values, they may have values that are not shown here, and you will have to handle them.
    /// -1: These are for messages that are directly added to the chat box, they are most likely from the result of commands.
    /// 0: This is for public chat message.
    /// 1: This is for error messages.
    /// 2: This is for notifications.
    /// 3: This is for private messages. 
    /// </summary>
    public int ChatKind;
    
    public bool IsBot;
}