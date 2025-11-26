using LunarChatApp.Shared.Rest.Optional;

namespace LunarChatApp.Shared.Rest.Servers;

public class EditMemberRequest : ILunarRequest
{
    public Optional<bool> voice_deafen { get; set; }
    public Optional<bool> voice_mute { get; set; }
}
