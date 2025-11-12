namespace LunarChatApp.Shared.Core.Channels;

public class ChannelModel
{
    public string id { get; set; }
    public string name { get; set; }
    public ChannelType type { get; set; }
    public DateTime created_at { get; set; }
}
