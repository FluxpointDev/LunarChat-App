using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Accounts;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.ViewModels.User;
using LunarChatApp.Views;
using LunarChatApp.Views.Main;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.ViewModels;

public partial class ServersViewModel : ViewModelBase
{
    public TestState state { get; set; }
    private MainViewModel main;
    private ServiceManager services;
    public ServersViewModel(ServiceManager sv, MainViewModel mainModel)
    {
        services = sv;
        state = services.State;
        main = mainModel;
        services.State.OnPageSelect += State_OnPageSelect;
        services.State.Socket.OnSelectServer += OnSelectServer;
        services.State.Socket.OnSelectChannel += OnSelectChannel;
        services.State.Socket.OnAddServer += State_OnAddServer;
        services.State.Socket.OnRemoveServer += State_OnRemoveServer;
        if (state.Socket.CurrentServer == null)
        {
            _selectedPage = new HomeView() { DataContext = new HomeViewModel(services) };
            //_selectedHeader = new HomeHeader() { DataContext = new HomeHeaderModel() };
            _selectedSidebar = new DMsListView { DataContext = new DMsListModel(services) };
        }
        else
        {
            _selectedHeader = new ServerHeaderView() { DataContext = new ServerHeaderViewModel(services, state.Socket.CurrentServer.Server) };
            _selectedSidebar = new ChannelsListView() { DataContext = new ChannelListViewModel(services, state) };
            if (state.Socket.CurrentChannel != null)
                _selectedPage = new ChannelView() { DataContext = new ChannelViewModel(state, services, null) };
        }

        if (ServersList == null)
        {
            ServersList = new ObservableCollection<ServerIcon>(state.Socket.Servers.Values.Select(x => new ServerIcon() { DataContext = new ServerIconViewModel(services, x.Server) }));
            ServersList.Add(new ServerIcon()
            {
                DataContext = new ServerIconViewModel(services, new Shared.Core.Servers.Server
                {
                    Id = "0",
                    Name = "+"
                })
            });
        }
    }

    private void State_OnRemoveServer(Shared.Core.Servers.Server server)
    {
        ServersList.Remove(ServersList.FirstOrDefault(x => (x.DataContext as ServerIconViewModel).Id == server.Id));
        if (server.Id == state.Socket.CurrentServer?.Server.Id)
        {
            SelectedHeader = null;
            SelectedSidebar = null;
            SelectedPage = null;
            state.Socket.CurrentServer = null;
        }
    }

    private void State_OnAddServer(Shared.Core.Servers.Server server)
    {
        var serverItem = ServersList.FirstOrDefault(x => (x.DataContext as ServerIconViewModel)!.Id == server.Id);
        if (serverItem != null)
            return;

        ServersList.Add(new ServerIcon()
        {
            DataContext = new ServerIconViewModel(services, new Shared.Core.Servers.Server
            {
                Id = server.Id,
                Name = server.Name
            })
        });
    }

    private void State_OnPageSelect(UserControl control)
    {
        SelectedPage = control;
    }

    private void OnSelectChannel(Shared.Core.Channels.Channel channel, Relation user)
    {
        SelectedPage = new ChannelView() { DataContext = new ChannelViewModel(state, services, user) };
    }

    private void OnSelectServer(Shared.Core.Servers.Server server)
    {
        SelectedHeader = new ServerHeaderView() { DataContext = new ServerHeaderViewModel(services, server) };
        SelectedSidebar = new ChannelsListView() { DataContext = new ChannelListViewModel(services, state) };
        SelectedPage = null;
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

        SelectedHeader = null;
        //SelectedHeader = new HomeHeader() { DataContext = new HomeHeaderModel() };
        SelectedSidebar = new DMsListView { DataContext = new DMsListModel(services) };
        SelectedPage = new HomeView() { DataContext = new HomeViewModel(services) };
        state.Socket.CurrentServer = null;
    }

    [RelayCommand]
    public void OpenSettings()
    {
        services.PageManager.OnSwitchPage(new SettingsPage
        {
            DataContext = new SettingsViewModel(services.PageManager, state, services.ThemeWatcher, main)
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
    }

    [RelayCommand]
    public void Logout()
    {
        state.Socket.WebSocket.StopWebSocket = true;
        state.Socket.CurrentId = null;
        services.Rest.Http.DefaultRequestHeaders.Remove("Auth-Id");
        services.PageManager.OnSwitchPage(new LoginPage
        {
            DataContext = new LoginViewModel(services, main)
        });
    }

    [RelayCommand]
    public void Quit()
    {
        Environment.Exit(0);
    }
}
