using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Views;
using LunarChatSharp.Core.Users;

namespace LunarChatApp.ViewModels.Dialogs;

public partial class StatusDialogModel : ViewModelBase
{
    public StatusDialogModel(TestState state)
    {
        StatusText = state.StatusText;
        StatusType = state.StatusType;
    }
    [ObservableProperty]
    private string? _statusText;

    [ObservableProperty]
    private UserStatusType _statusType;
}