using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers.Settings;
using LunarChatApp.Views;
using LunarChatApp.Views.Servers.Settings;

namespace LunarChatApp.ViewModels.Servers;

public partial class ServerSettingsModel : ViewModelBase
{
    private PageManager pageManager;
    private TestState state { get; set; }
    private ServiceManager services;
    public ServerSettingsModel(PageManager page, TestState st, ServiceManager sv)
    {
        services = sv;
        pageManager = page;
        state = st;
        ServerName = st.Socket.CurrentServer.Server.Name;
        if (SelectedPage == null)
            SelectedPage = new ServerSettingsOverview() { DataContext = new ServerSettingsOverviewModel(services) };
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
        SelectedTitle = "Overview";
        SelectedPage = new ServerSettingsOverview() { DataContext = new ServerSettingsOverviewModel(services) };
    }

    [RelayCommand]
    public void OpenSystemSettings()
    {
        SelectedTitle = "System";
        SelectedPage = new ServerSettingsSystem() { DataContext = new ServerSettingsSystemModel(services) };
    }

    [RelayCommand]
    public void OpenMembersSettings()
    {
        SelectedTitle = "Members";
        SelectedPage = new ServerSettingsMembers() { DataContext = new ServerSettingsMembersModel() };
    }

    [RelayCommand]
    public void OpenRolesSettings()
    {
        SelectedTitle = "Roles";
        SelectedPage = new ServerSettingsRoles() { DataContext = new ServerSettingsRolesModel() };
    }

    [RelayCommand]
    public void OpenEmotesSettings()
    {
        SelectedTitle = "Emotes";
        SelectedPage = new ServerSettingsEmojis() { DataContext = new ServerSettingsEmojisModel() };
    }

    [RelayCommand]
    public void OpenInvitesSettings()
    {
        SelectedTitle = "Invites";
        SelectedPage = new ServerSettingsInvites() { DataContext = new ServerSettingsInvitesModel() };
    }

    [RelayCommand]
    public void OpenBansSettings()
    {
        SelectedTitle = "Bans";
        SelectedPage = new ServerSettingsBans() { DataContext = new ServerSettingsBansModel() };
    }
}
