using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;

namespace LunarChatApp.ViewModels.Dialogs;

public partial class JoinServerDialogModel(ServiceManager services) : ViewModelBase
{
    [ObservableProperty]
    private bool _showOptions = true;

    [ObservableProperty]
    private bool _showJoin;

    [ObservableProperty]
    private bool _showCreate;

    [ObservableProperty]
    private string? _textbox;

    [RelayCommand]
    public void ShowJoinServer()
    {
        ShowOptions = false;
        ShowJoin = true;
    }

    [RelayCommand]
    public void ShowCreateServer()
    {
        ShowOptions = false;
        ShowCreate = true;
    }
}
