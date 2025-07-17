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

    public int ChatKind;
    
    public bool IsBot;
}