using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.ViewModels.User;
using LunarChatApp.Views;
using LunarChatApp.Views.Main;
using LunarChatSharp.Core.Users;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using LunarChatSharp.Rest.Users;
using LunarChatSharp.Websocket.Events.Account;
using LunarChatSharp.Websocket.Events.Servers;
using Material.Icons;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels;

public partial class ServersModel : ViewModelBase
{
    public TestState state { get; set; }
    private MainModel main;
    private ServiceManager services;
    public ServersModel(ServiceManager sv, MainModel mainModel)
    {
        services = sv;
        state = services.State;
        main = mainModel;

        if (OperatingSystem.IsAndroid() || OperatingSystem.IsIOS())
            isExpanded = false;
        else
            isExpanded = true;

        services.State.OnExpandChannels += ExpandChannels;
        services.State.OnPageSelect += State_OnPageSelect;
        services.Client.OnSelectServer += OnSelectServer;
        services.Client.OnSelectChannel += OnSelectChannel;
        services.Client.OnAddServer += State_OnAddServer;
        services.Client.OnRemoveServer += State_OnRemoveServer;
        services.Client.OnPresenceUpdate += PresenceUpdate;
        services.Client.OnAccountUpdate += AccountUpdate;
        services.Client.OnServerUpdate += ServerUpdate;
        if (state.Socket.CurrentServer == null)
        {
            _selectedPage = new HomeView() { DataContext = new HomeModel(services) };
            //_selectedHeader = new HomeHeader() { DataContext = new HomeHeaderModel() };
            _selectedSidebar = new DMsListView { DataContext = new DMsListModel(services) };
        }
        else
        {
            _selectedHeader = new ServerHeaderView() { DataContext = new ServerHeaderModel(services, state.Socket.CurrentServer.Server) };
            _selectedSidebar = new ChannelsListView() { DataContext = new ChannelListModel(services, state) };
            if (state.Socket.CurrentChannel != null)
                _selectedPage = new ChannelView() { DataContext = new ChannelViewModel(state, services) };
        }

        if (ServersList == null)
        {
            ServersList = new ObservableCollection<ServerIcon>(state.Socket.Servers.Values.Select(x => new ServerIcon() { DataContext = new ServerIconModel(services, x.Server) }));
            addServerModel = new ServerIconModel(services, new RestServer
            {
                Id = "0",
                Name = "+",
                CreatedAt = DateTime.UtcNow,
                OwnerId = null!,
                SystemMessages = null!,
                DefaultPermissions = null!,
            });
            discoveryModel = new ServerIconModel(services, new RestServer
            {
                Id = "1",
                Name = "o",
                CreatedAt = DateTime.UtcNow,
                OwnerId = null!,
                SystemMessages = null!,
                DefaultPermissions = null!,
            });
        }
    }

    private async Task ExpandChannels(bool? value)
    {
        if (value.HasValue)
            IsExpanded = value.Value;
        else
            IsExpanded = !IsExpanded;
    }

    [ObservableProperty]
    private bool isExpanded = true;

    [ObservableProperty]
    private ServerIconModel addServerModel;

    [ObservableProperty]
    private ServerIconModel discoveryModel;

    private async Task ServerUpdate(RestServer server, ServerUpdateEvent ev)
    {
        if (string.IsNullOrEmpty(ev.Changed.Name))
            return;

        var item = ServersList.FirstOrDefault(x => (x.DataContext as ServerIconModel)?.Id == ev.ServerId);
        if (item != null)
        {
            ServerIconModel? model = item.DataContext as ServerIconModel;
            if (model == null)
                return;

            Avalonia.Threading.Dispatcher.UIThread.Post(() =>
            {
                if (server.Name != null)
                {
                    model.Name = server.Name;
                    model.Fallback = server.GetFallback();
                }
            });

        }
    }

    private async Task AccountUpdate(AccountUpdateEvent ev)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (ev.DisplayName != null)
            {
                state.DisplayName = ev.DisplayName;
                state.CurrentDisplayName = ev.DisplayName;
            }

            if (ev.Username != null)
                state.Username = ev.Username;

