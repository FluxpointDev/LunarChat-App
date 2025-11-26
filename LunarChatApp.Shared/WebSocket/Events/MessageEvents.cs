using LunarChatApp.Shared.Core.Users;

namespace LunarChatApp.Shared.WebSocket.Events;

public class MessageRecievedEvent : SocketMessage
{
    public MessageRecievedEvent() : base("message_create") { }

    public string id;
    public string content;
    public User user;
    public string channel_id;
}
public class MessageDeleteEvent : SocketMessage
{
    public MessageDeleteEvent() : base("message_delete") { }

    public string id;
    public string content;
    public User user;
    public string channel_id;
}

public class MessageUpdateEvent : SocketMessage
{
    public MessageUpdateEvent() : base("message_update") { }

    public string id;
    public string content;
    public User user;
    public string channel_id;
}