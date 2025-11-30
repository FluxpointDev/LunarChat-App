using Avalonia.Controls;
using CommunityToolkit.Mvvm.Input;
using LunarChatApp.Services;
using LunarChatApp.ViewModels.Dialogs;
using LunarChatSharp.Rest.Servers;
using System.Linq;
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
    public void LunarCommunity()
    {
        services.PageManager.SwitchServer(services, services.State.Socket.Servers.Values.FirstOrDefault(x => x.Server.Name == "Lunar Community").Server);
    }


    [RelayCommand]
    public void LunarDevs()
    {
        services.PageManager.SwitchServer(services, services.State.Socket.Servers.Values.FirstOrDefault(x => x.Server.Name == "Lunar Developers").Server);
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
