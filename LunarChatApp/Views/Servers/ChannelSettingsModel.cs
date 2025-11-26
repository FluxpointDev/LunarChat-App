using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.ViewModels.Servers.Channels;
using LunarChatApp.Views;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelSettingsModel : ViewModelBase
{
    private PageManager pageManager;
    private TestState state { get; set; }
    private Channel channel;
    public ChannelSettingsModel(PageManager page, TestState st, Channel chan)
    {
        pageManager = page;
        state = st;
        channel = chan;
        ChannelName = chan.Name;
        if (SelectedPage == null)
            SelectedPage = new ChannelSettingsOverview { DataContext = new ChannelSettingsOverviewModel(chan) };
    }

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
        SelectedPage = new ChannelSettingsOverview { DataContext = new ChannelSettingsOverviewModel(channel) };
    }
}
