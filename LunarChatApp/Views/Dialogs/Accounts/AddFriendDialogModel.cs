using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.Views.Dialogs;

public partial class AddFriendDialogModel : ViewModelBase
{
    [ObservableProperty]
    private string? _username;
}
