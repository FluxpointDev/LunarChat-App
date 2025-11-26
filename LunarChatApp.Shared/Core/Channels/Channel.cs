namespace LunarChatApp.Shared.Core.Channels;

public class Channel
{
    public string Id;
    public string Name;
    public string? Topic;
    public ChannelType Type;
    public string? ServerId;
    public static Channel Create(ChannelModel model)
    {
        return new Channel();
    }
}
