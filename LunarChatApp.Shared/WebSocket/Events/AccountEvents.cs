using LunarChatApp.Shared.Core.Accounts;

namespace LunarChatApp.Shared.WebSocket.Events;

public class AccountFriendAdd : SocketMessage
{
    public AccountFriendAdd() : base("account_friend_add") { }
    public Relation relation;
}
public class AccountFriendRemove : SocketMessage
{
    public AccountFriendRemove() : base("account_friend_remove") { }
    public string user_id;
}
public class AccountBlockAdd : SocketMessage
{
    public AccountBlockAdd() : base("account_block_add") { }
    public Relation relation;
}
public class AccountBlockRemove : SocketMessage
{
    public AccountBlockRemove() : base("account_block_remove") { }
    public string user_id;
}