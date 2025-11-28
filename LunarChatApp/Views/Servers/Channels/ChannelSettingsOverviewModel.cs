using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Core.Channels;
using LunarChatApp.Shared.Rest.Channels;
using LunarChatApp.Views;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers.Channels;

public partial class ChannelSettingsOverviewModel : ViewModelBase
{
    private Channel channel;
    private ServiceManager services;
    public ChannelSettingsOverviewModel(ServiceManager sv, Channel chan)
    {
        services = sv;
        channel = chan;
        ChannelNameEdit = chan.Name;
        ChannelTopicEdit = chan.Topic;
    }

    [ObservableProperty]
    private string? _channelNameEdit;

    [ObservableProperty]
    private string? _channelTopicEdit;

    [RelayCommand]
    public async Task UpdateChannel()
    {
        await services.Rest.PatchAsync($"/channels/{channel.Id}", new UpdateChannelRequest
        {
            server_id = channel.ServerId,
            name = ChannelNameEdit,
            topic = ChannelTopicEdit
        });
    }

    [RelayCommand]
    public async Task DeleteChannel()
    {
        await services.Rest.DeleteAsync($"/channels/{channel.Id}", new UpdateChannelRequest
        {
            server_id = channel.ServerId
        });
    }
}
