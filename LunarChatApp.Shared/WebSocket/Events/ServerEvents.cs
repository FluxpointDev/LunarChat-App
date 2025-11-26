using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Servers;

namespace LunarChatApp.Shared.WebSocket.Events;

public class ServerJoinEvent : SocketMessage
{
    public ServerJoinEvent() : base("server_join")
    {

    }
    public Server server;
    public Member member;
    public Dictionary<string, Channel> channels = new Dictionary<string, Channel>();
}
public class ServerLeftEvent : SocketMessage
{
    public ServerLeftEvent() : base("server_left")
    {

    }
    public string server_id;
}
public class ServerUpdateEvent : SocketMessage
{
    public ServerUpdateEvent() : base("server_update")
    {

    }
    public string server_id;
    public string? name;
    public string? description;
}