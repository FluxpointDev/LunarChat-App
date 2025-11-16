using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using ShadUI;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace LunarChatApp.ViewModels;

public partial class ServersViewModel : ViewModelBase
{
    private PageManager pageManager;
    public TestState state { get; set; }
    private ThemeWatcher themeWatcher;
    private MainViewModel main;
    private RestClient rest;
    public ServersViewModel(PageManager page, TestState st, ThemeWatcher theme, MainViewModel mainModel, RestClient rs)
    {
        pageManager = page;
        state = st;
        themeWatcher = theme;
        main = mainModel;
        st.OnSelectServer += OnSelectServer;
        st.OnSelectChannel += OnSelectChannel;
        if (CrockeryList == null)
            CrockeryList = new ObservableCollection<ServerIcon>(state.Servers.Values.Select(x => new ServerIcon() { DataContext = new ServerIconViewModel(state, page, x.Server) }));

        if (_selectedPage == null)
        {
            if (state.CurrentServer == null)
            {
                _selectedPage = new HomeView();
                _selectedSidebar = new DMsListView();
            }
            else
            {
                _selectedSidebar = new ChannelsListView() { DataContext = new ChannelListViewModel(state, state.CurrentServer) };
                if (state.CurrentChannel != null)
                    _selectedPage = new ChannelView() { DataContext = new ChannelViewModel(state) };
            }
        }

        rest = rs;
    }

    private void OnSelectChannel(Shared.Core.Channels.Channel channel)
    {
        SelectedPage = new ChannelView() { DataContext = new ChannelViewModel(state) };
    }

    private void OnSelectServer(Shared.Core.Servers.Server server)
    {
        SelectedHeader = new ServerHeaderView();
        SelectedSidebar = new ChannelsListView() { DataContext = new ChannelListViewModel(state, state.CurrentServer) };
        SelectedPage = null;
    }

    [ObservableProperty]
    private ObservableCollection<ServerIcon> _crockeryList;

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
        SelectedSidebar = new DMsListView();
        SelectedPage = new HomeView();
    }

    [RelayCommand]
    public void OpenSettings()
    {
        pageManager.OnSwitchPage(new SettingsPage
        {
            DataContext = new SettingsViewModel(pageManager, state, themeWatcher, main)
        });
    }

    [RelayCommand]
    public void Logout()
    {
        pageManager.OnSwitchPage(new LoginPage
        {
            DataContext = new LoginViewModel(pageManager, rest, state, themeWatcher, main)
        });
    }

    [RelayCommand]
    public void Quit()
    {
        Environment.Exit(0);
    }
}
