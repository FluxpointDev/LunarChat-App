namespace LunarChatApp.Shared.WebSocket.Events;

public class ReactionAddedEvent : SocketMessage
{
    public ReactionAddedEvent() : base("reaction_add") { }
    public string message_id;
    public string user_id;
    public string channel_id;
    public string emoji_id;
}

public class ReactionRemovedEvent : SocketMessage
{
    public ReactionRemovedEvent() : base("reaction_remove") { }
    public string message_id;
    public string user_id;
    public string channel_id;
    public string emoji_id;
}
