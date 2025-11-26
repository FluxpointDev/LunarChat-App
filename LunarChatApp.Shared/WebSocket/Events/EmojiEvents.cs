using LunarChatApp.Shared.Core.Messages;

namespace LunarChatApp.Shared.WebSocket.Events;

public class EmojiCreateEvent : SocketMessage
{
    public EmojiCreateEvent() : base("emoji_create")
    {

    }
    public Emoji emoji;
}
public class EmojiDeleteEvent : SocketMessage
{
    public EmojiDeleteEvent() : base("emoji_delete")
    {

    }
    public string emoji_id;
}
public class EmojiUpdateEvent : SocketMessage
{
    public EmojiUpdateEvent() : base("emoji_update")
    {

    }
    public string emoji_id;
    public string name;
}