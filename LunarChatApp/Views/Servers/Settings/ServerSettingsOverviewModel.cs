using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Servers;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers.Settings;

public partial class ServerSettingsOverviewModel : ViewModelBase
{
    private ServiceManager services;
    public ServerSettingsOverviewModel(ServiceManager sv)
    {
        services = sv;
        ServerNameEdit = services.State.Socket.CurrentServer.Server.Name;
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

    [ObservableProperty]
    private string? _serverNameEdit;

    [ObservableProperty]
    private bool canManage;

    [RelayCommand]
    public async Task SaveSettings()
    {
        var data = new EditServerRequest();
        data.Name = ServerNameEdit;
        try
        {
            await services.Rest.EditServerAsync(services.State.Socket.CurrentServer.Server.Id, data);
        }
        catch { }
    }

    [RelayCommand]
    public void TransferOwnership()
    {

    }

    [RelayCommand]
    public async Task DeleteServer()
    {
        try
        {
            await services.Rest.DeleteServerAsync(services.State.Socket.CurrentServer.Server.Id);
        }
        catch { }

    }
}
