using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Servers;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Servers.Settings;

public partial class ServerSettingsDiscoveryModel : ViewModelBase
{
    public ServerSettingsDiscoveryModel(ServiceManager sv)
    {
        services = sv;
        ServerDescriptionEdit = services.State.Socket.CurrentServer.Server.Description;
        vanityInvite = services.State.Socket.CurrentServer.Server.VanityInvite;
        isPublic = services.State.Socket.CurrentServer.Server.Features.HasFlag(ServerFeature.Discoverable);
        services.State.Socket.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        canManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageServer);
    }

    private async Task PermissionUpdate()
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.Socket.CurrentServer.HasPermission(services.State.Socket.CurrentServer.Member, ServerPermission.ManageServer);
        });

    }

    private ServiceManager services;

    [ObservableProperty]
    private string? _serverDescriptionEdit;

    [ObservableProperty]
    private string vanityInvite;

    [ObservableProperty]
    private bool isPublic;

    [ObservableProperty]
    private bool canManage;

    [RelayCommand]
    public async Task SaveSettings()
    {
        var server = services.State.Socket.CurrentServer.Server;
        var data = new EditServerRequest();

        if (server.Description != ServerDescriptionEdit)
            data.Description = ServerDescriptionEdit ?? "";

        if (server.VanityInvite != VanityInvite)
            data.VanityInvite = VanityInvite;

        if (server.Features.HasFlag(ServerFeature.Discoverable) != IsPublic)
            data.IsDiscoverable = IsPublic;
        try
        {
            await services.Rest.EditServerAsync(services.State.Socket.CurrentServer.Server.Id, data);
        }
        catch { }
    }
}
