namespace LunarChatApp.Shared.Rest.Accounts;

public class ChangePasswordRequest : ILunarRequest
{
    public string password { get; set; }
}
