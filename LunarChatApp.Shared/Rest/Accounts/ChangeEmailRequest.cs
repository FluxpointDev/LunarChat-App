namespace LunarChatApp.Shared.Rest.Accounts;

public class ChangeEmailRequest : ILunarRequest
{
    public string email { get; set; }
    public string token { get; set; }
}
