namespace LunarChatApp.Shared.Core.Channels;

public class TextChannel : Channel
{
    public static TextChannel Create(ChannelModel model)
    {
        return new TextChannel();
    }
}
