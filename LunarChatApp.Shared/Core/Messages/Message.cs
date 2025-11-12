namespace LunarChatApp.Shared.Core.Messages;

public class Message
{
    public static Message Create(MessageModel model)
    {
        return new Message();
    }
}
