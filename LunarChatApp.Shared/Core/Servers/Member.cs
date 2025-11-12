using LunarChatServer.Shared.Rest;

namespace LunarChatApp.Shared.Core.Servers;

public class Member
{
    public static Member Create(MemberModel model)
    {
        return new Member();
    }
}
