using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Core.Servers;
using System.Collections.Concurrent;

namespace LunarChatApp.Shared.WebSocket.Events;

public class AuthEvent : SocketMessage
{
    public AuthEvent() : base("auth") { }
    public string user_id { get; set; }
}
public class ReadyEvent : SocketMessage
{
    public ReadyEvent() : base("ready") { }

    public Server[] servers;
    public ConcurrentDictionary<string, List<Channel>> channels;
    public Emoji[] emojis;
    public Dictionary<string, Relation> Friends;
    public Dictionary<string, Relation> Blocks;
}
