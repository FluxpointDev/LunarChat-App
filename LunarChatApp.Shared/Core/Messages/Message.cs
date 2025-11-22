namespace LunarChatApp.Shared.Core.Messages;

public class Message
{
    public string ChannelId { get; set; }
    public string Username { get; set; }
    public string Content { get; set; }
    public static Message Create(MessageModel model)
    {
        return new Message();
    }
}
