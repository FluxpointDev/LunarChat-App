using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LunarChatApp.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private UserControl? _selectedPage = new SettingsProfile();

    [ObservableProperty]
    public string? _selectedTitle = "Profile";
}
