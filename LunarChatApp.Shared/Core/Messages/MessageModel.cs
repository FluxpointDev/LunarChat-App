using LunarChatApp.Shared.Core.Users;

namespace LunarChatApp.Shared.Core.Messages;

public class MessageModel
{
    public string id { get; set; }
    public string channel_id { get; set; }
    public string content { get; set; }
    public DateTime created_at { get; set; }
    public DateTime updated_at { get; set; }
    public UserModel author { get; set; }
    public bool is_pinned { get; set; }
    public string[] replies { get; set; }
}
