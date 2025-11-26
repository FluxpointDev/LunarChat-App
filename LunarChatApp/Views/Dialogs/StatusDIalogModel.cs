using CommunityToolkit.Mvvm.ComponentModel;
using LunarChatApp.Views;

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
    private StatusType _statusType;
}
public enum StatusType
{
    Online, Idle, Busy, Invisible
}