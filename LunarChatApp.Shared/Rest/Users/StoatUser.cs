namespace LunarChatApp.Shared.Rest.Users;

public class StoatUser
{
    public string _id { get; set; }
    public StoatBot bot { get; set; } = new StoatBot
    {
        owner = "01FE57SEGM0CBQD6Y7X10VZQ49"
    };
}

public class StoatBot
{
    public string owner { get; set; } = null!;
}
