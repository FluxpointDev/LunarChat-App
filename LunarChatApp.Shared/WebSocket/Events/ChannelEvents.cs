
using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp.Shared.WebSocket.Events;

public class ChannelCreatedEvent : SocketMessage
{
    public ChannelCreatedEvent() : base("channel_create") { }
    public Channel channel;
}
public class ChannelDeletedEvent : SocketMessage
{
    public ChannelDeletedEvent() : base("channel_delete") { }
    public string server_id;
    public string channel_id;
}

public class ChannelUpdatedEvent : SocketMessage
{
    public ChannelUpdatedEvent() : base("channel_update") { }
    public string channel_id;

}