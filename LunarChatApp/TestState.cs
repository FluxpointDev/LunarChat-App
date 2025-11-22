using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Core.Servers;
using LunarChatApp.Shared.WebSocket;
using LunarChatApp.ViewModels.Dialogs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LunarChatApp;

public partial class TestState : ObservableObject
{
    public StatusType StatusType;
    public string? StatusText;
    public ServersPage? CachedServersPage;
    public LunarSocketClient? WebSocket;
    public string? CurrentId;

    [ObservableProperty]
    private string _username = "test";

    [ObservableProperty]
    public string _displayName = "Test";

    public ServerState? CurrentServer;
    public Channel? CurrentChannel;
    public Dictionary<string, ServerState> Servers = new Dictionary<string, ServerState>
    {
        { "1", new ServerState
            {
                Server = new Server
                {
                    Id = "1",
                    Name = "Fluxpoint Development"
                },
                Channels = new Dictionary<string, Channel>
                {
                    { "1", new Channel
                        {
                            Id = "1",
                            Name = "rules",
                            Type = ChannelType.Rules
                        }
                    },
                    { "2", new Channel
                        {
                            Id = "2",
                            Name = "schedule",
                            Type = ChannelType.Schedule
                        }
                    },
                    { "3", new Channel
                        {
                            Id = "3",
                            Name = "general",
                            Type = ChannelType.Text
                        }
                    },
                    { "4", new Channel
                        {
                            Id = "4",
                            Name = "media",
                            Type = ChannelType.Media
                        }
                    },
                    { "5", new Channel
                        {
                            Id = "5",
                            Name = "voice",
                            Type = ChannelType.Voice
                        }
                    },
                },
                Messages = new Dictionary<string, List<Message>>
                {
                    { "1", new List<Message>() },
                    { "2", new List<Message>() },
                    { "3", new List<Message>() },
                    { "4", new List<Message>() },
                    { "5", new List<Message>() }
                }
            }
        }
    };

    public Dictionary<string, List<Message>> PrivateMessages = new Dictionary<string, List<Message>>();

    public delegate void PageEventHandler(UserControl control);
    public delegate void ServerEventHandler(Server server);

    public delegate void ChannelEventHandler(Channel channel, Shared.Core.Users.User user);
    public delegate void EventHandler();

    public event PageEventHandler? OnPageSelect;
    public event ServerEventHandler? OnSelectServer;
    public event ChannelEventHandler? OnSelectChannel;

    //public event EventHandler? OnServersUpdate;

    public void TriggerPageSelect(UserControl control)
    {
        OnPageSelect?.Invoke(control);
    }
    public void TriggerSelectServer(Server server)
    {
        OnSelectServer?.Invoke(server);
    }

    public void TriggerSelectChannel(Channel channel, Shared.Core.Users.User user)
    {
        OnSelectChannel?.Invoke(channel, user);
    }
}
public class ServerState
{
    public Func<Channel, Task> OnChannelUpdated;
    public Server Server;
    public Dictionary<string, Channel> Channels = new Dictionary<string, Channel>();
    public Dictionary<string, List<Message>> Messages = new Dictionary<string, List<Message>>();
}