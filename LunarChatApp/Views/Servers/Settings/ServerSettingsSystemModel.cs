using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DynamicData;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Servers;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsSystemModel : ViewModelBase
{
    private ServiceManager services;
    public ServerSettingsSystemModel(ServiceManager sv)
    {
        services = sv;
        canManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        Items = new ObservableCollection<ChannelListItem>
        {
            new ChannelListItem
            {
                id = ""
            }
        };
        Items.AddRange(services.State.Socket.CurrentServer.Channels.Values.Select(x => new ChannelListItem
        {
            id = x.Id,
            Name = x.Name
        }));
        UpdateSystemMessages();
    }

    private async Task PermissionUpdate()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ChannelPermission.ManageChannel);
        });

    }

    [ObservableProperty]
    private bool canManage;

    public void UpdateSystemMessages()
    {
        var server = services.State.Socket.CurrentServer;
        if (server == null)
            return;

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.MemberJoined))
        {
            SelectedJoinMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberJoined);
            if (SelectedJoinMessage == null)
                SelectedJoinMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberJoined, Name = "invalid-channel" };
        }
        if (SelectedJoinMessage == null)
            SelectedJoinMessage = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.MemberLeft))
        {
            SelectedLeftMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberLeft);
            if (SelectedLeftMessage == null)
                SelectedLeftMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberLeft, Name = "invalid-channel" };
        }
        if (SelectedLeftMessage == null)
            SelectedLeftMessage = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.MemberBanned))
        {
            SelectedBanMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberBanned);
            if (SelectedBanMessage == null)
                SelectedBanMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberBanned, Name = "invalid-channel" };
        }
        if (SelectedBanMessage == null)
            SelectedBanMessage = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.MemberUnbanned))
        {
            SelectedUnbanMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberUnbanned);
            if (SelectedUnbanMessage == null)
                SelectedUnbanMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberUnbanned, Name = "invalid-channel" };
        }
        if (SelectedUnbanMessage == null)
            SelectedUnbanMessage = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.MemberKicked))
        {
            SelectedKickMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberKicked);
            if (SelectedKickMessage == null)
                SelectedKickMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberKicked, Name = "invalid-channel" };
        }
        if (SelectedKickMessage == null)
            SelectedKickMessage = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.MemberTimedout))
        {
            SelectedTimeoutMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberTimedout);
            if (SelectedTimeoutMessage == null)
                SelectedTimeoutMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberTimedout, Name = "invalid-channel" };
        }
        if (SelectedTimeoutMessage == null)
            SelectedTimeoutMessage = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.AppAdded))
        {
            SelectedAppAdded = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.AppAdded);
            if (SelectedAppAdded == null)
                SelectedAppAdded = new ChannelListItem { id = server.Server.SystemMessages.AppAdded, Name = "invalid-channel" };
        }
        if (SelectedAppAdded == null)
            SelectedAppAdded = Items.First();

        if (!string.IsNullOrEmpty(server.Server.SystemMessages.AppRemoved))
        {
            SelectedAppRemoved = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.AppRemoved);
            if (SelectedAppRemoved == null)
                SelectedAppRemoved = new ChannelListItem { id = server.Server.SystemMessages.AppRemoved, Name = "invalid-channel" };
        }
        if (SelectedAppRemoved == null)
            SelectedAppRemoved = Items.First();
    }

    public bool IsLoaded;

    [ObservableProperty]
    private ObservableCollection<ChannelListItem> _items;

    [ObservableProperty]
    private ChannelListItem? _selectedJoinMessage;

    [ObservableProperty]
    private ChannelListItem? _selectedLeftMessage;

    [ObservableProperty]
    private ChannelListItem? _selectedBanMessage;

    [ObservableProperty]
    private ChannelListItem? _selectedUnbanMessage;

    [ObservableProperty]
    private ChannelListItem? _selectedKickMessage;

    [ObservableProperty]
    private ChannelListItem? _selectedTimeoutMessage;

    [ObservableProperty]
    private ChannelListItem? _selectedAppAdded;

    [ObservableProperty]
    private ChannelListItem? _selectedAppRemoved;

    public Dictionary<string, string> UpdatedProperties = new Dictionary<string, string>();

    [RelayCommand]
    public async Task UpdateSystem()
    {
        try
        {
            await services.Rest.EditServerAsync(services.State.Socket.CurrentServer?.Server.Id!, new LunarChatSharp.Rest.Servers.EditServerRequest
            {
                SystemMessages = new RestServerSystemMessages
                {
                    MemberJoined = SelectedJoinMessage != null ? SelectedJoinMessage.id : "",
                    MemberLeft = SelectedLeftMessage != null ? SelectedLeftMessage.id : "",
                    MemberBanned = SelectedBanMessage != null ? SelectedBanMessage.id : "",
                    MemberUnbanned = SelectedUnbanMessage != null ? SelectedUnbanMessage.id : "",
                    MemberKicked = SelectedKickMessage != null ? SelectedKickMessage.id : "",
                    MemberTimedout = SelectedTimeoutMessage != null ? SelectedTimeoutMessage.id : "",
                    AppAdded = SelectedAppAdded != null ? SelectedAppAdded.id : "",
                    AppRemoved = SelectedAppRemoved != null ? SelectedAppRemoved.id : "",
                }
            });
        }
        catch { }
    }
}
public partial class ChannelListItem : ObservableObject
{
    [ObservableProperty]
    private string _name;

    public string id;
}