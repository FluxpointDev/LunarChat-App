using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers.Settings;

namespace LunarChatApp.ViewModels.Servers;

public partial class ServerSettingsModel : ViewModelBase
{
    private PageManager pageManager;
    private TestState state { get; set; }

    public ServerSettingsModel(PageManager page, TestState st)
    {
        pageManager = page;
        state = st;
        ServerName = st.CurrentServer.Server.Name;
        if (SelectedPage == null)
            SelectedPage = new ServerSettingsOverview() { DataContext = new ServerSettingsOverviewModel(state) };
    }

    [ObservableProperty]
    private string? _serverName;

    [ObservableProperty]
    private UserControl? _selectedPage;

    [ObservableProperty]
    public string? _selectedTitle = "Overview";

    [RelayCommand]
    public void CloseSettings()
    {
        pageManager.OnSwitchPage(state.CachedServersPage);
    }

    [RelayCommand]
    public void OpenTestSettings()
    {
        SelectedPage = new ServerSettingsOverview() { DataContext = new ServerSettingsOverviewModel(state) };
    }
}
