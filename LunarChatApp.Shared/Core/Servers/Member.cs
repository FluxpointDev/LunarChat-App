namespace LunarChatApp.Shared.Core.Servers;

public class Member
{
    public string Id { get; set; }
    public static Member Create(MemberModel model)
    {
        return new Member();
    }
}
