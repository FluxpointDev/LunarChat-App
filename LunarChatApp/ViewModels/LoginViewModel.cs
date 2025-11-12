using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.ViewModels;

public partial class LoginViewModel : ViewModelBase
{
    [ObservableProperty]
    private string? _email;

    [ObservableProperty]
    private string? _password;

    [ObservableProperty]
    private string? _username;
}
