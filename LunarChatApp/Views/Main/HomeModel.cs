using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatSharp.Rest.Servers;
using System.Threading.Tasks;

namespace LunarChatApp.Views.Main;

public partial class HomeModel(ServiceManager services) : ViewModelBase
{
    [RelayCommand]
    public void DiscoverServer()
    {

    }

    [RelayCommand]
    public void CreateServer()
    {
        services.Dialogs.Create(new JoinServerDialog(), new JoinServerDialogModel(services), "Server").WithSubmit(SubmitServer).Open();
    }

    [RelayCommand]
    public async Task LunarCommunity()
    {
        if (!services.State.Socket.Servers.ContainsKey(services.State.Socket.LunarCommunityId))
            await services.Rest.PutAsync($"/servers/{services.State.Socket.LunarCommunityId}/members/{services.State.Socket.CurrentId}");

        if (services.State.Socket.Servers.TryGetValue(services.State.Socket.LunarCommunityId, out var server))
            services.PageManager.SwitchServer(services, server.Server);
    }


    [RelayCommand]
    public async Task LunarDevs()
    {
        if (!services.State.Socket.Servers.ContainsKey(services.State.Socket.LunarDevId))
            await services.Rest.PutAsync($"/servers/{services.State.Socket.LunarDevId}/members/{services.State.Socket.CurrentId}");

        if (services.State.Socket.Servers.TryGetValue(services.State.Socket.LunarDevId, out var server))
            services.PageManager.SwitchServer(services, server.Server);
    }

    public async Task SubmitServer(UserControl control)
    {
        JoinServerDialogModel? model = control.DataContext as JoinServerDialogModel;
        if (model.ShowJoin)
        {

        }
        else if (model.ShowCreate)
        {
            if (string.IsNullOrEmpty(model.Textbox))
                return;

            await services.Rest.PostAsync<CreateServerRequest>("/servers", new CreateServerRequest
            {
                Name = model.Textbox
            });
        }
    }
}
