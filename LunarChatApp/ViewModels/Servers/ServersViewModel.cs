using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatApp.ViewModels.Servers;
using LunarChatApp.ViewModels.User;
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
        services.State.OnSelectServer += OnSelectServer;
        services.State.OnSelectChannel += OnSelectChannel;

        if (_selectedPage == null)
        {
            if (state.CurrentServer == null)
            {
                _selectedPage = new HomeView();
                _selectedSidebar = new DMsListView { DataContext = new DMsListModel(services) };
            }
            else
            {
                _selectedSidebar = new ChannelsListView() { DataContext = new ChannelListViewModel(services, state) };
                if (state.CurrentChannel != null)
                    _selectedPage = new ChannelView() { DataContext = new ChannelViewModel(state, services, null) };
            }
        }

        if (ServersList == null)
            ServersList = new ObservableCollection<ServerIcon>(state.Servers.Values.Select(x => new ServerIcon() { DataContext = new ServerIconViewModel(state, services.PageManager, x.Server) }));
    }

    private void State_OnPageSelect(UserControl control)
    {
        SelectedPage = control;
    }

    private void OnSelectChannel(Shared.Core.Channels.Channel channel, Shared.Core.Users.User user)
    {
        SelectedPage = new ChannelView() { DataContext = new ChannelViewModel(state, services, user) };
    }

    private void OnSelectServer(Shared.Core.Servers.Server server)
    {
        SelectedHeader = new ServerHeaderView() { DataContext = new ServerHeaderViewModel(services) };
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
        SelectedHeader = null;
        SelectedSidebar = new DMsListView { DataContext = new DMsListModel(services) };
        SelectedPage = new HomeView();
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
        services.Dialogs.Create(new StatusDialogModel(state), "Set Status").WithSubmit(SubmitStatus).Open();
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
        state.WebSocket.StopWebSocket = true;
        state.WebSocket = null;
        state.CurrentId = null;
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
