using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp.Shared.Rest.Channels;

public class CreateChannelRequest : ILunarRequest
{
    public string name { get; set; }
    public string topic { get; set; }
    public string serverId { get; set; }
    public ChannelType type { get; set; }
}
