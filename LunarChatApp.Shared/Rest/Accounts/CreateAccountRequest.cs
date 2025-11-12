namespace LunarChatApp.Shared.Rest.Accounts;

public class CreateAccountRequest : ILunarRequest
{
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}
