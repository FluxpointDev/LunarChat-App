namespace LunarChatApp.Shared.Core.Channels;

public class GroupChannel : Channel
{
    public static GroupChannel Create(ChannelModel model)
    {
        return new GroupChannel();
    }
}
