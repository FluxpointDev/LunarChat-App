namespace LunarChatApp.Shared.WebSocket.Events;

public class ServerMemberJoinedEvent : SocketMessage
{
    public ServerMemberJoinedEvent() : base("server_member_join")
    {

    }
    public string server_id;
}
public class ServerMemberLeftEvent : SocketMessage
{
    public ServerMemberLeftEvent() : base("server_member_left")
    {

    }
    public string server_id;
}
public class ServerMemberUpdateEvent : SocketMessage
{
    public ServerMemberUpdateEvent() : base("server_member_update")
    {

    }
    public string server_id;
}
public class ServerMemberBannedEvent : SocketMessage
{
    public ServerMemberBannedEvent() : base("server_member_ban")
    {

    }
    public string server_id;
}
public class ServerMemberKickEvent : SocketMessage
{
    public ServerMemberKickEvent() : base("server_member_kick")
    {

    }
    public string server_id;
}
public class ServerMemberTimeoutEvent : SocketMessage
{
    public ServerMemberTimeoutEvent() : base("server_member_timeout")
    {

    }
    public string server_id;
}


