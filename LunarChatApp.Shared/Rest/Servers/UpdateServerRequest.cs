using LunarChatApp.Shared.Rest.Optional;

namespace LunarChatApp.Shared.Rest.Servers;

public class UpdateServerRequest : ILunarRequest
{
    public Optional<string?> name { get; set; }
    public Optional<string?> description { get; set; }
    public Optional<UpdateServerSystemMessages> system_messages { get; set; }
}
public class UpdateServerSystemMessages
{
    public Optional<string?> user_joined { get; set; }
    public Optional<string?> user_left { get; set; }
    public Optional<string?> user_banned { get; set; }
    public Optional<string?> user_kicked { get; set; }
    public Optional<string?> user_timedout { get; set; }
}