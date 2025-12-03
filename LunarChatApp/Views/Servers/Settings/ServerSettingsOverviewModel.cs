using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Views;
using LunarChatSharp;
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
        ServerDescriptionEdit = services.State.Socket.CurrentServer.Server.Description;
    }

    [ObservableProperty]
    private string? _serverNameEdit;

    [ObservableProperty]
    private string? _serverDescriptionEdit;

    [RelayCommand]
    public async Task SaveSettings()
    {
        var data = new EditServerRequest();
        data.Name = ServerNameEdit;
        data.Description = ServerDescriptionEdit;
        await services.Rest.EditServerAsync(services.State.Socket.CurrentServer.Server.Id, data);
    }

    [RelayCommand]
    public async Task DeleteServer()
    {
        await services.Rest.LeaveServerAsync(services.State.Socket.CurrentServer.Server.Id);
    }
}
