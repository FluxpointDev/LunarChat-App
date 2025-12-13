using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
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
        if (chan.Type == LunarChatSharp.Core.Channels.ChannelType.Group)
        {
            CanManage = services.Client.CurrentId == chan.GroupSettings?.OwnerId;
        }
        else
        {
            if (services.State.Socket.CurrentServer == null)
                return;

            services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
            canManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageServer);
        }

    }

    private async Task PermissionUpdate()
    {
        CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageServer);
    }

    [ObservableProperty]
    private string? _channelNameEdit;

    [ObservableProperty]
    private string? _channelTopicEdit;

    [ObservableProperty]
    private bool canManage;

    [RelayCommand]
    public async Task UpdateChannel()
    {
        var req = new UpdateChannelRequest();
        if (channel.Name != ChannelNameEdit)
            req.Name = ChannelNameEdit;

        if (channel.Topic != ChannelTopicEdit)
            req.Topic = ChannelTopicEdit;

        try
        {
            await services.Rest.UpdateChannelAsync(channel.Id, req);
        }
        catch { }
    }

    [RelayCommand]
    public async Task DeleteChannel()
    {
        try
        {
            await services.Rest.DeleteChannelAsync(channel.Id, new DeleteChannelRequest
            {
                ServerId = channel.ServerId
            });
        }
        catch { }

    }
}
