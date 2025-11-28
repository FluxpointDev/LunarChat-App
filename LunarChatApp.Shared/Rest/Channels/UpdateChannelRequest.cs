namespace LunarChatApp.Shared.Rest.Channels;

public class UpdateChannelRequest : ILunarRequest
{
    public string server_id { get; set; }
    public string name { get; set; }
    public string topic { get; set; }
}
