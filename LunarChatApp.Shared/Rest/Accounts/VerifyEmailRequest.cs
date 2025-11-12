namespace LunarChatApp.Shared.Rest.Accounts;

public class VerifyEmailRequest : ILunarRequest
{
    public string email { get; set; }
}
