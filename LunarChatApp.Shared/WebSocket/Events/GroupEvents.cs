using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp.Shared.WebSocket.Events;

public class GroupJoinEvent : SocketMessage
{
    public GroupJoinEvent() : base("group_join") { }
    public GroupChannel group;
}
public class GroupLeaveEvent : SocketMessage
{
    public GroupLeaveEvent() : base("group_leave") { }
    public string group_id;
}