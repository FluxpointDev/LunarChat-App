using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
using LunarChatSharp.Core.Servers;
using LunarChatSharp.Rest.Servers;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LunarChatApp.ViewModels.Servers.Settings;

public partial class ServerSettingsOverviewModel : ViewModelBase
{
    private ServiceManager services;
    public ServerSettingsOverviewModel(ServiceManager sv)
    {
        services = sv;
        ServerNameEdit = services.State.CurrentServer.Server.Name;
        services.State.CurrentServer.OnPermissionUpdate += PermissionUpdate;
        canManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageServer);
        if (!string.IsNullOrEmpty(services.State.CurrentServer.Server.IconId))
            ServerIcon = new Uri(services.State.CurrentServer.Server.GetIconUrl());
    }

    private async Task PermissionUpdate(RestServer server)
    {
        Avalonia.Threading.Dispatcher.UIThread.Post(() =>
        {
            CanManage = services.State.CurrentServer.HasPermission(services.State.CurrentServer.Member, ServerPermission.ManageServer);
        });

    }

    [ObservableProperty]
    private string? _serverNameEdit;

    [ObservableProperty]
    private Uri serverIcon;

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
                await services.Rest.EditServerAsync(services.State.CurrentServer?.Server?.Id, new EditServerRequest
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
            await services.Rest.EditServerAsync(services.State.CurrentServer?.Server?.Id, new EditServerRequest
            {
                Icon = ""
            });
        }
        catch { }
    }

    [ObservableProperty]
    private bool canManage;

    [RelayCommand]
    public async Task SaveSettings()
    {
        var data = new EditServerRequest();
        data.Name = ServerNameEdit;
        try
        {
            await services.Rest.EditServerAsync(services.State.CurrentServer.Server.Id, data);
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
            await services.Rest.DeleteServerAsync(services.State.CurrentServer.Server.Id);
        }
        catch { }

    }
}
