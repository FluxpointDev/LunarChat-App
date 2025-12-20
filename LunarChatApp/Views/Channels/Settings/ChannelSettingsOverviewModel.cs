using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Channels;
using LunarChatSharp.Rest.Servers;
using System;
using System.IO;
using System.Linq;
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
        services.Client.OnGroupUpdate += GroupUpdate;
        if (chan.Type == LunarChatSharp.Core.Channels.ChannelType.Group)
        {
            isGroup = true;
            CanManage = services.Client.CurrentId == chan.GroupSettings?.OwnerId;
            if (!string.IsNullOrEmpty(chan.GroupSettings.IconId))
                GroupIcon = new Uri(chan.GroupSettings.GetIconUrl());
        }
        else
        {
            if (services.State.CurrentServer == null)
                return;

            services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
            canManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageServer);
        }

    }

    private async Task GroupUpdate(RestChannel channel, UpdateChannelRequest req)
    {
        if (channel.Id != services.State.CurrentChannel?.Id || req.Icon == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            if (req.Icon == "")
            {
                GroupIcon = null;
            }
            else
            {
                GroupIcon = new Uri(channel.GroupSettings.GetIconUrl());
            }
        });
    }

    private async Task PermissionUpdate(RestServer server)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageServer);
        });

    }

    [ObservableProperty]
    private string? _channelNameEdit;

    [ObservableProperty]
    private bool isGroup;

    [ObservableProperty]
    private string? _channelTopicEdit;

    [ObservableProperty]
    private bool canManage;

    [ObservableProperty]
    private Uri groupIcon;

    [RelayCommand]
    public async Task UpdateChannel()
    {
        var req = new UpdateChannelRequest();
        if (channel.Name != ChannelNameEdit)
            req.Name = ChannelNameEdit;

        if (channel.Topic != ChannelTopicEdit)
            req.Topic = ChannelTopicEdit ?? "";

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

    [RelayCommand]
    public async Task UploadIcon()
    {
        var files = await services.FilePicker();
        if (!files.Any())
            return;

        _ = Task.Run(async () =>
        {
            using (Stream stream = await files.First().OpenReadAsync())
            {
                await services.Rest.UpdateChannelAsync(channel.Id, new UpdateChannelRequest
                {
                    Icon = Utils.GetImageBase64(stream)
                });
            }
        });
    }

    [RelayCommand]
    public async Task ClearIcon()
    {
        try
        {
            await services.Rest.UpdateChannelAsync(channel.Id, new UpdateChannelRequest
            {
                Icon = ""
            });
        }
        catch { }
    }
}
