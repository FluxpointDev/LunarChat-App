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

        if (services.State.CurrentServer != null)
        {
            canManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.ManageChannel);
            services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;

            Items = new ObservableCollection<ChannelListItem>
            {
                new ChannelListItem
                {
                    id = null
                }
            };
            Items.AddRange(services.State.CurrentServer.Channels.Values.Select(x => new ChannelListItem
            {
                id = x.Id,
                Name = x.Name
            }));
        }

        UpdateSystemMessages();
    }

    private async Task PermissionUpdate(RestServer server)
    {
        if (services.State.CurrentServer == null)
            return;

        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ChannelPermission.ManageChannel);
        });

    }

    [ObservableProperty]
    private bool canManage;

    public void UpdateSystemMessages()
    {
        var server = services.State.CurrentServer;
        if (server == null)
            return;

        if (server.Server.SystemMessages.MemberJoined.HasValue)
        {
            SelectedJoinMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberJoined);
            if (SelectedJoinMessage == null)
                SelectedJoinMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberJoined, Name = "invalid-channel" };
        }
        if (SelectedJoinMessage == null)
            SelectedJoinMessage = Items.First();

        if (server.Server.SystemMessages.MemberLeft.HasValue)
        {
            SelectedLeftMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberLeft);
            if (SelectedLeftMessage == null)
                SelectedLeftMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberLeft, Name = "invalid-channel" };
        }
        if (SelectedLeftMessage == null)
            SelectedLeftMessage = Items.First();

        if (server.Server.SystemMessages.MemberBanned.HasValue)
        {
            SelectedBanMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberBanned);
            if (SelectedBanMessage == null)
                SelectedBanMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberBanned, Name = "invalid-channel" };
        }
        if (SelectedBanMessage == null)
            SelectedBanMessage = Items.First();

        if (server.Server.SystemMessages.MemberUnbanned.HasValue)
        {
            SelectedUnbanMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberUnbanned);
            if (SelectedUnbanMessage == null)
                SelectedUnbanMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberUnbanned, Name = "invalid-channel" };
        }
        if (SelectedUnbanMessage == null)
            SelectedUnbanMessage = Items.First();

        if (server.Server.SystemMessages.MemberKicked.HasValue)
        {
            SelectedKickMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberKicked);
            if (SelectedKickMessage == null)
                SelectedKickMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberKicked, Name = "invalid-channel" };
        }
        if (SelectedKickMessage == null)
            SelectedKickMessage = Items.First();

        if (server.Server.SystemMessages.MemberTimedout.HasValue)
        {
            SelectedTimeoutMessage = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.MemberTimedout);
            if (SelectedTimeoutMessage == null)
                SelectedTimeoutMessage = new ChannelListItem { id = server.Server.SystemMessages.MemberTimedout, Name = "invalid-channel" };
        }
        if (SelectedTimeoutMessage == null)
            SelectedTimeoutMessage = Items.First();

        if (server.Server.SystemMessages.AppAdded.HasValue)
        {
            SelectedAppAdded = Items.FirstOrDefault(x => x.id == server.Server.SystemMessages.AppAdded);
            if (SelectedAppAdded == null)
                SelectedAppAdded = new ChannelListItem { id = server.Server.SystemMessages.AppAdded, Name = "invalid-channel" };
        }
        if (SelectedAppAdded == null)
            SelectedAppAdded = Items.First();

        if (server.Server.SystemMessages.AppRemoved.HasValue)
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
        if (services.State.CurrentServer == null)
            return;

        try
        {
            await services.Rest.EditServerAsync(services.State.CurrentServer.Server.Id, new LunarChatSharp.Rest.Servers.EditServerRequest
            {
                SystemMessages = new RestServerSystemMessages
                {
                    MemberJoined = SelectedJoinMessage != null ? SelectedJoinMessage.id : 0,
                    MemberLeft = SelectedLeftMessage != null ? SelectedLeftMessage.id : 0,
                    MemberBanned = SelectedBanMessage != null ? SelectedBanMessage.id : 0,
                    MemberUnbanned = SelectedUnbanMessage != null ? SelectedUnbanMessage.id : 0,
                    MemberKicked = SelectedKickMessage != null ? SelectedKickMessage.id : 0,
                    MemberTimedout = SelectedTimeoutMessage != null ? SelectedTimeoutMessage.id : 0,
                    AppAdded = SelectedAppAdded != null ? SelectedAppAdded.id : 0,
                    AppRemoved = SelectedAppRemoved != null ? SelectedAppRemoved.id : 0,
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

    public ulong? id;
}