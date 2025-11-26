namespace LunarChatApp.Shared.Core.Channels;

public class VoiceChannel : Channel
{
    public static VoiceChannel Create(ChannelModel model)
    {
        return new VoiceChannel();
    }
}
