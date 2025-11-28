using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.ViewModels.Servers.Channels;
using LunarChatApp.Views;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers;

public partial class ChannelSettingsModel : ViewModelBase
{
    private ServiceManager services;
    private PageManager pageManager;
    private TestState state { get; set; }
    private Channel channel;
    public ChannelSettingsModel(ServiceManager sv, Channel chan)
    {
        services = sv;
        pageManager = sv.PageManager;
        state = sv.State;
        channel = chan;
        ChannelName = chan.Name;
        sv.State.Socket.CurrentServer.OnChannelUpdate += UpdateChannel;
        if (SelectedPage == null)
            SelectedPage = new ChannelSettingsOverview { DataContext = new ChannelSettingsOverviewModel(services, chan) };
    }

    private async Task UpdateChannel(Channel channel)
    {
        ChannelName = channel.Name;
        if (SelectedPage is ChannelSettingsOverview overview)
        {
            ChannelSettingsOverviewModel model = (overview.DataContext as ChannelSettingsOverviewModel);
            model.ChannelNameEdit = channel.Name;
            model.ChannelTopicEdit = channel.Topic;
        }
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
        SelectedPage = new ChannelSettingsOverview { DataContext = new ChannelSettingsOverviewModel(services, channel) };
    }
}
