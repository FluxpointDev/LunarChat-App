namespace LunarChatApp.Shared.Core.Channels;

public class Channel
{
    public string Id;
    public string Name;
    public ChannelType Type;
    public static Channel Create(ChannelModel model)
    {
        return new Channel();
    }
}
