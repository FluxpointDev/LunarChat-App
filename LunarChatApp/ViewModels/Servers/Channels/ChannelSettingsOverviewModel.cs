using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Shared.Core.Channels;

namespace LunarChatApp.ViewModels.Servers.Channels;

public partial class ChannelSettingsOverviewModel : ViewModelBase
{
    private Channel channel;
    public ChannelSettingsOverviewModel(Channel chan)
    {
        channel = chan;
        ChannelNameEdit = chan.Name;
        ChannelTopicEdit = chan.Topic;
    }

    [ObservableProperty]
    private string? _channelNameEdit;

    [ObservableProperty]
    private string? _channelTopicEdit;

    [RelayCommand]
    public void DeleteChannel()
    {

    }
}
