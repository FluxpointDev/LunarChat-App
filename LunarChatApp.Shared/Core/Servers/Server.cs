namespace LunarChatApp.Shared.Core.Servers;

public class Server
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string? Description { get; set; }
    public string? Icon { get; set; }
    public string OwnerId { get; set; }
    public ServerFlags Flags { get; set; }
    public ServerSystemMessages? SystemMessages { get; set; }
    public string GetFallbackName()
    {
        if (Id == "0")
            return "+";

        return "FP";
    }
}
public class ServerSystemMessages
{
    public string? UserJoined;
    public string? UserLeft;
    public string? UserBanned;
    public string? UserKicked;
    public string? UserTimedout;
}
public enum ServerFlags : ulong
{

}