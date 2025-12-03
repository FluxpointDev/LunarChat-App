using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Rest.Channels;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Channels.Settings;

public partial class ChannelSettingsOverviewModel : ViewModelBase
{
    private RestChannel channel;
    private ServiceManager services;
    public ChannelSettingsOverviewModel(ServiceManager sv, RestChannel chan)
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
        await services.Rest.UpdateChannelAsync(channel.Id, new UpdateChannelRequest
        {
            Name = ChannelNameEdit,
            Topic = ChannelTopicEdit
        });
    }

    [RelayCommand]
    public async Task DeleteChannel()
    {
        await services.Rest.DeleteChannelAsync(channel.Id, new DeleteChannelRequest
        {
            ServerId = channel.ServerId
        });
    }
}
