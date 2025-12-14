using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.Views.Servers.Settings;

public partial class AuditLogChangeItemModel : ObservableObject
{
    [ObservableProperty]
    private string? text;
}
