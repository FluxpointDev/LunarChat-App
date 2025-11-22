namespace LunarChatApp.Shared.Rest.Messages;

public class SendMessageRequest : ILunarRequest
{
    public string content { get; set; }
}
