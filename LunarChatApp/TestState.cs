using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Core.Messages;
using LunarChatApp.Shared.Core.Servers;
using System.Collections.Generic;

namespace LunarChatApp;

public static class TestState
{
    public static List<Server> Servers = new List<Server>();
    public static Dictionary<string, List<Channel>> Channels = new Dictionary<string, List<Channel>>();
    public static Dictionary<string, List<Message>> Messages = new Dictionary<string, List<Message>>();
}
