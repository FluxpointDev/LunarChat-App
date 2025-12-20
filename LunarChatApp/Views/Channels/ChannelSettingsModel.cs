using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Channels.Settings;
using LunarChatApp.Views;
using LunarChatApp.Views.Channels.Settings;
using LunarChatApp.Views.Servers.Settings;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Rest.Webhooks;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelSettingsModel : ViewModelBase
{
    private ServiceManager services;
    private PageManager pageManager;
    private TestState state { get; set; }
    private RestChannel channel;
    public ChannelSettingsModel(ServiceManager sv, RestChannel chan)
    {
        services = sv;
        pageManager = sv.PageManager;
        state = sv.State;
        channel = chan;
        ChannelName = chan.Name;
        if (chan.Type == LunarChatSharp.Core.Channels.ChannelType.Group)
        {
            channelType = "Group Channel";
        }
        else
        {
            channelType = "Server Channel";
            isServerChannel = true;
            sv.Client.OnChannelUpdate += UpdateChannel;
            if (sv.State.CurrentServer != null)
                sv.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        }

        if (SelectedPage == null)
            SelectedPage = new ChannelSettingsOverview { DataContext = new ChannelSettingsOverviewModel(services, chan) };
    }

    private async Task PermissionUpdate(RestServer server)
    {
        bool HasManage = services.State.CurrentServer.CanManageChannel(services.State.CurrentServer.Member);
        if (HasManage)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            pageManager.OnSwitchPage(state.CachedServersPage);
        });
    }

    private async Task UpdateChannel(RestChannel channel, UpdateChannelRequest request)
    {
        if (state.CurrentChannel?.Id != channel.Id)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ChannelName = channel.Name;
            if (SelectedPage is ChannelSettingsOverview overview)
            {
                ChannelSettingsOverviewModel model = (overview.DataContext as ChannelSettingsOverviewModel);

                if (channel.Name != null)
                    model.ChannelNameEdit = channel.Name;

                if (channel.Topic != null)
                    model.ChannelTopicEdit = channel.Topic;
            }
        });
    }

    [ObservableProperty]
    private string channelType;

    [ObservableProperty]
    private bool isServerChannel;

    [ObservableProperty]
    private UserControl? _selectedPage;

    [ObservableProperty]
    private string? _channelName;

    [ObservableProperty]
    public string? _selectedTitle = "Overview";

    [RelayCommand]
    public void CloseSettings()
    {
        pageManager.OnSwitchPage(state.CachedServersPage);
    }

    [RelayCommand]
    public void OpenOverviewSettings()
    {
        SelectedTitle = "Overview";
        SelectedPage = new ChannelSettingsOverview { DataContext = new ChannelSettingsOverviewModel(services, channel) };
    }

    [RelayCommand]
    public void OpenWebhooksSettings()
    {
        SelectedTitle = "Webhooks";
        SelectedPage = new ChannelSettingsWebhooks { DataContext = new ChannelSettingsWebhooksModel(services, OpenWebhooksSettings, OpenWebhookInfo) };
    }

    [RelayCommand]
    public void OpenGroupUsers()
    {
        SelectedTitle = "Users";
        SelectedPage = new ChannelSettingsGroupUsers { DataContext = new ChannelSettingsGroupUsersModel(services) };
    }

    [RelayCommand]
    public void OpenApps()
    {
        SelectedTitle = "Apps";
        SelectedPage = new ServerSettingsApps() { DataContext = new ServerSettingsAppsModel(services, true) };
    }

    public void OpenWebhookInfo(RestWebhook webhook)
    {
        SelectedTitle = "Edit Webook - " + webhook.Name;
        //SelectedPage = new ServerSettingsRoleInfo { DataContext = new ServerSettingsRoleInfoModel(services, role, OpenRolesSettings) };
    }
}
