using LunarChatApp.Shared.Core;

namespace LunarChatApp.Shared.Core.Users;

public class UserModel
{
    public string? id { get; set; }
    public string? username { get; set; }
    public string? display_name { get; set; }
    public AttachmentModel? avatar { get; set; }
    public ulong badges { get; set; }
}
