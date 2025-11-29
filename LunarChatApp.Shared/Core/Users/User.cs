namespace LunarChatApp.Shared.Core.Users;

public class User
{
    public string Id { get; set; }
    public string Username { get; set; }
    public string DisplayName { get; set; }
    public bool IsBot { get; set; }
    public UserPublicFlags PublicFlags { get; set; }
}
public enum UserPublicFlags : ulong
{

}