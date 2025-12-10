using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Servers.Settings;
using LunarChatApp.Views;
using LunarChatApp.Views.Servers.Settings;
using LunarChatSharp.Rest.Roles;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Websocket.Events.Servers;
using System.Threading.Tasks;

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
        services.Client.OnServerUpdate += ServerUpdate;
        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        if (SelectedPage == null)
            SelectedPage = new ServerSettingsOverview() { DataContext = new ServerSettingsOverviewModel(services) };
    }

    private async Task PermissionUpdate()
    {
        bool CanView = services.State.Socket.CurrentServer.CanManageServer(services.State.Socket.CurrentServer.Member);
        if (!CanView)
            pageManager.OnSwitchPage(state.CachedServersPage);
    }

    private async Task ServerUpdate(RestServer server, ServerUpdateEvent ev)
    {
        if (string.IsNullOrEmpty(ev.Name) || server.Id != services.Socket.State.CurrentServer.Server.Id)
            return;

        ServerName = ev.Name;
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
        SelectedPage = new ServerSettingsMembers() { DataContext = new ServerSettingsMembersModel(services) };
    }

    [RelayCommand]
    public void OpenRolesSettings()
    {
        SelectedTitle = "Roles";
        SelectedPage = new ServerSettingsRoles() { DataContext = new ServerSettingsRolesModel(services, OpenRolesSettings, OpenRoleInfo) };
    }

    public void OpenRoleInfo(RestRole role)
    {
        SelectedTitle = "Edit Role - " + role.Name;
        SelectedPage = new ServerSettingsRoleInfo { DataContext = new ServerSettingsRoleInfoModel(services, role, OpenRolesSettings) };
    }

    [RelayCommand]
    public void OpenEmotesSettings()
    {
        SelectedTitle = "Emotes";
        SelectedPage = new ServerSettingsEmojis() { DataContext = new ServerSettingsEmojisModel(services) };
    }

    [RelayCommand]
    public void OpenInvitesSettings()
    {
        SelectedTitle = "Invites";
        SelectedPage = new ServerSettingsInvites() { DataContext = new ServerSettingsInvitesModel(services) };
    }

    [RelayCommand]
    public void OpenBansSettings()
    {
        SelectedTitle = "Bans";
        SelectedPage = new ServerSettingsBans() { DataContext = new ServerSettingsBansModel(services) };
    }

    [RelayCommand]
    public void OpenAppsSettings()
    {
        SelectedTitle = "Apps";
        SelectedPage = new ServerSettingsApps() { DataContext = new ServerSettingsAppsModel(services) };
    }

    [RelayCommand]
    public void OpenDiscoverySettings()
    {
        SelectedTitle = "Discovery";
        SelectedPage = new ServerSettingsDiscovery() { DataContext = new ServerSettingsDiscoveryModel(services) };
    }
}
