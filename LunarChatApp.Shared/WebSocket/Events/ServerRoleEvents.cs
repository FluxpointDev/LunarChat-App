namespace LunarChatApp.Shared.WebSocket.Events;

public class ServerRoleCreateEvent : SocketMessage
{
    public ServerRoleCreateEvent() : base("server_role_create")
    {

    }
    public string server_id;
}
public class ServerRoleDeleteEvent : SocketMessage
{
    public ServerRoleDeleteEvent() : base("server_role_delete")
    {

    }
    public string server_id;
}
public class ServerRoleUpdateEvent : SocketMessage
{
    public ServerRoleUpdateEvent() : base("server_role_update")
    {

    }
    public string server_id;
}