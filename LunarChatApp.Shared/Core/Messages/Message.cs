namespace LunarChatApp.Shared.Core.Messages;

public class Message
{
    public string Content;
    public static Message Create(MessageModel model)
    {
        return new Message();
    }
}
