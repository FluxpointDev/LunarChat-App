namespace LunarChatApp.Shared.Core.Messages;

public class Message
{
    public string Id { get; set; }
    public string ChannelId { get; set; }
    public string AuthorId { get; set; }
    public string Username { get; set; }
    public string Content { get; set; }
    public MessageFlags Flags { get; set; }
    public static Message Create(MessageModel model)
    {
        return new Message();
    }
}
public enum MessageFlags : ulong
{

}
