namespace LunarChatApp.Shared.Rest.Apps;

public class CreateTeamRequest : ILunarRequest
{
    public required string name { get; set; }
}
