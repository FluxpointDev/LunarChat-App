using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Core.Servers;
using System.Collections.Concurrent;

namespace LunarChatApp.Shared.WebSocket;

public class SocketState
{
    public bool APIEnabled = true;
    public LunarSocketClient? WebSocket;
    public string? CurrentId;
    public SocketServerState? CurrentServer;
    public Channel? CurrentChannel;
    public ConcurrentDictionary<string, SocketServerState> Servers = new ConcurrentDictionary<string, SocketServerState>();
    public ConcurrentDictionary<string, List<Channel>> Channels = new ConcurrentDictionary<string, List<Channel>>();

    public Dictionary<string, Relation> Friends = new Dictionary<string, Relation>();
    public Dictionary<string, Relation> Blocks = new Dictionary<string, Relation>();

    public delegate void ServerEventHandler(Server server);

    public delegate void ChannelEventHandler(Channel channel, Relation user);
    public delegate void EventHandler();


    public event ServerEventHandler? OnAddServer;
    public event ServerEventHandler? OnRemoveServer;
    public event ServerEventHandler? OnSelectServer;
    public event ChannelEventHandler? OnSelectChannel;

    public Func<Relation, Task>? OnFriendAdd;
    public Func<Relation, Task>? OnFriendRemove;

    public Func<Relation, Task>? OnBlockAdd;
    public Func<Relation, Task>? OnBlockRemove;

    public Func<Message, Task>? OnMessageEdit;
    public Func<Message, Task>? OnMessageDelete;

    public void TriggerAddServer(Server server)
    {
        OnAddServer?.Invoke(server);
    }

    public void TriggerDeleteServer(Server server)
    {
        OnRemoveServer?.Invoke(server);
    }

    public void TriggerSelectServer(Server server)
    {
        OnSelectServer?.Invoke(server);
    }

    public void TriggerSelectChannel(Channel channel, Relation user)
    {
        OnSelectChannel?.Invoke(channel, user);
    }
}
public class SocketServerState
{
    public Func<Channel, Task> OnChannelCreate;
    public Func<Channel, Task> OnChannelDelete;
    public Func<Channel, Task> OnChannelUpdate;
    public Server Server;
    public ConcurrentDictionary<string, Channel> Channels = new ConcurrentDictionary<string, Channel>();
    public ConcurrentDictionary<string, List<Message>> Messages = new ConcurrentDictionary<string, List<Message>>();
}
