using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.Shared.Rest.Optional;
using LunarChatApp.Shared.Rest.Servers;
using LunarChatApp.Views;
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
        var data = new UpdateServerRequest();
        data.name = Optional.Some(ServerNameEdit);
        data.description = Optional.Some(ServerDescriptionEdit);
        await services.Rest.PatchAsync<UpdateServerRequest>($"/servers/{services.State.Socket.CurrentServer.Server.Id}", data);
    }

    [RelayCommand]
    public async Task DeleteServer()
    {
        await services.Rest.DeleteAsync("/servers/" + services.State.Socket.CurrentServer.Server.Id);
    }
}
