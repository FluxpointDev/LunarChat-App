using LunarChatApp.Shared.Rest.Optional;

namespace LunarChatApp.Shared.Rest.Servers;

public class UpdateRoleRequest : ILunarRequest
{
    public Optional<string> name { get; set; }
}
