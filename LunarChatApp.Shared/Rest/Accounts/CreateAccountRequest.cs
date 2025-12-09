using System.Text.Json.Serialization;

namespace LunarChatApp.Shared.Rest.Accounts;

public class CreateAccountRequest : ILunarRequest
{
    [JsonPropertyName("username")]
    public string username { get; set; }
    public string email { get; set; }
    public string password { get; set; }
}
