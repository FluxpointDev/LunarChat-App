namespace LunarChatApp.Shared.Rest.Messages;

public class EditMessageRequest : ILunarRequest
{
    public string content { get; set; }
}
