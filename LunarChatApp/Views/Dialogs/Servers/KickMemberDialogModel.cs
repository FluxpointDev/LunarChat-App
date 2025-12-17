using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.Views.Dialogs.Servers;

public partial class KickMemberDialogModel : ViewModelBase
{
    [ObservableProperty]
    private string? reason;
}