            if (string.IsNullOrEmpty(state.CurrentDisplayName))
                state.CurrentDisplayName = state.Username;
        });

    }

    private async Task PresenceUpdate(RestUserPresence presence)
    {

    }

    private async Task State_OnRemoveServer(RestServer server)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ServersList.Remove(ServersList.FirstOrDefault(x => (x.DataContext as ServerIconModel).Id == server.Id));
            if (server.Id == state.Socket.CurrentServer?.Server.Id)
            {
                SelectedHeader = null;
                SelectedSidebar = null;
                SelectedPage = null;
                state.Socket.CurrentServer = null;
            }
        });

    }

    private async Task State_OnAddServer(RestServer server)
    {
        var serverItem = ServersList.FirstOrDefault(x => (x.DataContext as ServerIconModel)!.Id == server.Id);
        if (serverItem != null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            ServersList.Add(new ServerIcon()
            {
                DataContext = new ServerIconModel(services, server)
            });
        });
    }

    private void State_OnPageSelect(UserControl control)
    {
        SelectedPage = control;
    }

    private async Task OnSelectChannel(RestChannel channel)
    {
        if (channel == null)
            SelectedPage = null;
        else
            SelectedPage = new ChannelView() { DataContext = new ChannelViewModel(state, services) };
    }

    private async Task OnSelectServer(RestServer? server)
    {
        if (!IsExpanded)
            IsExpanded = true;
        if (server == null)
        {
            state.Socket.CurrentServer = null;
            SelectedHeader = null;
            SelectedSidebar = null;
            SelectedPage = null;
        }
        else
        {
            SelectedHeader = new ServerHeaderView() { DataContext = new ServerHeaderModel(services, server) };
            SelectedSidebar = new ChannelsListView() { DataContext = new ChannelListModel(services, state) };
            SelectedPage = null;
        }
    }

    [ObservableProperty]
    private ObservableCollection<ServerIcon> _serversList;

    [ObservableProperty]
    private UserControl? _selectedHeader;

    [ObservableProperty]
    private UserControl? _selectedSidebar;

    [ObservableProperty]
    private UserControl? _selectedPage;

    [RelayCommand]
    public void OpenHome()
    {
        if (SelectedPage?.GetType() == typeof(HomeView))
            return;

        if (!IsExpanded)
            IsExpanded = true;

        SelectedHeader = null;
        //SelectedHeader = new HomeHeader() { DataContext = new HomeHeaderModel() };
        SelectedSidebar = new DMsListView { DataContext = new DMsListModel(services) };
        SelectedPage = new HomeView() { DataContext = new HomeModel(services) };
        state.Socket.CurrentServer = null;
    }

    [RelayCommand]
    public void CopyUserID()
    {
        services.CopyText(services.Client.CurrentId);
    }

    [RelayCommand]
    public void OpenSettings()
    {
        services.PageManager.OnSwitchPage(new SettingsPage
        {
            DataContext = new SettingsModel(services, main)
        });
    }

    [RelayCommand]
    public void OpenStatusDialog()
    {
        services.Dialogs.Create(new StatusDialog(), new StatusDialogModel(state), "Set Status").WithSubmit(SubmitStatus).Open();
    }

    public void SubmitStatus(UserControl control)
    {
        StatusDialogModel? model = control.DataContext as StatusDialogModel;
        state.StatusText = model.StatusText;
        state.StatusType = model.StatusType;

        switch (state.StatusType)
        {
            case UserStatusType.Online:
                StatusIcon = MaterialIconKind.Circle;
                StatusColor = "#FF00C853";
                break;
            case UserStatusType.Idle:
                StatusIcon = MaterialIconKind.MoonLastQuarter;
                StatusColor = "#FFFFD600";
                break;
            case UserStatusType.Focus:
                StatusIcon = MaterialIconKind.Adjust;
                StatusColor = "#FF2979FF";
                break;
            case UserStatusType.DoNotDisturb:
                StatusIcon = MaterialIconKind.DoNotDisturbOn;
                StatusColor = "#FFFF1744";
                break;
            case UserStatusType.Invisible:
            case UserStatusType.Offline:
                StatusIcon = MaterialIconKind.CircleOutline;
                StatusColor = "#80E5E5E5";
                break;
        }
    }

    [ObservableProperty]
    private MaterialIconKind _statusIcon = MaterialIconKind.Circle;

    [ObservableProperty]
    private string _statusColor = "#FF00C853";

    [RelayCommand]
    public void Logout()
    {
        services.Socket.StopWebSocket = true;
        services.Client.CurrentId = null;
        services.Client.Token = null;
        services.Rest.Http.DefaultRequestHeaders.Remove("Auth-Id");
        services.PageManager.OnSwitchPage(new LoginPage
        {
            DataContext = new LoginModel(services, main)
        });
    }

    [RelayCommand]
    public void Quit()
    {
        Environment.Exit(0);
    }
}
